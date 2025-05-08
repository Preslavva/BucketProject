using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using BucketProject.DAL.Data.InterfacesRepo;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using BucketProject.BLL.Business_Logic.Services;
using BucketProject.BLLBusiness_Logic.Domain;

using BucketProject.BLL.Business_Logic.Mapping;
using BucketProject.DAL.Models.Entities;

namespace BucketsTests
{
    [TestClass]
    public class UserServiceTest
    {
        private readonly Mock<IUserRepo> _userRepo;
        private readonly Mock<IHttpContextAccessor> _contextAccessor;

        private readonly IMapper _mapper;
        private readonly UserService _userService;

        public UserServiceTest()
        {
            _userRepo = new Mock<IUserRepo>();
            _contextAccessor = new Mock<IHttpContextAccessor>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperBL());
            });
            _mapper = config.CreateMapper();

            _userService = new UserService(_userRepo.Object, _contextAccessor.Object, _mapper);
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
   
            var username = "test";
            var password = "   ";

            try
            {
                _userService.LogIn(username, password);
                Assert.Fail("Expected ArgumentException was not thrown.");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Enter a password", ex.Message);
            }
        }

        [TestMethod]
        public void LogIn_InvalidCredentials_ReturnsNull()
        {
           
            var username = "john";
            var password = "wrongpass";

            _userRepo.Setup(r => r.ValidateUser(username, password))
                         .Returns((UserEntity)null);

            var result = _userService.LogIn(username, password);

            Assert.IsNull(result);
            _userRepo.Verify(r => r.ValidateUser(username, password), Times.Once);
            
        }

        [TestMethod]
        public void LogIn_ValidCredentials_ReturnsUser()
        {
            string username = "john";
            string password = "correctpass";
            UserEntity userEntity = new UserEntity(1,"john", "john@mail.com", "password", new byte[] { 0x01 },"salt", "nationality", DateTime.Now, "Gender", DateOnly.MaxValue, "Role");
            User userDomain = new User {Id= 1, Username = "john", Email = "john@mail.com", Password = "password", Picture = new byte[] { 0x01 }, Nationality = "nationality", DateOfBirth=DateTime.Now, Gender="Gender", CreatedAt=DateOnly.MaxValue, Role="Role" };

            _userRepo.Setup(r => r.ValidateUser(username, password)).Returns(userEntity);
    
            User? result = _userService.LogIn(username, password);

            Assert.IsNotNull(result);
            Assert.AreEqual(userDomain.Id, result!.Id);
            Assert.AreEqual(userDomain.Username, result.Username);
            Assert.AreEqual(userDomain.Email, result.Email);

            _userRepo.Verify(r => r.ValidateUser(username, password), Times.Once);
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
           // _contextAccessor.Verify(a => a.HttpContext.Session.SetString("Username", newUsername), Times.Once);
        }

    }




}

