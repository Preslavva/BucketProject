using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BucketProject.BLL.Business_Logic.Mapping;
using BucketProject.BLLBusiness_Logic.Domain;
using BucketProject.BLL.Business_Logic.Domain;
using BucketProject.DAL.Models.Entities;
using BucketProject.DAL.Models.Enums;


namespace BucketsTests
{
    [TestClass]
    public class AutoMapperBLTests
    {
        private readonly IMapper _mapper;

        public AutoMapperBLTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperBL());
            });
            _mapper = config.CreateMapper();
    
            //config.AssertConfigurationIsValid();

            _mapper = config.CreateMapper();
        }

        [TestMethod]
        public void User_ToUserEntity_Maps_Correctly()
        {
            DateTime dob = DateTime.Parse("1990-01-01T12:34:56Z");
            User user = new User
            {
                Id = 1,
                Username = "username",
                Email = "email",
                Password = "password",
                Salt = "salt",
                DateOfBirth = dob,
                Nationality = "nationality",
                Gender = "gender",
                CreatedAt= DateOnly.MaxValue,
                Picture = new byte[] {1,2,3 },
                Role = "role"

            };

            UserEntity entity = _mapper.Map<UserEntity>(user);
            Assert.AreEqual(1, entity.Id);
            Assert.AreEqual("username", entity.Username);
            Assert.AreEqual("email", entity.Email);
            Assert.AreEqual("password", entity.Password);
            Assert.AreEqual("salt", entity.Salt);
            Assert.AreEqual(dob, entity.DateOfBirth);
            Assert.AreEqual("nationality", entity.Nationality);
            Assert.AreEqual("gender", entity.Gender);
            Assert.AreEqual(DateOnly.MaxValue, entity.CreatedAt);
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, entity.Picture); ;
            Assert.AreEqual("role", entity.Role);


        }
        

        [TestMethod]
        public void UserEntity_ToUser_Maps_Correctly()
        {
            DateTime dob = DateTime.Parse("1990-01-01T12:34:56Z");

            UserEntity entity = new UserEntity(1, "username", "email", "password", new byte[] { 1, 2, 3 },"salt", "nationality", dob, "gender", DateOnly.MaxValue, "role");

            User user = _mapper.Map<User>(entity);

            Assert.AreEqual(1, user.Id);
            Assert.AreEqual("username", user.Username);
            Assert.AreEqual("email", user.Email);
            Assert.AreEqual("password", user.Password);
            Assert.AreEqual("salt", user.Salt);
            Assert.AreEqual(dob, user.DateOfBirth);
            Assert.AreEqual("nationality", user.Nationality);
            Assert.AreEqual("gender", user.Gender);
            Assert.AreEqual(DateOnly.MaxValue, user.CreatedAt);
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, user.Picture); ;
            Assert.AreEqual("role", user.Role);

        }

        [TestMethod]
        public void Goal_ToGoalEntity_Maps_Correctly()
        {
            DateTime createdAt = DateTime.Now;
            DateTime completedAt = DateTime.Now;
            DateTime deadline = DateTime.Now;


            Goal goal = new Goal(1, Category.Week, GoalType.Education, createdAt, completedAt, "desc", deadline, true, false, false, 2, 1);

            GoalEntity entity = _mapper.Map<GoalEntity>(goal);

            Assert.AreEqual(1, entity.Id);
            Assert.AreEqual(Category.Week, entity.Category);
            Assert.AreEqual(GoalType.Education, entity.Type);
            Assert.AreEqual(createdAt, entity.CreatedAt);
            Assert.AreEqual(completedAt, entity.CompletedAt);
            Assert.AreEqual("desc", entity.Description);
            Assert.AreEqual(deadline, entity.Deadline);
            Assert.AreEqual(true, entity.IsDone);
            Assert.AreEqual(false, entity.IsDeleted);
            Assert.AreEqual(false, entity.IsPostponed);
            Assert.AreEqual(2, entity.ParentGoalId);
            Assert.AreEqual(1, entity.OwnerId);

        }

        [TestMethod]
        public void GoalEntity_ToGoal_Maps_Correctly()
        {
            DateTime createdAt = DateTime.Now;
            DateTime completedAt = DateTime.Now;
            DateTime deadline = DateTime.Now;


            GoalEntity entity = new GoalEntity(1, Category.Week, GoalType.Education, "desc",createdAt, deadline, completedAt, true, false, false, 2, 1);

            Goal goal = _mapper.Map<Goal>(entity);

            Assert.AreEqual(1, goal.Id);
            Assert.AreEqual(Category.Week, goal.Category);
            Assert.AreEqual(GoalType.Education, goal.Type);
            Assert.AreEqual(createdAt, goal.CreatedAt);
            Assert.AreEqual(completedAt, goal.CompletedAt);
            Assert.AreEqual("desc", goal.Description);
            Assert.AreEqual(deadline, goal.Deadline);
            Assert.AreEqual(true, goal.IsDone);
            Assert.AreEqual(false, goal.IsDeleted);
            Assert.AreEqual(false, goal.IsPostponed);
            Assert.AreEqual(2, goal.ParentGoalId);
            Assert.AreEqual(1, goal.OwnerId);

        }
    }
}
