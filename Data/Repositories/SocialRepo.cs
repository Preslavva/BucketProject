using System.Data;
using System.Reflection;
using BucketProject.DAL.Data.InterfacesRepo;
using BucketProject.DAL.Data.Repositories;
using BucketProject.DAL.Models.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Data.Repositories
{
    public class SocialRepo : Repository, ISocialRepo
    {
        public SocialRepo(IConfiguration configuration) : base(configuration) { }

    
        public List<UserEntity> LoadFriends(int userId)
        {
            var friends = new List<UserEntity>();
            const string sql = @"
SELECT u.UserId, u.Username, u.Picture
FROM [User] AS u
INNER JOIN Friends AS f
  ON (f.UserId   = @Id AND u.UserId = f.FriendId)
  OR (f.FriendId = @Id AND u.UserId = f.UserId)
WHERE f.Status = 'Accepted';
";
            try
            {
                using var conn = GetSqlConnection();
                conn.Open();
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", userId);
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    friends.Add(new UserEntity(
                        rdr.GetInt32(rdr.GetOrdinal("UserId")),
                        rdr.GetString(rdr.GetOrdinal("Username")),
                        rdr.IsDBNull(rdr.GetOrdinal("Picture"))
                          ? null
                          : (byte[])rdr["Picture"]));
                }
            }
            catch (SqlException ex) { throw new Exception($"DB error in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex); }
            catch (Exception ex) { throw new Exception($"Unexpected error in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex); }
            return friends;
        }

        public List<UserEntity> LoadNonFriends(int userId)
        {
            var users = new List<UserEntity>();
            const string sql = @"
SELECT u.UserId, u.Username, u.Picture
FROM [User] AS u
WHERE u.UserId <> @Id
  AND u.Role = 'User'
  AND u.UserId NOT IN (
      SELECT CASE WHEN f.UserId   = @Id THEN f.FriendId ELSE f.UserId END
      FROM Friends f
      WHERE (f.UserId = @Id OR f.FriendId = @Id)
        AND f.Status IN ('Pending','Accepted')
  );
";
            try
            {
                using var conn = GetSqlConnection();
                conn.Open();
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", userId);
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    users.Add(new UserEntity(
                        rdr.GetInt32(rdr.GetOrdinal("UserId")),
                        rdr.GetString(rdr.GetOrdinal("Username")),
                        rdr.IsDBNull(rdr.GetOrdinal("Picture"))
                          ? null
                          : (byte[])rdr["Picture"]));
                }
            }
            catch (SqlException ex) { throw new Exception($"DB error in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex); }
            catch (Exception ex) { throw new Exception($"Unexpected error in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex); }
            return users;
        }


        public bool SendFriendRequest(int userId, int friendId)
        {
            if (userId == friendId)
                throw new ArgumentException("A user cannot befriend themself.");

            // ensure the smaller ID is always in @UserIdNorm so the PK is consistent
            int userIdNorm = Math.Min(userId, friendId);
            int friendIdNorm = Math.Max(userId, friendId);

            const string sql = @"
BEGIN TRANSACTION;

UPDATE Friends
SET
    Status      = 'Pending',
    RequestedBy = @RequestedBy
WHERE
    UserId       = @UserIdNorm
    AND FriendId = @FriendIdNorm
    AND Status   = 'Declined';

IF @@ROWCOUNT = 0
BEGIN
    INSERT INTO dbo.Friends (UserId, FriendId, Status, RequestedBy)
    VALUES (@UserIdNorm, @FriendIdNorm, 'Pending', @RequestedBy);
END

COMMIT TRANSACTION;
";
            try
            {
                using var conn = GetSqlConnection();
                conn.Open();
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@UserIdNorm", userIdNorm);
                cmd.Parameters.AddWithValue("@FriendIdNorm", friendIdNorm);
                cmd.Parameters.AddWithValue("@RequestedBy", userId);

                cmd.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                // you’ll no longer get a PK violation here
                throw new Exception($"DB error in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
            }
        }


        public bool RespondToFriendRequest(int userId, int requesterId, bool accept)
        {
            int u = Math.Min(userId, requesterId),
                f = Math.Max(userId, requesterId);
            const string sql = @"
UPDATE dbo.Friends
   SET Status    = @Status
 WHERE UserId   = @U
   AND FriendId = @F
   AND Status   = 'Pending';
";
            using var conn = GetSqlConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Status", accept ? "Accepted" : "Declined");
            cmd.Parameters.AddWithValue("@U", u);
            cmd.Parameters.AddWithValue("@F", f);
            return cmd.ExecuteNonQuery() == 1;
        }

     
        public List<UserEntity> LoadIncomingRequests(int userId)
        {
            var list = new List<UserEntity>();
            const string sql = @"
SELECT 
    u.UserId,
    u.Username,
    u.Picture
FROM dbo.[User] AS u
JOIN dbo.Friends AS f
  ON ( (f.UserId   = @Id AND u.UserId = f.FriendId)
    OR (f.FriendId = @Id AND u.UserId = f.UserId) )
WHERE f.Status      = 'Pending'
  AND f.RequestedBy <> @Id;
";
            try
            {
                using var conn = GetSqlConnection();
                conn.Open();
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", userId);
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new UserEntity(
                        rdr.GetInt32(rdr.GetOrdinal("UserId")),
                        rdr.GetString(rdr.GetOrdinal("Username")),
                        rdr.IsDBNull(rdr.GetOrdinal("Picture"))
                          ? null
                          : (byte[])rdr["Picture"]));
                }
            }
            catch (SqlException ex) { throw new Exception($"DB error in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex); }
            catch (Exception ex) { throw new Exception($"Unexpected error in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex); }
            return list;
        }

        /// <summary>
        /// Remove an existing friendship (or cancel a pending/declined request).
        /// </summary>
        public bool TryRemoveFriend(int userId, int friendId)
        {
            if (userId == friendId)
                throw new ArgumentException("A user cannot unfriend themself.");

            const string sql = @"
DELETE FROM dbo.Friends
WHERE 
  UserId   = CASE WHEN @UserId < @FriendId THEN @UserId   ELSE @FriendId END
  AND FriendId = CASE WHEN @UserId < @FriendId THEN @FriendId ELSE @UserId   END;
";
            using var conn = GetSqlConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@FriendId", friendId);
            int rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }
        public List<UserEntity> LoadOutgoingRequests(int userId)
        {
            var list = new List<UserEntity>();
            const string sql = @"
SELECT u.UserId, u.Username, u.Picture
FROM dbo.[User] AS u
JOIN dbo.Friends AS f
  ON ( (f.UserId   = @Id AND u.UserId = f.FriendId)
    OR (f.FriendId = @Id AND u.UserId = f.UserId) )
WHERE f.Status      = 'Pending'
  AND f.RequestedBy = @Id;
";
            using var conn = GetSqlConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", userId);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new UserEntity(
                    rdr.GetInt32(rdr.GetOrdinal("UserId")),
                    rdr.GetString(rdr.GetOrdinal("Username")),
                    rdr.IsDBNull(rdr.GetOrdinal("Picture")) ? null : (byte[])rdr["Picture"]
                ));
            }
            return list;
        }

    }

}
