using System.Data;
using System.Reflection;
using BucketProject.DAL.Data.InterfacesRepo;
using BucketProject.DAL.Models.Entities;
using BucketProject.DAL.Models.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BucketProject.DAL.Data.Repositories;

public class GoalRepo: Repository, IGoalRepo
{

    public GoalRepo(IConfiguration configuration):base(configuration)
    { 
    }

    public int InsertGoal(int ownerUserId, GoalEntity goal)
    {
        const string sql = @"
INSERT INTO dbo.Goal
       (Category, Description, Type, Deadline, IsDone, IsDeleted,
        CreatedAt, CompletedAt, IsPostponed, ParentGoalId, OwnerId)
OUTPUT INSERTED.Id
VALUES (@Category, @Description, @Type, @Deadline, @IsDone, @IsDeleted,
        @CreatedAt, @CompletedAt, @IsPostponed, @ParentGoalId, @OwnerId);";

        using var conn = GetSqlConnection();
        conn.Open();

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Category", goal.Category.ToString());
        cmd.Parameters.AddWithValue("@Description", goal.Description);
        cmd.Parameters.AddWithValue("@Type", goal.Type.ToString());
        cmd.Parameters.AddWithValue("@Deadline", goal.Deadline ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@IsDone", goal.IsDone);
        cmd.Parameters.AddWithValue("@IsDeleted", goal.IsDeleted);
        cmd.Parameters.AddWithValue("@CreatedAt", goal.CreatedAt);
        cmd.Parameters.AddWithValue("@CompletedAt", goal.CompletedAt ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@IsPostponed", goal.IsPostponed);
        cmd.Parameters.AddWithValue("@ParentGoalId", goal.ParentGoalId ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@OwnerId", ownerUserId);

        int newId = (int)cmd.ExecuteScalar();
        goal.Id = newId;                 
        return newId;
    }

    public void AssignUsersToGoal(int goalId, IEnumerable<int> userIds)
    {
        const string sql = @"INSERT INTO dbo.User_Goal (UserId, GoalId)
                         VALUES (@UserId, @GoalId);";

        using var conn = GetSqlConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();
        try
        {
            using var cmd = new SqlCommand(sql, conn, tx);
            cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@GoalId", SqlDbType.Int) { Value = goalId });

            foreach (var userId in userIds.Distinct())
            {
                cmd.Parameters["@UserId"].Value = userId;
                cmd.ExecuteNonQuery();
            }

            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }



    public GoalEntity GetGoalById(int id)
    {

        try
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();

                string queryGetGoal = @"SELECT *
                                     FROM Goal            
                                     WHERE Id = @Id";

                using (SqlCommand getGoal = new SqlCommand(queryGetGoal, conn))
                {
                    getGoal.Parameters.AddWithValue("@Id", id);
                   

                    using (SqlDataReader reader = getGoal.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int goalId = reader.GetInt32(0);
                            Category cat = Enum.Parse<Category>(reader.GetString(1));
                            string description = reader.GetString(2);
                            bool isDone = reader.GetBoolean(3);
                            bool isDeleted = reader.GetBoolean(4);
                            DateTime createdAt = reader.GetDateTime(5);
                            DateTime? deadline = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6);
                            GoalType type = Enum.Parse<GoalType>(reader.GetString(7));
                            DateTime? completedAt = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8);
                            bool isPostponed = reader.GetBoolean(9);
                            int? parentGoalId = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10);
                            int ownerId = reader.GetInt32(11);


                            return new GoalEntity(
                                goalId,
                                cat,
                                type,
                                description,
                                createdAt,
                                deadline,
                                completedAt,
                                isDone,
                                isDeleted,
                                isPostponed,
                                parentGoalId,
                                ownerId
                            );

                    
                        }
                    }
                }
            }
            return null;
        }
        catch (SqlException sqlEx)
        {
            throw new Exception($"Database error occurred while loading goals: {sqlEx.Message}", sqlEx);
        }
        catch (Exception ex)
        {
            throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
        }

    }

    public List<GoalEntity> LoadPersonalGoalsOfUserbyCategory(int userId, Category category)
    {
        List<GoalEntity> goals = new List<GoalEntity>();

        try
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();

                string queryGetGoals = @"SELECT
    g.Id,
    g.Category,
    g.[Description],
    g.IsDone,
    g.IsDeleted,
    g.CreatedAt,
    g.Deadline,
    g.Type,
    g.CompletedAt,
    g.IsPostponed,
    g.ParentGoalId,
    g.OwnerId
FROM dbo.Goal        AS g
JOIN dbo.User_Goal   AS ug   ON ug.GoalId = g.Id         
WHERE ug.UserId  = @UserId                               
  AND g.IsDeleted = 0
  AND g.Category  = @Category
  AND NOT EXISTS (
        SELECT 1
        FROM dbo.User_Goal AS x
        WHERE x.GoalId = g.Id
          AND x.UserId <> @UserId
      );
;";

                using (SqlCommand loadGoals = new SqlCommand(queryGetGoals, conn))
                {
                    loadGoals.Parameters.AddWithValue("@Category", category.ToString()); 
                    loadGoals.Parameters.AddWithValue("@UserId", userId);
                    loadGoals.Parameters.AddWithValue("@IsDeleted", false);

                    using (SqlDataReader reader = loadGoals.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            Category cat = Enum.Parse<Category>(reader.GetString(1));
                            string description = reader.GetString(2);
                            bool isDone = reader.GetBoolean(3);
                            bool isDeleted = reader.GetBoolean(4);
                            DateTime createdAt = reader.GetDateTime(5);
                            DateTime? deadline = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6);
                            GoalType type = Enum.Parse<GoalType>(reader.GetString(7));
                            DateTime? completedAt = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8);
                            bool isPostponed = reader.GetBoolean(9);
                            int? parentGoalId = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10);
                            int ownerId = reader.GetInt32(11);



                            GoalEntity goal = new GoalEntity(
                                id,
                                cat,
                                type,
                                description,
                                createdAt,
                                deadline,
                                completedAt,
                                isDone,
                                isDeleted,
                                isPostponed,
                                parentGoalId,
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
            throw new Exception($"Database error occurred while loading goals: {sqlEx.Message}", sqlEx);
        }
        catch (Exception ex)
        {
            throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
        }

        return goals;
    }
    public List<GoalEntity> LoadSharedGoalsOfUserByCategory(int userId, Category category)
    {
        var goals = new List<GoalEntity>();

        const string queryGetGoals = @"
SELECT 
    g.Id,
    g.Category,
    g.[Description],
    g.IsDone,
    g.IsDeleted,
    g.CreatedAt,
    g.Deadline,
    g.Type,
    g.CompletedAt,
    g.IsPostponed,
    g.ParentGoalId,
    g.OwnerId
FROM dbo.Goal AS g
INNER JOIN dbo.User_Goal AS ug
  ON g.Id      = ug.GoalId
 AND ug.UserId = @UserId 
WHERE 
    g.Category   = @Category
  AND g.IsDeleted = 0
  AND (
      SELECT COUNT(*) 
      FROM dbo.User_Goal AS x
      WHERE x.GoalId = g.Id
  ) > 1                       
;
                    
";

        try
        {
            using var conn = GetSqlConnection();
            conn.Open();
            using var cmd = new SqlCommand(queryGetGoals, conn);
            cmd.Parameters.AddWithValue("@Category", category.ToString());
            cmd.Parameters.AddWithValue("@UserId", userId);

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
            throw new Exception($"Database error occurred while loading shared goals: {sqlEx.Message}", sqlEx);
        }
        catch (Exception ex)
        {
            throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
        }

        return goals;
    }

    public List<GoalEntity> LoadGoalsOfUser(int userId)
    {
        List<GoalEntity> goals = new List<GoalEntity>();

        try
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();

                string queryGetGoals = @"SELECT g.Id, g.Category, g.[Description], g.IsDone, g.IsDeleted, g.CreatedAt, g.Deadline, g.Type, g.CompletedAt, g.IsPostponed, g.ParentGoalId, g.OwnerId
                                     FROM Goal AS g
                                     INNER JOIN User_Goal AS ug ON g.Id = ug.GoalId
                                     WHERE ug.UserId = @UserId AND g.IsDeleted = @IsDeleted";

                using (SqlCommand loadGoals = new SqlCommand(queryGetGoals, conn))
                {
                    loadGoals.Parameters.AddWithValue("@UserId", userId);
                    loadGoals.Parameters.AddWithValue("@IsDeleted", false);

                    using (SqlDataReader reader = loadGoals.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            Category cat = Enum.Parse<Category>(reader.GetString(1));
                            string description = reader.GetString(2);
                            bool isDone = reader.GetBoolean(3);
                            bool isDeleted = reader.GetBoolean(4);
                            DateTime createdAt = reader.GetDateTime(5);
                            DateTime? deadline = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6);
                            GoalType type = Enum.Parse<GoalType>(reader.GetString(7));
                            DateTime? completedAt = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8);
                            bool isPostponed = reader.GetBoolean(9);
                            int? parentGoalId = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10);
                            int ownerId = reader.GetInt32(11);



                            GoalEntity goal = new GoalEntity(
                                id,
                                cat,
                                type,
                                description,
                                createdAt,
                                deadline,
                                completedAt,
                                isDone,
                                isDeleted,
                                isPostponed,
                                parentGoalId,
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
            throw new Exception($"Database error occurred while loading goals: {sqlEx.Message}", sqlEx);
        }
        catch (Exception ex)
        {
            throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
        }

        return goals;
    }

    public List<GoalEntity> LoadChildGoalsOfGoals(int goalId)
    {
        List<GoalEntity> goals = new List<GoalEntity>();

        try
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();

                string queryGetGoal = @"SELECT *
                                     FROM Goal            
                                     WHERE ParentGoalId = @Id";

                using (SqlCommand getGoal = new SqlCommand(queryGetGoal, conn))
                {
                    getGoal.Parameters.AddWithValue("@Id", goalId);


                    using (SqlDataReader reader = getGoal.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            Category cat = Enum.Parse<Category>(reader.GetString(1));
                            string description = reader.GetString(2);
                            bool isDone = reader.GetBoolean(3);
                            bool isDeleted = reader.GetBoolean(4);
                            DateTime createdAt = reader.GetDateTime(5);
                            DateTime? deadline = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6);
                            GoalType type = Enum.Parse<GoalType>(reader.GetString(7));
                            DateTime? completedAt = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8);
                            bool isPostponed = reader.GetBoolean(9);
                            int? parentGoalId = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10);
                            int ownerId = reader.GetInt32(11);


                            GoalEntity goal = new GoalEntity(
                               id,
                               cat,
                               type,
                               description,
                               createdAt,
                               deadline,
                               completedAt,
                               isDone,
                               isDeleted,
                               isPostponed,
                               parentGoalId,
                               ownerId
                           );

                            goals.Add(goal);
                        }
                    }
                    }
                }
            
            return null;
        }
        catch (SqlException sqlEx)
        {
            throw new Exception($"Database error occurred while loading goals: {sqlEx.Message}", sqlEx);
        }
        catch (Exception ex)
        {
            throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
        }
    }
    public void ChangeGoalStatus(GoalEntity goal)
    {
        try
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();
                string queryChangeStatus = @"update Goal
                                                 set IsDone = @IsDone, CompletedAt = @CompletedAt
                                                 where Id = @Id";

                using (SqlCommand changeStatus = new SqlCommand(queryChangeStatus, conn))
                {
                    changeStatus.Parameters.AddWithValue("@IsDone", goal.IsDone);
                    changeStatus.Parameters.AddWithValue("@Id", goal.Id);
                    changeStatus.Parameters.AddWithValue("@CompletedAt", goal.CompletedAt ?? (object)DBNull.Value);



                    changeStatus.ExecuteNonQuery();
                }
            }
        }

        catch (SqlException sqlEx)
        {
            throw new Exception($"Database error occurred while changing goal status: {sqlEx.Message}", sqlEx);
        }
        catch (Exception ex)
        {
            throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
        }
    }


    public void UpdateGoalDescription(GoalEntity goal)
    {
        try
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();
                string queryChangeStatus = @"update Goal
                                                 set Description = @Description
                                                 where Id = @Id";

                using (SqlCommand changeStatus = new SqlCommand(queryChangeStatus, conn))
                {
                    changeStatus.Parameters.AddWithValue("@Description", goal.Description);
                    changeStatus.Parameters.AddWithValue("@Id", goal.Id);


                    changeStatus.ExecuteNonQuery();
                }
            }
        }

        catch (SqlException sqlEx)
        {
            throw new Exception($"Database error occurred while changing goal description: {sqlEx.Message}", sqlEx);
        }
        catch (Exception ex)
        {
            throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
        }
    }

    public void DeleteGoal(GoalEntity goal)
    {
        try
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();
                string queryDeleteGoal = @"update Goal set IsDeleted = @IsDeleted where Id = @Id";

                using (SqlCommand changeStatus = new SqlCommand(queryDeleteGoal, conn))
                {
                    changeStatus.Parameters.AddWithValue("@Id", goal.Id);
                    changeStatus.Parameters.AddWithValue("@IsDeleted", true);


                    changeStatus.ExecuteNonQuery();
                }
            }
        }

        catch (SqlException sqlEx)
        {
            throw new Exception($"Database error occurred while deleting goal: {sqlEx.Message}", sqlEx);
        }
        catch (Exception ex)
        {
            throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
        }
    }

    public void PostponeGoal(GoalEntity goal)
    {
        try
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();
                string queryPostponeGoal = @"update Goal set Deadline = @Deadline,IsPostponed=@IsPostponed where Id = @Id";

                using (SqlCommand changeStatus = new SqlCommand(queryPostponeGoal, conn))
                {
                    changeStatus.Parameters.AddWithValue("@Deadline", goal.Deadline);
                    changeStatus.Parameters.AddWithValue("@IsPostponed", true);
                    changeStatus.Parameters.AddWithValue("@Id", goal.Id);

                    changeStatus.ExecuteNonQuery();
                }
            }
        }

        catch (SqlException sqlEx)
        {
            throw new Exception($"Database error occurred while deleting goal: {sqlEx.Message}", sqlEx);
        }
        catch (Exception ex)
        {
            throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
        }
    }

       public int GetIdOfUser(string username)
        {
        try
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();
                string queryUpdateName = @"select UserId from [User] where Username = @Username";

                using (SqlCommand changeStatus = new SqlCommand(queryUpdateName, conn))
                {
                    changeStatus.Parameters.AddWithValue("@Username", username);

                    int id = (int)changeStatus.ExecuteScalar();
                    return id;
                }
            }
        }

        catch (SqlException sqlEx)
        {
            throw new Exception($"Database error occurred while updatting email: {sqlEx.Message}", sqlEx);
        }
        catch (Exception ex)
        {
            throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
        }
    }

    public List<GoalEntity> LoadExpiredGoalsOfUser(int userId)
    {
        List<GoalEntity> goals = new List<GoalEntity>();

        try
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();

                string queryGetGoals = @"SELECT g.Id, g.Category, g.[Description], g.IsDone, g.IsDeleted, g.CreatedAt, g.Deadline, g.Type, g.CompletedAt, g.IsPostponed, g.ParentGoalId, g.OwnerId
                                     FROM Goal AS g
                                     INNER JOIN User_Goal AS ug ON g.Id = ug.GoalId
                                     WHERE ug.UserId = @UserId AND g.IsDeleted = @IsDeleted AND g.Deadline < CAST(GETDATE() AS DATE)
";

                using (SqlCommand loadGoals = new SqlCommand(queryGetGoals, conn))
                {
                    loadGoals.Parameters.AddWithValue("@UserId", userId);
                    loadGoals.Parameters.AddWithValue("@IsDeleted", false);



                    using (SqlDataReader reader = loadGoals.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            Category cat = Enum.Parse<Category>(reader.GetString(1));
                            string description = reader.GetString(2);
                            bool isDone = reader.GetBoolean(3);
                            bool isDeleted = reader.GetBoolean(4);
                            DateTime createdAt = reader.GetDateTime(5);
                            DateTime? deadline = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6);
                            GoalType type = Enum.Parse<GoalType>(reader.GetString(7));
                            DateTime? completedAt = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8);
                            bool isPostponed = reader.GetBoolean(9);
                            int? parentGoalId = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10);
                            int ownerId = reader.GetInt32(11);



                            GoalEntity goal = new GoalEntity(
                                id,
                                cat,
                                type,
                                description,
                                createdAt,
                                deadline,
                                completedAt,
                                isDone,
                                isDeleted,
                                isPostponed,
                                parentGoalId,
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
            throw new Exception($"Database error occurred while loading goals: {sqlEx.Message}", sqlEx);
        }
        catch (Exception ex)
        {
            throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
        }

        return goals;
    }

    public List<UserEntity> LoadSharedUsersForGoal(int goalId, int ownerId)
    {
        var list = new List<UserEntity>();
        const string sql = @"
SELECT u.UserId, u.Username, u.Picture
FROM dbo.User_Goal ug
JOIN dbo.[User] u
  ON ug.UserId = u.UserId
WHERE ug.GoalId   = @GoalId
  AND ug.UserId  <> @OwnerId;
";
        using var conn = GetSqlConnection();
        conn.Open();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@GoalId", goalId);
        cmd.Parameters.AddWithValue("@OwnerId", ownerId);
        using var rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            list.Add(new UserEntity(
              rdr.GetInt32(rdr.GetOrdinal("UserId")),
              rdr.GetString(rdr.GetOrdinal("Username")),
              rdr.IsDBNull(rdr.GetOrdinal("Picture"))
                ? null
                : (byte[])rdr["Picture"]
            ));
        }
        return list;
    }
    public void AssignUserToGoal(int goalId, int userId)
    {
        const string sql = @"
INSERT INTO dbo.User_Goal (UserId, GoalId)
VALUES (@UserId, @GoalId);";

        using var conn = GetSqlConnection();
        conn.Open();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@UserId", userId);
        cmd.Parameters.AddWithValue("@GoalId", goalId);
        cmd.ExecuteNonQuery();
    }

}





