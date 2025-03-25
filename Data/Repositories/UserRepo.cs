using System.Data;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using Azure.Identity;
using BucketProject.Data.InterfacesRepo;
using BucketProject.Data.Models;
using BucketProject.Data.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BucketProject.Business_Logic.InterfacesService;

namespace BucketProject.Data.Repositories

{
    public class UserRepo : IUserRepo
    {
        private readonly string connString;
        private readonly IPasswordHasher _passwordHasher;

        public UserRepo(IConfiguration configuration, IPasswordHasher passwordHasher)
        {
            connString = configuration.GetConnectionString("DefaultConnection");
            _passwordHasher = passwordHasher;
        }

        public bool Register(RegisterViewModel user)
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

                var (hashedPassword, salt) = _passwordHasher.HashPassword(user.Password);

                string insertSql = @"INSERT INTO [User] ([Username], Email, [Password], Salt) 
                             VALUES (@Username, @Email, @Password, @Salt)";
                using SqlCommand insertCommand = new SqlCommand(insertSql, connection);
                insertCommand.Parameters.AddWithValue("@Username", user.Username);
                insertCommand.Parameters.AddWithValue("@Email", user.Email);
                insertCommand.Parameters.AddWithValue("@Password", hashedPassword);
                insertCommand.Parameters.AddWithValue("@Salt", salt);


                insertCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error registering user: " + ex.Message);
            }
        }

        public User? ValidateUser(string username, string password)
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
                                       : (byte[])reader["Picture"]);


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

        public List<User>? LoadUsersFromDB()
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
                                    reader.IsDBNull(reader.GetOrdinal("Photo"))
                                    ? null
                                    : (byte[])reader["Photo"]));
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

        public void AddPhoto(User user, byte[] picture)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string queryAddPicture = @"update [User] set Picture = @Picture where UserId = @UserId";

                    using (SqlCommand changeStatus = new SqlCommand(queryAddPicture, conn))
                    {
                        changeStatus.Parameters.AddWithValue("@Picture", SqlDbType.VarBinary).Value = picture;
                        changeStatus.Parameters.AddWithValue("@UserId", user.Id);

                        changeStatus.ExecuteNonQuery();
                    }
                }
            }

            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while adding photo: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
            }
        }

        public void UpdateName(User user, string username)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string queryUpdateName = @"update [User] set Username = @Username where UserId = @UserId";

                    using (SqlCommand changeStatus = new SqlCommand(queryUpdateName, conn))
                    {
                        changeStatus.Parameters.AddWithValue("@Username", username);
                        changeStatus.Parameters.AddWithValue("@UserId", user.Id);

                        changeStatus.ExecuteNonQuery();
                    }
                }
            }

            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while updatting name: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
            }
        }
        public void UpdateEmail(User user, string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string queryUpdateName = @"update [User] set Email = @Email where UserId = @UserId";

                    using (SqlCommand changeStatus = new SqlCommand(queryUpdateName, conn))
                    {
                        changeStatus.Parameters.AddWithValue("@Email", email);
                        changeStatus.Parameters.AddWithValue("@UserId", user.Id);

                        changeStatus.ExecuteNonQuery();
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
        public int GetIdOfUserU(string username)
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


        public User? GetUserByUsername(string username)
        {
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connString))
                {
                    sqlConn.Open();
                    string queryValidateUser = @"SELECT UserId, [Username], Email, [Password], Picture
                                         FROM [User]
                                         WHERE Username = @Username";

                    using (SqlCommand validateUser = new SqlCommand(queryValidateUser, sqlConn))
                    {
                        validateUser.Parameters.Add("@Username", SqlDbType.NVarChar).Value = username;

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
                                       : (byte[])reader["Picture"]);


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
    }
}
