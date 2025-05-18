using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.DAL.Data.InterfacesRepo;
using BucketProject.DAL.Models.Entities;
using BucketProject.DAL.Models.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BucketProject.DAL.Data.Repositories
{
    public class ManagerRepo : Repository, IManagerRepo
    {
        private readonly ILogger<ManagerRepo> _logger;

        public ManagerRepo(IConfiguration configuration, ILogger<ManagerRepo> logger) : base(configuration)
        {
            _logger = logger;
        }

        public List<UserEntity> GetAllUsers()
        {
            var users = new List<UserEntity>();

            try
            {
                using (SqlConnection sqlConn = GetSqlConnection())
                {
                    sqlConn.Open();

                    string query = @"
                SELECT UserId, [Username], Email, [Password], Picture, Salt, Nationality, DateOfBirth, Gender, CreatedAt,[Role]
                FROM [User]
                WHERE [Role] <> @Role";


                    using (SqlCommand cmd = new SqlCommand(query, sqlConn))
                    {
                        cmd.Parameters.AddWithValue("@Role", "Manager");

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new UserEntity(
                                    id: (int)reader["UserId"],
                                    username: reader["Username"].ToString(),
                                    email: reader["Email"].ToString(),
                                    password: reader["Password"].ToString(),
                                    picture: reader.IsDBNull(reader.GetOrdinal("Picture")) ? null : (byte[])reader["Picture"],
                                    salt: reader["Salt"].ToString(),
                                    nationality: reader["Nationality"].ToString(),
                                    dateOfBirth: (DateTime)reader["DateOfBirth"],
                                    gender: reader["Gender"].ToString(),
                                    createdAt: DateOnly.FromDateTime((DateTime)reader["CreatedAt"]),
                                    role: reader["Role"].ToString()
                                ));
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error in GetAllUsers");
                throw new Exception("A database error occurred while retrieving all users.", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllUsers");
                throw;
            }

            return users;
        }

        public List<GoalEntity> LoadAllPersonalGoals()
        {
            List<GoalEntity> goals = new List<GoalEntity>();

            try
            {
                using (SqlConnection conn = GetSqlConnection())
                {
                    conn.Open();

                    string queryGetGoals = @"
 SELECT 
    g.Id,
    g.Category,
    g.[Description],
    ug.IsDone,
    g.IsDeleted,
    g.CreatedAt,
    g.Deadline,
    g.Type,
    ug.CompletedAt,
    g.IsPostponed,
    g.ParentGoalId,
    g.OwnerId
FROM dbo.Goal AS g
JOIN dbo.User_Goal AS ug
  ON g.Id = ug.GoalId
WHERE g.IsDeleted = 0
  AND (
    SELECT COUNT(*) 
    FROM dbo.User_Goal AS x 
    WHERE x.GoalId = g.Id
  ) = 1


;";

                    using (SqlCommand loadGoals = new SqlCommand(queryGetGoals, conn))
                    {
                        loadGoals.Parameters.AddWithValue("@IsDeleted", false);

                        using (SqlDataReader reader = loadGoals.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                Category cat = Enum.Parse<Category>(reader.GetString(1));
                                string desc = reader.GetString(2);
                                bool isDone = reader.GetBoolean(3);
                                bool isDeleted = reader.GetBoolean(4);
                                DateTime createdAt = reader.GetDateTime(5);
                                DateTime? deadline = reader.IsDBNull(6) ? null : reader.GetDateTime(6);
                                GoalType type = Enum.Parse<GoalType>(reader.GetString(7));
                                DateTime? completedAt = reader.IsDBNull(8) ? null : reader.GetDateTime(8);
                                bool isPostponed = reader.GetBoolean(9);
                                int? parentId = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10);
                                int ownerId = reader.GetInt32(11);



                                GoalEntity goal = new GoalEntity(
                                    id,
                                    cat,
                                    type,
                                    desc,
                                    createdAt,
                                    deadline,
                                    completedAt,
                                    isDone,
                                    isDeleted,
                                    isPostponed,
                                    parentId,
                                    ownerId
                                );

                                goals.Add(goal);
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error in LoadAllPersonalGoals()");
                throw new Exception("A database error occurred while loading all personal goals.", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LoadAllPersonalGoals()");
                throw;
            }

            return goals;
        }


        public List<GoalEntity> LoadAllSharedGoals()
        {
            var goals = new List<GoalEntity>();

            const string queryGetGoals = @"
SELECT
  g.Id,
  g.Category,
  g.[Description],
  ug.IsDone,
  g.IsDeleted,
  g.CreatedAt,
  g.Deadline,
  g.Type,
  ug.CompletedAt,
  g.IsPostponed,
  g.ParentGoalId,
  g.OwnerId
FROM dbo.Goal AS g
JOIN dbo.User_Goal AS ug
  ON g.Id = ug.GoalId
WHERE
  g.IsDeleted = 0
  AND (
    SELECT COUNT(*)
    FROM dbo.User_Goal AS x
    WHERE x.GoalId = g.Id
  ) > 1
  AND ug.UserId = g.OwnerId
";

            try
            {
                using var conn = GetSqlConnection();
                conn.Open();
                using var cmd = new SqlCommand(queryGetGoals, conn);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var goal = new GoalEntity(
                        id: reader.GetInt32(0),
                        category: Enum.Parse<Category>(reader.GetString(1)),
                        type: Enum.Parse<GoalType>(reader.GetString(7)),
                        description: reader.GetString(2),
                        createdAt: reader.GetDateTime(5),
                        deadline: reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                        completedAt: reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                        isDone: reader.GetBoolean(3),
                        isDeleted: reader.GetBoolean(4),
                        isPostponed: reader.GetBoolean(9),
                        parentGoalId: reader.IsDBNull(10) ? null : reader.GetInt32(10),
                        ownerId: reader.GetInt32(11)
                    );
                    goals.Add(goal);
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error in LoadAllSharedGoals()");
                throw new Exception("A database error occurred while loading shared goals.", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LoadAllSharedGoals()");
                throw;
            }

            return goals;
        }

        public List<GoalEntity> GetAllGoals()
        {
            const string query = @"
SELECT 
    Id, 
    Category, 
    [Description], 
    IsDeleted, 
    CreatedAt, 
    Deadline, 
    Type, 
    IsPostponed, 
    ParentGoalId, 
    OwnerId
FROM dbo.Goal
WHERE IsDeleted = 0;
";

            var goals = new List<GoalEntity>();

            try
            {
                using var conn = GetSqlConnection();
                conn.Open();

                using var cmd = new SqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    Category category = Enum.Parse<Category>(reader.GetString(1));
                    string description = reader.GetString(2);
                    bool isDeleted = reader.GetBoolean(3);
                    DateTime createdAt = reader.GetDateTime(4);
                    DateTime? deadline = reader.IsDBNull(5) ? null : reader.GetDateTime(5);
                    GoalType type = Enum.Parse<GoalType>(reader.GetString(6));
                    bool isPostponed = reader.GetBoolean(7);
                    int? parentId = reader.IsDBNull(8) ? null : reader.GetInt32(8);
                    int ownerId = reader.GetInt32(9);

                    bool isDone = false;
                    DateTime? completedAt = null;

                    goals.Add(new GoalEntity(
                        id,
                        category,
                        type,
                        description,
                        createdAt,
                        deadline,
                        completedAt,
                        isDone,
                        isDeleted,
                        isPostponed,
                        parentId,
                        ownerId
                    ));
                }

                return goals;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error while loading all goals.");
                throw new Exception("Database error while loading goals.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while loading all goals.");
                throw;
            }
        }

        public int GetActiveUsersCount()
        {
            int count = 0;

            try
            {
                using (SqlConnection sqlConn = GetSqlConnection())
                {
                    sqlConn.Open();

                    string query = @"
                 SELECT COUNT(DISTINCT ug.UserId)
                FROM [User_Goal] ug
                INNER JOIN [Goal] g ON ug.GoalId = g.Id
                WHERE g.CreatedAt >= DATEADD(DAY, -14, GETDATE())
            ";

                    using (SqlCommand cmd = new SqlCommand(query, sqlConn))
                    {
                        count = (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error in GetActiveUsersCount");
                throw new Exception("A database error occurred while retrieving the active user count.", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetActiveUsersCount");
                throw;
            }

            return count;
        }
        public List<UserEntity> SearchUsers(string query, string gender, string nationality, int? minAge, int? maxAge, DateTime? createdAfter)
        {
            var users = new List<UserEntity>();
            var today = DateTime.Today;

            using (var connection = GetSqlConnection())
            {
                var command = new SqlCommand(@"
            SELECT * FROM [User]
            WHERE (@query IS NULL OR Username LIKE '%' + @query + '%' OR Email LIKE '%' + @query + '%')
              AND (@gender IS NULL OR Gender = @gender)
              AND (@nationality IS NULL OR Nationality = @nationality)
              AND (@minAge IS NULL OR DATEDIFF(YEAR, DateOfBirth, @today) >= @minAge)
              AND (@maxAge IS NULL OR DATEDIFF(YEAR, DateOfBirth, @today) <= @maxAge)
              AND (@createdAfter IS NULL OR CreatedAt >= @createdAfter)
        ", connection);

    
                command.Parameters.AddWithValue("@query", string.IsNullOrWhiteSpace(query) ? DBNull.Value : query);
                command.Parameters.AddWithValue("@gender", string.IsNullOrWhiteSpace(gender) ? DBNull.Value : gender);
                command.Parameters.AddWithValue("@nationality", string.IsNullOrWhiteSpace(nationality) ? DBNull.Value : nationality);
                command.Parameters.AddWithValue("@minAge", minAge.HasValue ? (object)minAge.Value : DBNull.Value);
                command.Parameters.AddWithValue("@maxAge", maxAge.HasValue ? (object)maxAge.Value : DBNull.Value);
                command.Parameters.AddWithValue("@createdAfter", createdAfter.HasValue ? (object)createdAfter.Value : DBNull.Value);
                command.Parameters.AddWithValue("@today", today);

                connection.Open();
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var user = new UserEntity(
                        id: reader.GetInt32(reader.GetOrdinal("UserId")),
                        username: reader.GetString(reader.GetOrdinal("Username")),
                        email: reader.GetString(reader.GetOrdinal("Email")),
                        password: reader.GetString(reader.GetOrdinal("Password")),
                        picture: reader["Picture"] as byte[],
                        salt: reader.GetString(reader.GetOrdinal("Salt")),
                        nationality: reader.GetString(reader.GetOrdinal("Nationality")),
                        dateOfBirth: reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                        gender: reader.GetString(reader.GetOrdinal("Gender")),
                        createdAt: DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("CreatedAt"))),
                        role: reader.GetString(reader.GetOrdinal("Role"))
                    );

                    users.Add(user);
                }
            }

            return users;
        }
        public List<string> GetAllDistinctNationalities()
        {
            var result = new List<string>();

            using var conn = GetSqlConnection();
            var cmd = new SqlCommand("SELECT DISTINCT Nationality FROM [User] ORDER BY Nationality", conn);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add(reader.GetString(0));

            return result;
        }

        public List<string> GetAllDistinctGenders()
        {
            var result = new List<string>();

            using var conn = GetSqlConnection();
            var cmd = new SqlCommand("SELECT DISTINCT Gender FROM [User] ORDER BY Gender", conn);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add(reader.GetString(0));

            return result;
        }

    }
}

