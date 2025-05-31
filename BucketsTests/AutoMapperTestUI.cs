using BucketProject.UI.BucketProject.Mapping;
using AutoMapper;
using BucketProject.UI.ViewModels.ViewModels;
using BucketProject.BLL.Business_Logic.DTOs;
using BucketProject.BLL.Business_Logic.Domain;
using BucketProject.DAL.Models.Enums;


namespace BucketsTests
{
    [TestClass]
    public class AutoMapperTestUI
    {
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _config;

        public TestContext TestContext { get; set; }

        public AutoMapperTestUI()
        {
             _config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperUI());
            });
            _mapper = _config.CreateMapper();
        }

        [TestMethod]
        public void AutoMapper_Configuration_Is_Valid()
        {
            try
            {
                _config.AssertConfigurationIsValid();
            }
            catch (Exception ex)
            {
                TestContext.WriteLine("AutoMapper configuration validation failed.");
                TestContext.WriteLine($"Exception: {ex.Message}");
                TestContext.WriteLine($"Stack Trace: {ex.StackTrace}");
                Assert.Fail("AutoMapper configuration is invalid. See test output for details.");
            }
        }


        [TestMethod]
        public void LogInViewModel_ToUser_Maps_Correctly()
        {
            LogInViewModel vm = new LogInViewModel
            {
                Username = "username",
                Password = "password"
            };

            User user = _mapper.Map<User>(vm);

            Assert.AreEqual("username", user.Username);
            Assert.AreEqual("password", user.Password);
        }

        [TestMethod]
        public void User_ToUserViewModel_Maps_Correctly()
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
                CreatedAt = DateOnly.MaxValue,
                Picture = new byte[] { 1, 2, 3 },
                Role = "role"

            };

            UserViewModel vm = _mapper.Map<UserViewModel>(user);

            Assert.AreEqual("username", vm.Username);
            Assert.AreEqual("email", vm.Email);
            Assert.AreEqual(DateOnly.MaxValue, vm.CreatedAt);
            Assert.AreEqual("nationality", vm.Nationality);
            Assert.AreEqual("gender", vm.Gender);
            Assert.AreEqual(dob, vm.DateOfBirth);

        }

        [TestMethod]
        public void Goal_ToGoalViewModel_Maps_Correctly()
        {
            DateTime createdAt = DateTime.Now;
            DateTime completedAt = DateTime.Now;
            DateTime deadline = DateTime.Now;


            Goal goal = new Goal(1, Category.Week, GoalType.Education, createdAt, completedAt, "desc", deadline, true, false, false, 2, 1);

            GoalViewModel vm = _mapper.Map<GoalViewModel>(goal);

            Assert.AreEqual(1, vm.Id);
            Assert.AreEqual("Week", vm.Category);
            Assert.AreEqual("Education", vm.Type);
            Assert.AreEqual("desc", vm.Description);
            Assert.AreEqual(true, vm.IsDone);
            Assert.AreEqual(2, vm.ParentGoalId);
            Assert.AreEqual(1, vm.OwnerId);
        }

        [TestMethod]
        public void GoalViewModel_ToGoal_Maps_Correctly()
        {
            GoalViewModel vm = new GoalViewModel
            {
                Id = 1,
                Category = "Week",
                Type = "Education",
                Description = "desc",
                IsDone = true,
                ParentGoalId = 2,
                OwnerId = 1
            };

            Goal goal = _mapper.Map<Goal>(vm);

            Assert.AreEqual(1, goal.Id);
            Assert.AreEqual(Category.Week, goal.Category);
            Assert.AreEqual(GoalType.Education, goal.Type);
            Assert.AreEqual("desc", goal.Description);
            Assert.AreEqual(true, goal.IsDone);
            Assert.AreEqual(2, goal.ParentGoalId);
            Assert.AreEqual(1, goal.OwnerId);

        }
    }
}
