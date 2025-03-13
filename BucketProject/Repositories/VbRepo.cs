using BucketProject.Models;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace BucketProject.Repositories
{
    public class VbRepo
    {
        private const string connString = "Server=DESKTOP-0DITB5G;Database=BucketProject;Trusted_Connection=True; TrustServerCertificate=True;";

        public void CreateVB(string name, User owner, DateTime createdAt)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string queryCreateVB = @"insert into Vision_Board (Name, OwnerId, Created_at)
                                           values (@Name, @OwnerId, @Created_at)";

                    using (SqlCommand createVB = new SqlCommand(queryCreateVB, conn))
                    {
                        createVB.Parameters.AddWithValue("@Name", name);
                        createVB.Parameters.AddWithValue("@OwnerId", owner.Id);

                        createVB.ExecuteNonQuery();
                    }
                }
            }

            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while creating vision board: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
            }
        }

        public void DeleteVisionBoard(VisionBoard visionBoard)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string queryDeleteVB = @"alter Vision_Board set IsDeleted = @IsDeleted where VisionBoardId = @VisionBoardId" ;

                    using (SqlCommand deleteVB = new SqlCommand(queryDeleteVB, conn))
                    {
                        deleteVB.Parameters.AddWithValue("@Id", visionBoard.Id);
                        deleteVB.Parameters.AddWithValue("@IsDeleted", true);


                        deleteVB.ExecuteNonQuery();
                    }
                }
            }

            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while deleting vision board: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
            }
        }

        public void AssignVBToUser(VisionBoard visionBoard, User user)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string queryAssignVBToUser = @"insert into User_Vision_Board (UserId, VisionBoardId)
                                                   values (@userId, @VisionBoardId)";

                    using (SqlCommand assignVBToUser = new SqlCommand(queryAssignVBToUser, conn))
                    {
                        assignVBToUser.Parameters.AddWithValue("@UserId", user.Id);
                        assignVBToUser.Parameters.AddWithValue("@VisionBoardId", visionBoard.Id);


                        assignVBToUser.ExecuteNonQuery();
                    }
                }
            }

            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while assigning vision board to user: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
            }
        }

        public List<VisionBoard> LoadVBsOfUser(User user)
        {
            List<VisionBoard> visionBoards = new List<VisionBoard>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string queryGetVBs = @"select vb.Id, vb.Name, vb.OwnerId, vb.Created_at
                                        from Vision_Board as vb
                                        inner join User_Vision_Board as uvb
                                        on vb.Id = uvb.VisionBoardId
                                        where uvb.UserId = @UserId and vb.IsDeleted = @IsDeleted";

                    using (SqlCommand getVBs = new SqlCommand(queryGetVBs, conn))
                    {

                        getVBs.Parameters.AddWithValue("@UserId", user.Id);
                        getVBs.Parameters.AddWithValue("@IsDeleted", false);


                        SqlDataReader reader = getVBs.ExecuteReader();

                        while (reader.Read())
                        {
                            visionBoards.Add(new VisionBoard(
                                (int)reader["Id"],
                                    reader["Name"].ToString(),
                                    (User)reader["OwnerId"],
                                    (DateTime)reader["Created_at"],
                                    (bool)reader["IsDeleted"]));
                                    
                        }
                    }
                    return visionBoards;
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while loading vision boards: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
            }
        }

        public List<byte[]> LoadPicturesFromVBNotDark(VisionBoard visionBoard)
        {

            List<byte[]> pictures = new List<byte[]>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string queryGetPictures = @"select Image
                                                from Vision_Board_Items
                                                where VisionBoardId = @VisionBoardId and isDarkened = @isDarkened and IsDeleted = @IsDeleted";

                    using (SqlCommand getVBs = new SqlCommand(queryGetPictures, conn))
                    {
                        getVBs.Parameters.AddWithValue("@VisionBoardId", visionBoard.Id);
                        getVBs.Parameters.AddWithValue("@isDarkened", false);
                        getVBs.Parameters.AddWithValue("@IsDeleted", false);



                        SqlDataReader reader = getVBs.ExecuteReader();

                        while (reader.Read())
                        {
                            pictures.Add(
                            reader.IsDBNull(reader.GetOrdinal("Image")) ? null : (byte[])reader["Image"]);
                        }
                    }
                    return pictures;
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while loading pictures: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
            }
        }
    

    public List<byte[]> LoadPicturesFromVBdark(VisionBoard visionBoard)
        {

            List<byte[]> pictures = new List<byte[]>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string queryGetPictures = @"select Image
                                                from Vision_Board_Items
                                                 where VisionBoardId = @VisionBoardId and isDarkened = @isDarkened and IsDeleted = @IsDeleted";

                    using (SqlCommand getVBs = new SqlCommand(queryGetPictures, conn))
                    {
                        getVBs.Parameters.AddWithValue("@VisionBoardId", visionBoard.Id);
                        getVBs.Parameters.AddWithValue("@isDarkened", true);
                        getVBs.Parameters.AddWithValue("@IsDeleted", false);

                        SqlDataReader reader = getVBs.ExecuteReader();

                        while (reader.Read())
                        {
                            pictures.Add(
                            reader.IsDBNull(reader.GetOrdinal("Image")) ? null : (byte[])reader["Image"]);
                        }
                    }
                    return pictures;
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while loading pictures: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
            }
        }

        public void AddImageInVB(byte[] image, VisionBoard visionBoard)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string queryAddImage = @"insert into Vision_Board (Image)
                                           values (@Imamge)
                                           where Id = @Id";

                    using (SqlCommand addImage = new SqlCommand(queryAddImage, conn))
                    {
                        addImage.Parameters.AddWithValue("@Image", image);
                        addImage.Parameters.AddWithValue("@Id", visionBoard.Id);

                        addImage.ExecuteNonQuery();
                    }
                }
            }

            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while adding an image to vision board: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
            }
        }

        public void ChangeImageStatus(byte[] image, bool isDarkened)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string queryChangeStatus = @"update Vision_Board_Items
                                                 set isDarkened = @isDarkened
                                                 where Image = @Image
                                                 ";

                    using (SqlCommand addImage = new SqlCommand(queryChangeStatus, conn))
                    {
                        addImage.Parameters.AddWithValue("@Image", image);
                        addImage.Parameters.AddWithValue("@ssDarkened", isDarkened);

                        addImage.ExecuteNonQuery();
                    }
                }
            }

            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while changing status of image: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
            }
        }

        public void DeleteImage(byte[] image)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string queryChangeStatus = @"update Vision_Board_Items
                                                 set IsDeleted = @IsDeleted
                                                 where Image = @Image
                                                 ";

                    using (SqlCommand addImage = new SqlCommand(queryChangeStatus, conn))
                    {
                        addImage.Parameters.AddWithValue("@Image", image);
                        addImage.Parameters.AddWithValue("@IsDeleted", true);

                        addImage.ExecuteNonQuery();
                    }
                }
            }

            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while deleting image: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
            }
        }

        public void UpdateVBName(VisionBoard visionBoard, string name)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string queryChangeStatus = @"update Vision_Board
                                                 set Name = @Name
                                                 where Id = @Id
                                                 ";

                    using (SqlCommand addImage = new SqlCommand(queryChangeStatus, conn))
                    {
                        addImage.Parameters.AddWithValue("@Name", name);
                        addImage.Parameters.AddWithValue("@Id", visionBoard.Id);

                        addImage.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Database error occurred while updating vision board name: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred in {MethodBase.GetCurrentMethod().Name}: {ex.Message}", ex);
            }
        }
    }
}
