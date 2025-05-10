using System.Text;
using Moq;
using BucketProject.DAL.Data.InterfacesRepo;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using BucketProject.BLL.Business_Logic.Services;
using BucketProject.BLLBusiness_Logic.Domain;
using BucketProject.BLL.Business_Logic.Mapping;
using BucketProject.DAL.Models.Entities;
using BucketProject.BLL.Business_Logic.InterfacesService;


namespace BucketsTests
{
    [TestClass]
    public class UserServiceTest
    {
        private readonly Mock<IUserRepo> _userRepo;
        private readonly Mock<IHttpContextAccessor> _contextAccessor;
        private readonly Mock<IPasswordHasher> _hasher;

        private readonly IMapper _mapper;
        private readonly UserService _userService;

        public UserServiceTest()
        {
            _userRepo = new Mock<IUserRepo>();
            _contextAccessor = new Mock<IHttpContextAccessor>();
            _hasher = new Mock<IPasswordHasher>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperBL());
            });
            _mapper = config.CreateMapper();

            _userService = new UserService(_userRepo.Object, _contextAccessor.Object, _mapper,_hasher.Object);
        }

        private void SetSession(string username)
        {
            byte[] usernameBytes = System.Text.Encoding.UTF8.GetBytes(username);
            Mock<ISession> session = new Mock<ISession>();
            session.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()));
            session.Setup(s => s.TryGetValue("Username", out usernameBytes)).Returns(true);

            _contextAccessor.Setup(a => a.HttpContext).Returns(new DefaultHttpContext() { Session = session.Object });
        }

        [TestMethod]
        public void LogIn_EmptyPassword_ThrowsArgumentException()
        {
            string username = "test";
            string password = "   ";

            var ex = Assert.ThrowsException<ArgumentException>(
                () => _userService.LogIn(username, password));
            Assert.AreEqual("password", ex.ParamName);
            StringAssert.StartsWith(ex.Message, "Enter a password");
        }

        [TestMethod]
        public void LogIn_WrongPassword_ReturnsNull()
        {
       
            string username = "john";
            string password = "wrongpass";
            UserEntity userEntity = new UserEntity(1, "john", "john@mail.com", "password", new byte[] { 0x01 }, "salt", "nationality", DateTime.Now, "Gender", DateOnly.MaxValue, "Role");
            _userRepo.Setup(r => r.GetUserByUsername(username)).Returns(userEntity);
            _hasher
              .Setup(h => h.VerifyPassword(password, userEntity.Password, userEntity.Salt))
              .Returns(false);

          
            User? result = _userService.LogIn(username, password);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void LogIn_WrongUsernameAndPassword_ReturnsNull()
        {

            string username = "wrongusername";
            string password = "wrongpass";
            UserEntity userEntity = new UserEntity(1, "john", "john@mail.com", "password", new byte[] { 0x01 }, "salt", "nationality", DateTime.Now, "Gender", DateOnly.MaxValue, "Role");
            _userRepo.Setup(r => r.GetUserByUsername(username)).Returns(userEntity);
            _hasher
              .Setup(h => h.VerifyPassword(password, userEntity.Password, userEntity.Salt))
              .Returns(false);


            User? result = _userService.LogIn(username, password);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void LogIn_ValidCredentials_ReturnsUser()
        {
            string username = "john";
            string password = "correctpass";
            string hashedPassword = "hashedPassword";
            string salt = "salt";
            UserEntity userEntity = new UserEntity(1, username, "john@mail.com", hashedPassword, new byte[] { 0x01 }, salt, "nationality", DateTime.Now, "Gender", DateOnly.MaxValue, "Role");
            User userDomain = new User { Id = 1, Username = username, Email = "john@mail.com", Password = password, Picture = new byte[] { 0x01 }, Nationality = "nationality", DateOfBirth = DateTime.Now, Gender = "Gender", CreatedAt = DateOnly.MaxValue, Role = "Role" };

            _userRepo.Setup(r => r.GetUserByUsername(username)).Returns(userEntity);
            _hasher
            .Setup(h => h.VerifyPassword(password, userEntity.Password, userEntity.Salt))
            .Returns(true);
            User? result = _userService.LogIn(username, password);

            Assert.IsNotNull(result);
            Assert.AreEqual(userDomain.Id, result!.Id);
            Assert.AreEqual(userDomain.Username, result.Username);
            Assert.AreEqual(userDomain.Email, result.Email);

            _userRepo.Verify(r => r.GetUserByUsername(username), Times.Once);
            _hasher.Verify(h => h.VerifyPassword(password, hashedPassword, salt), Times.Once);
        }

        [TestMethod]
        public void UpdateUsername_ValidUsername_UpdatesSessionAndRepository()
        {
            string oldUsername = "oldUser";
            string newUsername = "newUser";

            UserEntity userEntity = new UserEntity(1, oldUsername, "john@mail.com", "password", new byte[] { 0x01 }, "salt", "nationality", DateTime.Now, "Gender", DateOnly.MaxValue, "Role");


            SetSession(oldUsername);

            _userRepo.Setup(r => r.GetUserByUsername(oldUsername)).Returns(userEntity);

            _userService.UpdateUsername(newUsername);

            SetSession(newUsername);


            _userRepo.Verify(r => r.GetUserByUsername(oldUsername), Times.Once);
            _userRepo.Verify(r => r.UpdateName(userEntity, newUsername), Times.Once);

        }

        [TestMethod]
        public void UpdateUsername_EmptyNewUsername_ThrowsArgumentException()
        {
            string oldUsername = "oldUsername";
            string newUsername = "  ";
            SetSession(oldUsername);

            UserEntity userEntity = new UserEntity(1, oldUsername, "john@mail.com", "password", new byte[] { 0x01 }, "salt", "nationality", DateTime.Now, "Gender", DateOnly.MaxValue, "Role");

            _userRepo.Setup(r => r.GetUserByUsername(oldUsername)).Returns(userEntity);

            try
            {
                _userService.UpdateUsername(newUsername);
                Assert.Fail("Expected ArgumentException for empty newUsername.");
            }
            catch (ArgumentException ex)
            {

                StringAssert.Contains(ex.Message, "newUsername");

            }
            _userRepo.Verify(r=>r.UpdateName(It.IsAny<UserEntity>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateProfilePicture_NullFile_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
                await _userService.UpdateProfilePicture(null)
            );
        }

        [TestMethod]
        public async Task UpdateProfilePicture_ZeroLength_ThrowsException()
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(0L);

            await Assert.ThrowsExceptionAsync<Exception>(async () =>
                await _userService.UpdateProfilePicture(mockFile.Object)
            );
        }

        [TestMethod]
        public async Task UpdateProfilePicture_UserNotFound_AddPhotoCalledWithNullUser()
        {
            string username = "username";
            SetSession(username);

            byte[] bytes = { 9, 8, 7 };
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(bytes.Length);
            mockFile
                .Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns<Stream, CancellationToken>((s, _) =>
                    s.WriteAsync(bytes, 0, bytes.Length)
                );

            _userRepo.Setup(r => r.GetUserByUsername(username)).Returns((UserEntity?)null);

            await _userService.UpdateProfilePicture(mockFile.Object);

            _userRepo.Verify(r =>
                r.AddPhoto(
                    It.Is<UserEntity?>(u => u == null),
                    It.Is<byte[]>(b => b.SequenceEqual(bytes))
                ),
                Times.Once
            );
        }
        [TestMethod]
        public async Task UpdateProfilePicture_ValidFile_AddsPhotoSuccessfully()
        {
            string username = "username";
            SetSession(username);
            var userEntity = new UserEntity(1,username);
            _userRepo.Setup(r => r.GetUserByUsername(username))
                     .Returns(userEntity);

            byte[] imageBytes = Encoding.UTF8.GetBytes("test-image-bytes");
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(imageBytes.Length);
            mockFile
                .Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns<Stream, CancellationToken>((stream, _) =>
                    stream.WriteAsync(imageBytes, 0, imageBytes.Length)
                );

            await _userService.UpdateProfilePicture(mockFile.Object);


            _userRepo.Verify(r =>
                r.AddPhoto(
                    It.Is<UserEntity?>(u => u == userEntity),
                    It.Is<byte[]>(b => b.Length == imageBytes.Length
                                       && b.SequenceEqual(imageBytes))
                ),
                Times.Once
            );
        }

    }

}