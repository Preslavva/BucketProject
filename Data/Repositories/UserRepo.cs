using System.Data;
using System.Reflection;
using BucketProject.DAL.Data.InterfacesRepo;
using BucketProject.DAL.Models.Entities;
using Data.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BucketProject.DAL.Data.Repositories

{
    public class UserRepo : Repository, IUserRepo
    {
        private readonly ILogger<UserRepo> _logger;


        public UserRepo(IConfiguration configuration, ILogger<UserRepo>logger) : base(configuration)
        {
            _logger = logger;
        }

        public bool Register(UserEntity user)
        {
            try
            {
                using SqlConnection connection = GetSqlConnection();
                connection.Open();

                string checkSql = @"SELECT COUNT(*) FROM [User] WHERE [Username] = @Username OR Email = @Email";
                using SqlCommand checkCommand = new SqlCommand(checkSql, connection);
                checkCommand.Parameters.AddWithValue("@Username", user.Username);
                checkCommand.Parameters.AddWithValue("@Email", user.Email);

                int count = (int)checkCommand.ExecuteScalar();
                if (count > 0)
                {
                    throw new DuplicateUserException("Username or Email is already taken.");
                }

                string insertSql = @"
            INSERT INTO [User] 
                ([Username], Email, [Password], Salt, Nationality, DateOfBirth, Gender, CreatedAt, Role) 
            VALUES 
                (@Username, @Email, @Password, @Salt, @Nationality, @DateOfBirth, @Gender, @CreatedAt, @Role)";

                using SqlCommand insertCommand = new SqlCommand(insertSql, connection);
                insertCommand.Parameters.AddWithValue("@Username", user.Username);
                insertCommand.Parameters.AddWithValue("@Email", user.Email);
                insertCommand.Parameters.AddWithValue("@Password", user.Password);
                insertCommand.Parameters.AddWithValue("@Salt", user.Salt);
                insertCommand.Parameters.AddWithValue("@Nationality", user.Nationality);
                insertCommand.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
                insertCommand.Parameters.AddWithValue("@Gender", user.Gender);
                insertCommand.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
                insertCommand.Parameters.AddWithValue("@Role", "User");

                insertCommand.ExecuteNonQuery();
                return true;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error in Register. Username: {Username}, Email: {Email}", user.Username, user.Email);
                throw new Exception("A database error occurred during registration.", sqlEx);
            }
            catch (ApplicationException appEx)
            {
                _logger.LogWarning(appEx, "Validation error in Register. Username: {Username}, Email: {Email}", user.Username, user.Email);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Register. Username: {Username}, Email: {Email}", user.Username, user.Email);
                throw;
            }
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
                _logger.LogError(sqlEx,
                    "SQL error in AddPhoto (UserId={user.Id}, Picture={picture})",
                    user.Id, picture);

                throw new Exception("A database error occurred while adding a photo.", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                   "SQL error in AddPhoto (UserId={user.Id}, Picture={picture})",
                   user.Id, picture);

                throw;
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
                _logger.LogError(sqlEx,
                    "SQL error in UpdateName (UserId={user.Id}, Username={username})",
                    user.Id, username);

                throw new Exception("A database error occurred while updating username.", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                   "SQL error in UpdateName (UserId={user.Id}, Username={username})",
                   user.Id, username);

                throw;
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
                _logger.LogError(sqlEx,
                    "SQL error in GetUserByUsername (Username={username})",
                     username);

                throw new Exception("A database error occurred while getting user by username.", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "SQL error in GetUserByUsername (Username={username})",
                     username);

                throw;
            }
            return null;
        }
    }
}