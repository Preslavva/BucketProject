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

    public void InsertGoalAndAssignToUser(int userId, GoalEntity goal)
    {
        using (SqlConnection conn = GetSqlConnection())
        {
            conn.Open();
            using (SqlTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    string insertGoalQuery = @"
                    INSERT INTO Goal (Category, Description, Type, Deadline, IsDone, IsDeleted, CreatedAt, CompletedAt, IsPostponed, ParentGoalId)
                    OUTPUT INSERTED.Id
                    VALUES (@Category, @Description, @Type, @Deadline, @IsDone, @IsDeleted, @CreatedAt, @CompletedAt, @IsPostponed, @ParentGoalId);";

                    int goalId;

                    using (SqlCommand insertGoalCmd = new SqlCommand(insertGoalQuery, conn, transaction))
                    {
                        insertGoalCmd.Parameters.AddWithValue("@Category", goal.Category.ToString());
                        insertGoalCmd.Parameters.AddWithValue("@Description", goal.Description);
                        insertGoalCmd.Parameters.AddWithValue("@Type", goal.Type.ToString());
                        insertGoalCmd.Parameters.AddWithValue("@Deadline", goal.Deadline ?? (object)DBNull.Value);
                        insertGoalCmd.Parameters.AddWithValue("@IsDone", goal.IsDone);
                        insertGoalCmd.Parameters.AddWithValue("@IsDeleted", goal.IsDeleted);
                        insertGoalCmd.Parameters.AddWithValue("@CreatedAt", goal.CreatedAt);
                        insertGoalCmd.Parameters.AddWithValue("@CompletedAt", goal.CompletedAt ?? (object)DBNull.Value);
                        insertGoalCmd.Parameters.AddWithValue("@IsPostponed", goal.IsPostponed);
                        insertGoalCmd.Parameters.Add("@ParentGoalId", SqlDbType.Int).Value =
    goal.ParentGoalId ?? (object)DBNull.Value;




                        goalId = (int)insertGoalCmd.ExecuteScalar();
                    }

                    string insertUserGoalQuery = "INSERT INTO User_Goal (UserId, GoalId) VALUES (@UserId, @GoalId);";
                    using (SqlCommand insertUserGoalCmd = new SqlCommand(insertUserGoalQuery, conn, transaction))
                    {
                        insertUserGoalCmd.Parameters.AddWithValue("@UserId", userId);
                        insertUserGoalCmd.Parameters.AddWithValue("@GoalId", goalId);
                        insertUserGoalCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                }

            }
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
                                parentGoalId
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

    public List<GoalEntity> LoadGoalsOfUserbyCategory(int userId, Category category)
    {
        List<GoalEntity> goals = new List<GoalEntity>();

        try
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();

                string queryGetGoals = @"SELECT g.Id, g.Category, g.[Description], g.IsDone, g.IsDeleted, g.CreatedAt, g.Deadline, g.Type, g.CompletedAt, g.IsPostponed, g.ParentGoalId
                                     FROM Goal AS g
                                     INNER JOIN User_Goal AS ug ON g.Id = ug.GoalId
                                     WHERE g.Category = @Category AND ug.UserId = @UserId AND g.IsDeleted = @IsDeleted";

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
                                parentGoalId
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
    public List<GoalEntity> LoadGoalsOfUser(int userId)
    {
        List<GoalEntity> goals = new List<GoalEntity>();

        try
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                conn.Open();

                string queryGetGoals = @"SELECT g.Id, g.Category, g.[Description], g.IsDone, g.IsDeleted, g.CreatedAt, g.Deadline, g.Type, g.CompletedAt, g.IsPostponed, g.ParentGoalId
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
                                parentGoalId
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
                               parentGoalId
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

                string queryGetGoals = @"SELECT g.Id, g.Category, g.[Description], g.IsDone, g.IsDeleted, g.CreatedAt, g.Deadline, g.Type, g.CompletedAt, g.IsPostponed, g.ParentGoalId
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
                                parentGoalId
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


}





