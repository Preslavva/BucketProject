using System.Data;
using System.Reflection;
using BucketProject.Models;
using BucketProject.Repositories;
using BucketProject.ViewModels;
using Microsoft.Data.SqlClient;
namespace BucketProject.Services

{
    public class LogicManager
    {
            private const string connString = "Server=DESKTOP-0DITB5G;Database=BucketProject;Trusted_Connection=True; TrustServerCertificate=True;";

            internal static bool Register(RegisterViewModel user)
            {
                try
                {
                    using SqlConnection connection = new SqlConnection(connString);
                    connection.Open();

                    string checkSql = @"SELECT COUNT(*) FROM [User] WHERE [Username] = @Username or Email = @Email";
                    using SqlCommand checkCommand = new SqlCommand(checkSql, connection);
                    checkCommand.Parameters.AddWithValue("@Username", user.Username);
                    checkCommand.Parameters.AddWithValue("@Email", user.Email);


                    int count = (int)checkCommand.ExecuteScalar();

                    if (count > 0)
                    {
                        throw new ApplicationException("Username or Email is already taken.");
                    }

                    string insertSql = @"INSERT INTO [User] ([Username], Email, [Password]) 
                             VALUES (@Username, @Email, @Password)";
                    using SqlCommand insertCommand = new SqlCommand(insertSql, connection);
                    insertCommand.Parameters.AddWithValue("@Username", user.Username);
                    insertCommand.Parameters.AddWithValue("@Email", user.Email);
                    insertCommand.Parameters.AddWithValue("@Password", user.Password);

                    insertCommand.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Error registering user: " + ex.Message);
                }
            }

            internal static User? ValidateUser(string username, string password)
            {
                try
                {
                    using (SqlConnection sqlConn = new SqlConnection(connString))
                    {
                        sqlConn.Open();
                        string queryValidateUser = @"SELECT UserId, [Username], Email, [Password], Picture
                                         FROM [User]
                                         WHERE Username = @Username AND [Password] = @Password";

                        using (SqlCommand validateUser = new SqlCommand(queryValidateUser, sqlConn))
                        {
                            validateUser.Parameters.Add("@Username", SqlDbType.NVarChar).Value = username;
                            validateUser.Parameters.Add("@Password", SqlDbType.NVarChar).Value = password;



                            using (SqlDataReader reader = validateUser.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                return new User(
                                   (int)reader["UserId"],
                                   reader["Username"].ToString(),
                                   reader["Email"].ToString(),
                                   reader["Password"].ToString(),
                                   reader.IsDBNull(reader.GetOrdinal("Picture"))
                                       ? null
                                       : (byte[])reader["Picture"] 
                                   );


                            };
                            }
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    throw new Exception($"Database error occurred while validating customer: {sqlEx.Message}", sqlEx);
                }
                catch (Exception ex)
                {
                    throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
                }

                return null;
            }

            internal static List<User>? LoadUsersFromDB()
            {
                List<User> users = new List<User>();

                try
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        conn.Open();
                        string queryGetUsers = @"select Id, Username, Password, Email, Photo
                                                from [User]";

                        using (SqlCommand loadUsers = new SqlCommand(queryGetUsers, conn))
                        {
                            SqlDataReader reader = loadUsers.ExecuteReader();

                            while (reader.Read())
                            {
                            users.Add(new User(
                                (int)reader["Id"],
                                    reader["Username"].ToString(),
                                    reader["Email"].ToString(),
                                    reader["Password"].ToString(),
                                    reader.IsDBNull(reader.GetOrdinal("Picture"))
                                   ? null
                                   : (byte[])reader["Picture"]));
                            }
                        }
                        return users;
                    }
                }
                catch (SqlException sqlEx)
                {
                    throw new Exception($"Database error occurred while loading customers: {sqlEx.Message}", sqlEx);
                }
                catch (Exception ex)
                {
                    throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
                }
            }
        }
    }


