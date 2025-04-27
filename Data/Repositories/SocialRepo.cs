
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
        public SocialRepo(IConfiguration configuration) : base(configuration)
        {

        }

        public List<UserEntity> LoadFriends(int userId)
        {
            var friends = new List<UserEntity>();

            try
            {
                using (SqlConnection conn = GetSqlConnection())
                {
                    conn.Open();

                    const string Sql = @"
                SELECT  u.UserId,         
                        u.Username,
                        u.Picture
                FROM    dbo.[User] AS u
                WHERE   u.UserId IN (
                       SELECT FriendId FROM dbo.Friends WHERE UserId   = @Id
                       UNION
                       SELECT UserId   FROM dbo.Friends WHERE FriendId = @Id
                );";

                    using (SqlCommand cmd = new SqlCommand(Sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                
                                var friend = new UserEntity(
                                    reader.GetInt32(reader.GetOrdinal("UserId")),
                                    reader.GetString(reader.GetOrdinal("Username")),
                                    reader.IsDBNull(reader.GetOrdinal("Picture"))
                                        ? null
                                        : (byte[])reader["Picture"]
                                );
                                

                                friends.Add(friend);
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while loading: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
            }

            return friends;
        }
        public List<UserEntity> LoadNonFriends(int userId)
        {
            var users = new List<UserEntity>();

            try
            {
                using (var conn = GetSqlConnection())
                {
                    conn.Open();

                    const string Sql = @"
SELECT  
    u.UserId,
    u.Username,
    u.Picture
FROM dbo.[User] AS u
WHERE 
    u.UserId <> @Id
    AND u.Role = 'User'                 
    AND u.UserId NOT IN (
        SELECT FriendId FROM dbo.Friends WHERE UserId   = @Id 
        UNION
        SELECT UserId   FROM dbo.Friends WHERE FriendId = @Id
    );
";

                    using (var cmd = new SqlCommand(Sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", userId);

                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                users.Add(new UserEntity(
                                    rdr.GetInt32(rdr.GetOrdinal("UserId")),
                                    rdr.GetString(rdr.GetOrdinal("Username")),
                                    rdr.IsDBNull(rdr.GetOrdinal("Picture"))
                                        ? null
                                        : (byte[])rdr["Picture"]
                                ));
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while loading: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
            }

            return users;
        }


        public bool TryAddFriend(int userId, int friendId)
        {
            if (userId == friendId)
                throw new ArgumentException("A user cannot befriend themself.");

            const string Sql = @"
        INSERT INTO dbo.Friends (UserId, FriendId)
        SELECT
          CASE WHEN @UserId < @FriendId THEN @UserId ELSE @FriendId END,
          CASE WHEN @UserId < @FriendId THEN @FriendId ELSE @UserId END;
    ";

            using var conn = GetSqlConnection();
            conn.Open();

            using var cmd = new SqlCommand(Sql, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@FriendId", friendId);

            try
            {
                cmd.ExecuteNonQuery();      
                return true;                
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while befriending: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
            }
        }

        public bool TryRemoveFriend(int userId, int friendId)
        {
            if (userId == friendId)
                throw new ArgumentException("A user cannot unfriend themself.");

            const string Sql = @"
        DELETE FROM dbo.Friends
        WHERE 
          UserId   = CASE WHEN @UserId   < @FriendId THEN @UserId   ELSE @FriendId END
          AND
          FriendId = CASE WHEN @UserId   < @FriendId THEN @FriendId ELSE @UserId   END;
    ";

            using var conn = GetSqlConnection();
            conn.Open();

            using var cmd = new SqlCommand(Sql, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@FriendId", friendId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;   
        }

    }

}

