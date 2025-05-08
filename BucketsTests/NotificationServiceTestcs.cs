
using AutoMapper;
using BucketProject.DAL.Data.InterfacesRepo;
using Microsoft.AspNetCore.Http;
using Moq;
using BucketProject.BLL.Business_Logic.Services;
using BucketProject.BLLBusiness_Logic.Domain;

using BucketProject.BLL.Business_Logic.Mapping;
using BucketProject.BLL.Business_Logic.Domain;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.DAL.Models.Entities;
using BucketProject.DAL.Models.Enums;
using BucketProject.BLL.Business_Logic.Strategies;


namespace BucketsTests
{
    [TestClass]
    public class NotificationServiceTest
    {
        private readonly Mock<IHttpContextAccessor> _contextAccessor;
        private readonly Mock<IGoalRepo> _goalRepo;
        private readonly IMapper _mapper;
        private readonly NotificationService _notificationService;
        //private readonly Mock<IDeadlineStrategy> _deadlineDeterminator;
        //private readonly Mock<INotificationStrategy> _notificationManager;


        public NotificationServiceTest()
        {
            _contextAccessor = new Mock<IHttpContextAccessor>();
            _goalRepo = new Mock<IGoalRepo>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperBL());
            });
            _mapper = config.CreateMapper();

            _notificationService = new NotificationService(_goalRepo.Object, _contextAccessor.Object, _mapper);


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
        public void CheckAndNotify_ValidGoal_TriggersNotificationLogic()
        {
            string username = "testuser";
            SetSession(username);
            int userId = 1;
            DateTime today = new DateTime(2025, 5, 8);
            DateTime createdAt = today.AddDays(-3);
            DateTime expectedDeadline = today.AddDays(1);

            GoalEntity goalEntity = new GoalEntity(1, Category.Week, GoalType.Education, "description" ,createdAt, expectedDeadline, null, false, false, false, null, userId);
            Goal goal = new Goal(1, Category.Week, GoalType.Education, createdAt, expectedDeadline, "description", null, false, false, false, null, userId);


            List<UserEntity> userEntities = new List<UserEntity> {new UserEntity(2, "john", "john@mail.com", "password", new byte[] { 0x01 }, "salt", "nationality", DateTime.Now, "Gender", DateOnly.MaxValue, "Role") };
            List<User> userModels = new List<User> { new User { Id = 2, Username = "john", Email = "john@mail.com", Password = "password", Picture = new byte[] { 0x01 }, Nationality = "nationality", DateOfBirth = DateTime.Now, Gender = "Gender", CreatedAt = DateOnly.MaxValue, Role = "Role" } };
 

           

            _goalRepo.Setup(r => r.GetIdOfUser(username)).Returns(userId);
            _goalRepo.Setup(r => r.LoadGoalsOfUser(userId)).Returns(new List<GoalEntity> { goalEntity });
            _goalRepo.Setup(r => r.LoadSharedUsersForGoal(goal.Id, userId)).Returns(userEntities);


            //var deadlineStrategyMock = new Mock<IDeadlineStrategy>();
            //var notificationStrategyMock = new Mock<INotificationStrategy>();

            //_deadlineDeterminator.Setup(d => d.GetStrategy(goal.Category)).Returns(deadlineStrategyMock.Object);
            //_notificationManager.Setup(n => n.GetStrategy(goal.Category)).Returns(notificationStrategyMock.Object);

            //deadlineStrategyMock.Setup(d => d.GetDeadline(goal.CreatedAt, false)).Returns(expectedDeadline);
            //notificationStrategyMock.Setup(n => n.ShouldNotify(today, expectedDeadline)).Returns(true);


            IDeadlineStrategy deadlineStrategy = new WeekDeadlineStrategy();
            INotificationStrategy notificationStrategy = new WeekNotificationStrategy();

            var result = _notificationService.CheckAndNotify(today);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(goal.Id, result[0].Id);
            Assert.AreEqual(1, result[0].Recipients.Count);
            Assert.AreEqual("friend1", result[0].Recipients[0].Username);
        }



    }
}

