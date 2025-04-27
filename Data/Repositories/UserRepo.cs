using System.Data;
using System.Reflection;
using BucketProject.DAL.Data.InterfacesRepo;
using BucketProject.DAL.Models.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BucketProject.DAL.Data.Repositories

{
    public class UserRepo : Repository, IUserRepo
    {
        private readonly IPasswordHasher _passwordHasher;

        public UserRepo(IConfiguration configuration, IPasswordHasher passwordHasher) : base(configuration)
        {
            _passwordHasher = passwordHasher;
        }

        public bool Register(UserEntity user)
        {
               using SqlConnection connection = GetSqlConnection();
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

                string insertSql = @"INSERT INTO [User] ([Username], Email, [Password], Salt, Nationality, DateOfBirth, Gender, CreatedAt, Role) 
                             VALUES (@Username, @Email, @Password, @Salt, @Nationality, @DateOfBirth, @Gender, @CreatedAt, @Role)";
                using SqlCommand insertCommand = new SqlCommand(insertSql, connection);
                insertCommand.Parameters.AddWithValue("@Username", user.Username);
                insertCommand.Parameters.AddWithValue("@Email", user.Email);
                insertCommand.Parameters.AddWithValue("@Password", hashedPassword);
                insertCommand.Parameters.AddWithValue("@Salt", salt);
                insertCommand.Parameters.AddWithValue("@Nationality", user.Nationality);
                insertCommand.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
                insertCommand.Parameters.AddWithValue("@Gender", user.Gender);
                insertCommand.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
                insertCommand.Parameters.AddWithValue("@Role", "User");



                insertCommand.ExecuteNonQuery();
                return true;
            
        }

        public UserEntity? ValidateUser(string username, string password)
        {
            try
            {
                using (SqlConnection sqlConn = GetSqlConnection())
                {
                    sqlConn.Open();
                    string query = @"SELECT UserId, [Username], Email, [Password], Picture, Salt, Nationality, DateofBirth, Gender, CreatedAt, Role
                             FROM [User]
                             WHERE Username = @Username";

                    using (SqlCommand cmd = new SqlCommand(query, sqlConn))
                    {
                        cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = username;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader["Password"].ToString();
                                string storedSalt = reader["Salt"].ToString();

                                bool isValid = _passwordHasher.VerifyPassword(password, storedHash, storedSalt);

                                if (isValid)
                                {
                                    return new UserEntity(
                                      (int)reader["UserId"],
                                      reader["Username"].ToString(),
                                      reader["Email"].ToString(),
                                      storedHash,
                                      reader.IsDBNull(reader.GetOrdinal("Picture")) ? null : (byte[])reader["Picture"],
                                      storedSalt,
                                      reader["Nationality"].ToString(),
                                      (DateTime)reader["DateOfBirth"],
                                      reader["Gender"].ToString(),
                                      DateOnly.FromDateTime((DateTime)reader["CreatedAt"]),
                                      reader["Role"].ToString()

);
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while validating user: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred : {ex.Message}", ex);
            }

            return null;
        }



        public void AddPhoto(UserEntity user, byte[] picture)
        {
            try
            {
                using (SqlConnection conn = GetSqlConnection())
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
                throw new Exception($"An unexpected error occurred in: {ex.Message}", ex);
            }
        }

        public void UpdateName(UserEntity user, string username)
        {
            try
            {
                using (SqlConnection conn = GetSqlConnection())
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
                throw new Exception($"An unexpected error occurred in: {ex.Message}", ex);
            }
        }
        public void UpdateEmail(UserEntity user, string email)
        {
            try
            {
                using (SqlConnection conn = GetSqlConnection())
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
                throw new Exception($"An unexpected error occurred : {ex.Message}", ex);
            }
        }
        public int GetIdOfUserU(string username)
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
                throw new Exception($"An unexpected error occurred: {ex.Message}", ex);
            }
        }


        public UserEntity? GetUserByUsername(string username)
        {
            try
            {
                using (SqlConnection sqlConn = GetSqlConnection())
                {
                    sqlConn.Open();

                    string queryValidateUser = @"
                SELECT UserId, [Username], Email, [Password], Picture, Salt, Nationality, DateOfBirth, Gender, CreatedAt, Role
                FROM [User]
                WHERE Username = @Username";

                    using (SqlCommand validateUser = new SqlCommand(queryValidateUser, sqlConn))
                    {
                        validateUser.Parameters.Add("@Username", SqlDbType.NVarChar).Value = username;

                        using (SqlDataReader reader = validateUser.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new UserEntity(
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
);

                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while validating user: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred: {ex.Message}", ex);
        }
            return null;
        }
    }
}