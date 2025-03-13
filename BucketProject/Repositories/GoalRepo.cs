using System.Reflection;
using BucketProject.Models;
using BucketProject.ViewModels;
using Microsoft.Data.SqlClient;

namespace BucketProject.Repositories
{
    public class GoalRepo
    {
        private const string connString = "Server=DESKTOP-0DITB5G;Database=BucketProject;Trusted_Connection=True; TrustServerCertificate=True;";

        public void CreateGoal(Category category, string description, DateTime deadline, DateTime createdAt, bool isDone, bool isDeleted)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string queryCreateGoal = @"insert into Goal (Category, Description, Deadline, IsDone, IsDeleted, CreatedAt)
                                           values (@Category, @Description, @Deadline, @isDone, @IsDeleted, @CreatedAt)";

                    using (SqlCommand createGoal = new SqlCommand(queryCreateGoal, conn))
                    {
                        createGoal.Parameters.AddWithValue("@Category", category);
                        createGoal.Parameters.AddWithValue("@Description", description);
                        createGoal.Parameters.AddWithValue("@CreatedAt", createdAt);
                        createGoal.Parameters.AddWithValue("@Deadline", deadline);
                        createGoal.Parameters.AddWithValue("@IsDone", false);
                        createGoal.Parameters.AddWithValue("@IsDeleted", false);


                        createGoal.ExecuteNonQuery();

                    }
                }
            }

            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while creating goal: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
            }

        }

        public void AssignGoalToUser(User user, Goal goal)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string queyAssignGoalToUser = @" insert into User_Goal (UserId, GoalId)
                                                     values (@UserId, GoalId)";

                    using (SqlCommand assignGoalToUser = new SqlCommand(queyAssignGoalToUser, conn))
                    {
                        assignGoalToUser.Parameters.AddWithValue("@UserId", user.Id);
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

        public List<Goal> LoadGoalsOfUserbyCategory(User user, Category category)
        {
            List<Goal> goals = new List<Goal>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string queryGetGoals = @"select g.Id, g.Category, g.Description, g.DeadLine, g.IsDone
                                             from Goal as g
                                             inner join User_Goal as ug
                                             on g.Id = ug.GoalId
                                             where g.Category = @Category, ug.UserId = @UserId and IsDeleted = @IsDeleted";

                    using (SqlCommand loadGoals = new SqlCommand(queryGetGoals, conn))
                    {
                        loadGoals.Parameters.AddWithValue("@Category", category);
                        loadGoals.Parameters.AddWithValue("@UserId", user.Id);
                        loadGoals.Parameters.AddWithValue("@IsDeleted", false);


                        SqlDataReader reader = loadGoals.ExecuteReader();

                        while (reader.Read())
                        {
                            goals.Add(new Goal
                            {
                                Id = reader.GetInt32(0),
                                Category = (Category)Enum.Parse(typeof(Category), reader.GetString(1)),
                                Description = reader.GetString(2),
    
                                IsDone = reader.GetBoolean(3),
                                IsDeleted = reader.GetBoolean(4),
                                CreatedAt = reader.GetDateTime(5)
                            });
                        }
                    }
                    return goals;
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
    }
}

   



