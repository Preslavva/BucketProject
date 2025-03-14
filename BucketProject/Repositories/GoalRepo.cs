using System.Reflection;
using BucketProject.Models;
using BucketProject.ViewModels;
using Microsoft.Data.SqlClient;

namespace BucketProject.Repositories
{
    public class GoalRepo
    {
        private readonly string connString;

        public GoalRepo(IConfiguration configuration)
        {
            connString = configuration.GetConnectionString("DefaultConnection");
        }

        //public void CreateGoal(Goal goal)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connString))
        //        {
        //            conn.Open();
        //            string queryCreateGoal = @"insert into Goal (Category, Description, Deadline, IsDone, IsDeleted, CreatedAt)
        //                                   values (@Category, @Description, @Deadline, @IsDone, @IsDeleted, @CreatedAt)";

        //            using (SqlCommand createGoal = new SqlCommand(queryCreateGoal, conn))
        //            {
        //                createGoal.Parameters.AddWithValue("@Category", goal.Category.ToString());
        //                createGoal.Parameters.AddWithValue("@Description", goal.Description);
        //                createGoal.Parameters.AddWithValue("@CreatedAt", goal.CreatedAt);
        //                createGoal.Parameters.AddWithValue("@Deadline", goal.Deadline);
        //                createGoal.Parameters.AddWithValue("@IsDone", false);
        //                createGoal.Parameters.AddWithValue("@IsDeleted", false);


        //                createGoal.ExecuteNonQuery();

        //            }
        //        }
        //    }

        //    catch (SqlException sqlEx)
        //    {
        //        throw new Exception($"Database error occurred while creating goal: {sqlEx.Message}", sqlEx);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
        //    }

        //}
        public void InsertGoalAndAssignToUser(int userId,Goal goal)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction()) 
                {
                    try
                    {
                        string insertGoalQuery = "INSERT INTO Goal (Category, Description, Deadline, IsDone, IsDeleted, CreatedAt) OUTPUT INSERTED.Id " +
                            "VALUES (@Category, @Description, @Deadline, @IsDone, @IsDeleted, @CreatedAt);";
                        int goalId;

                        using (SqlCommand insertGoalCmd = new SqlCommand(insertGoalQuery, conn, transaction))
                        {
                            insertGoalCmd.Parameters.AddWithValue("@Category", goal.Category.ToString());
                            insertGoalCmd.Parameters.AddWithValue("@Description", goal.Description);
                            insertGoalCmd.Parameters.AddWithValue("@CreatedAt", goal.CreatedAt);
                            insertGoalCmd.Parameters.AddWithValue("@Deadline", goal.Deadline);
                            insertGoalCmd.Parameters.AddWithValue("@IsDone", false);
                            insertGoalCmd.Parameters.AddWithValue("@IsDeleted", false);


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

        public void AssignGoalToUser(int userId, Goal goal)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string queyAssignGoalToUser = @" insert into User_Goal (UserId, GoalId)
                                                     values (@UserId, @GoalId)";

                    using (SqlCommand assignGoalToUser = new SqlCommand(queyAssignGoalToUser, conn))
                    {
                        assignGoalToUser.Parameters.AddWithValue("@UserId", userId);
                        assignGoalToUser.Parameters.AddWithValue("@GoalId", goal.Id);

                        assignGoalToUser.ExecuteNonQuery();

                    }
                }
            }

            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while assigning a goal to user: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
            }
        }

        public List<Goal> LoadGoalsOfUserbyCategory(int userId, Category category)
        {
            List<Goal> goals = new List<Goal>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string queryGetGoals = @"SELECT g.Id, g.Category, g.[Description], g.IsDone, g.IsDeleted, g.CreatedAt, g.Deadline
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
                                goals.Add(new Goal
                                {
                                    Id = reader.GetInt32(0),
                                    Category = Enum.Parse<Category>(reader.GetString(1)),
                                    Description = reader.GetString(2),
                                    IsDone = reader.GetBoolean(3),
                                    IsDeleted = reader.GetBoolean(4),
                                    CreatedAt = reader.GetDateTime(5),
                                    Deadline = reader.GetDateTime(6)

                                });
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


        public void ChangeGoalStatus(Goal goal, bool isDone)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string queryChangeStatus = @"update Goal
                                                 set IsDone = @IsDone";

                    using (SqlCommand changeStatus = new SqlCommand(queryChangeStatus, conn))
                    {
                        changeStatus.Parameters.AddWithValue("@IsDone", isDone);

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


        public void UpdateGoalDescription(Goal goal, string description)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string queryChangeStatus = @"update Goal
                                                 set Description = @Description
                                                 where Id = @Id";

                    using (SqlCommand changeStatus = new SqlCommand(queryChangeStatus, conn))
                    {
                        changeStatus.Parameters.AddWithValue("@Description", description);
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

        public void DeleteGoal(Goal goal)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
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

        public void PostponeGoal(Goal goal, DateTime deadline)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string queryPostponeGoal = @"update Goal set Deadline = @Deadline where Id = @Id";

                    using (SqlCommand changeStatus = new SqlCommand(queryPostponeGoal, conn))
                    {
                        changeStatus.Parameters.AddWithValue("@Deadline", deadline);
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
                using (SqlConnection conn = new SqlConnection(connString))
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
    }
}

   



