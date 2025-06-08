using System.Reflection;
using BucketProject.BLL.Business_Logic.InterfacesRepo;
using BucketProject.DAL.Models.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BucketProject.DAL.Data.Repositories
{
    public class SocialRepo : Repository, ISocialRepo
    {
        private readonly ILogger<ISocialRepo> _logger;
        public SocialRepo(IConfiguration configuration, ILogger<SocialRepo> logger) : base(configuration)
        {
            _logger = logger;
        }


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
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx,
                    "SQL error in LoadFriends (UserId={userId})",
                    userId);

                throw new Exception("A database error occurred while loading friends.", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "SQL error in LoadFriends (UserId={userId})",
                    userId);

                throw;
            }
            return friends;
        }

        public List<UserEntity> LoadNonFriends(int userId, string searchTerm, int page, int pageSize)
        {
            var users = new List<UserEntity>();
            const string sql = @"
SELECT u.UserId, u.Username, u.Picture
FROM [User] AS u
WHERE u.UserId <> @Id
  AND u.Role = 'User'
  AND u.Username LIKE '%' + @SearchTerm + '%'
  AND u.UserId NOT IN (
      SELECT CASE WHEN f.UserId = @Id THEN f.FriendId ELSE f.UserId END
      FROM Friends f
      WHERE (f.UserId = @Id OR f.FriendId = @Id)
        AND f.Status IN ('Pending','Accepted')
  )
ORDER BY u.Username
OFFSET @Offset ROWS
FETCH NEXT @PageSize ROWS ONLY;
";

            try
            {
                using var conn = GetSqlConnection();
                conn.Open();
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", userId);
                cmd.Parameters.AddWithValue("@SearchTerm", searchTerm ?? string.Empty);
                cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    users.Add(new UserEntity(
                        rdr.GetInt32(rdr.GetOrdinal("UserId")),
                        rdr.GetString(rdr.GetOrdinal("Username")),
                        rdr.IsDBNull(rdr.GetOrdinal("Picture")) ? null : (byte[])rdr["Picture"]
                    ));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading non-friends with pagination");
                throw;
            }

            return users;
        }




        public bool SendFriendRequest(int userId, int friendId)
        {
            if (userId == friendId)
                throw new ArgumentException("A user cannot befriend themself.");

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
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx,
                    "SQL error in SendFriendRequest (UserId={userId}, FriendId={friendId})",
                    userId, friendId);

                throw new Exception("A database error occurred while sending freind request.", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                   "SQL error in SendFriendRequest (UserId={userId}, FriendId={friendId})",
                   userId, friendId);

                throw;
            }
        }


        public bool RespondToFriendRequest(int userId, int requesterId, bool accept)
        {
            int u = Math.Min(userId, requesterId);
            int f = Math.Max(userId, requesterId);

            const string sql = @"
        UPDATE dbo.Friends
           SET Status = @Status
         WHERE UserId = @U
           AND FriendId = @F
           AND Status = 'Pending';
    ";

            try
            {
                using var conn = GetSqlConnection();
                conn.Open();

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Status", accept ? "Accepted" : "Declined");
                cmd.Parameters.AddWithValue("@U", u);
                cmd.Parameters.AddWithValue("@F", f);

                return cmd.ExecuteNonQuery() == 1;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error in RespondToFriendRequest. UserId: {UserId}, RequesterId: {RequesterId}, Accept: {Accept}", userId, requesterId, accept);
                throw new Exception("A database error occurred while responding to the friend request.", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in RespondToFriendRequest. UserId: {UserId}, RequesterId: {RequesterId}, Accept: {Accept}", userId, requesterId, accept);
                throw;
            }
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
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx,
                    "SQL error in LoadIncomingRequests (UserId={userId})",
                    userId);

                throw new Exception("A database error occurred while loading incoming friend requests.", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                     "SQL error in LoadIncomingRequests (UserId={userId})",
                     userId);

                throw;
            }
            return list;
        }

        public bool TryRemoveFriend(int userId, int friendId)
        {
            if (userId == friendId)
                throw new ArgumentException("A user cannot unfriend themself.");

            const string sql = @"
DELETE FROM dbo.Friends
WHERE 
  UserId = CASE WHEN @UserId < @FriendId THEN @UserId ELSE @FriendId END
  AND FriendId = CASE WHEN @UserId < @FriendId THEN @FriendId ELSE @UserId END;
";

            try
            {
                using var conn = GetSqlConnection();
                conn.Open();

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@FriendId", friendId);

                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error in TryRemoveFriend. UserId: {UserId}, FriendId: {FriendId}", userId, friendId);
                throw new Exception("A database error occurred while removing the friend.", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in TryRemoveFriend. UserId: {UserId}, FriendId: {FriendId}", userId, friendId);
                throw;
            }
        }

        public List<UserEntity> LoadOutgoingRequests(int userId)
        {
            var list = new List<UserEntity>();

            const string sql = @"
SELECT u.UserId, u.Username, u.Picture
FROM dbo.[User] AS u
JOIN dbo.Friends AS f
  ON ( (f.UserId = @Id AND u.UserId = f.FriendId)
    OR (f.FriendId = @Id AND u.UserId = f.UserId) )
WHERE f.Status = 'Pending'
  AND f.RequestedBy = @Id;
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
                        rdr.IsDBNull(rdr.GetOrdinal("Picture")) ? null : (byte[])rdr["Picture"]
                    ));
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error in LoadOutgoingRequests. UserId: {UserId}", userId);
                throw new Exception("A database error occurred while loading outgoing friend requests.", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in LoadOutgoingRequests. UserId: {UserId}", userId);
                throw;
            }

            return list;
        }

        public UserEntity GetUserById(int userId)
        {
            const string sql = "SELECT UserId, Username FROM [User] WHERE UserId = @UserId;";

            try
            {
                using var conn = GetSqlConnection();
                conn.Open();

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                using var rdr = cmd.ExecuteReader();
                if (!rdr.Read())
                    return null;

                return new UserEntity(
                    rdr.GetInt32(0),
                    rdr.GetString(1)
                );
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error in GetUserById. UserId: {UserId}", userId);
                throw new Exception("A database error occurred while retrieving the user.", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetUserById. UserId: {UserId}", userId);
                throw;
            }
        }

        public int CountNonFriends(int userId, string searchTerm)
        {
            const string sql = @"
SELECT COUNT(*)
FROM [User] AS u
WHERE u.UserId <> @Id
  AND u.Role = 'User'
  AND u.Username LIKE '%' + @SearchTerm + '%'
  AND u.UserId NOT IN (
      SELECT CASE WHEN f.UserId = @Id THEN f.FriendId ELSE f.UserId END
      FROM Friends f
      WHERE (f.UserId = @Id OR f.FriendId = @Id)
        AND f.Status IN ('Pending','Accepted')
  )
";

            try
            {
                using var conn = GetSqlConnection();
                conn.Open();
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", userId);
                cmd.Parameters.AddWithValue("@SearchTerm", searchTerm ?? string.Empty);
                return (int)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting non-friends");
                throw;
            }
        }
    }

    }
