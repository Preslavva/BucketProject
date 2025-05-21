using AutoMapper;
using BucketProject.BLL.Business_Logic.Services;
using BucketProject.DAL.Data.InterfacesRepo;
using BucketProjetc.BLL.Business_Logic.InterfacesService;
using Microsoft.AspNetCore.Http;
using Moq;
using BucketProject.BLL.Business_Logic.Mapping;
using BucketProject.BLL.Business_Logic.Domain;
using BucketProject.DAL.Models.Entities;
using BucketProject.DAL.Models.Enums;
using System.ComponentModel.DataAnnotations;
using BucketProject.BLL.Business_Logic.InterfacesService;
using Exceptions.Exceptions;


namespace BucketsTests
{
    [TestClass]
    public class GoalServiceTest
    {
        private readonly Mock<IGoalRepo> _goalRepo;
        private readonly Mock<IGoalInviteRepo> _inviteRepo;
        private readonly Mock<IHttpContextAccessor> _contextAccessor;
        private readonly Mock<IAIClient> _aIClient;
        private readonly Mock<IUserService> _userService;


        private readonly IMapper _mapper;
        private readonly GoalService _goalService;

        public GoalServiceTest()
        {
            _goalRepo = new Mock<IGoalRepo>();
            _inviteRepo = new Mock<IGoalInviteRepo>();
            _aIClient = new Mock<IAIClient>();
            _contextAccessor = new Mock<IHttpContextAccessor>();
            _userService = new Mock<IUserService>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperBL());  
            });
            _mapper = config.CreateMapper();

            _goalService = new GoalService(_goalRepo.Object, _contextAccessor.Object, _mapper, _aIClient.Object, _inviteRepo.Object,_userService.Object);
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
        public void CreateGoal_EmptyDescription_ThrowsValidationException()
        {
            string? invalidDescription = "";
            Goal goalDomain = new Goal(
                id: 1,
                category: Category.Week,
                type: GoalType.Education,
                createdAt: DateTime.UtcNow,
                completedAt: DateTime.UtcNow.AddDays(1),
                description: invalidDescription,
                deadline: DateTime.UtcNow.AddDays(30),
                isDone: false,
                isDeleted: false,
                isPostponed: false,
                parentGoalId: null,
                ownerId: 123
            );

            var ex = Assert.ThrowsException<ValidationException>(() =>
                _goalService.CreateGoal(goalDomain, null)
            );

            Assert.AreEqual("Goal description cannot be empty.", ex.Message);
        }

        [TestMethod]
        public void CreateGoal_DescriptionTooLong_ThrowsValidationException()
        {
            string? invalidDescription = "This is a sample description that exceeds fifty characters.";
            Goal goalDomain = new Goal(
                id: 1,
                category: Category.Week,
                type: GoalType.Education,
                createdAt: DateTime.UtcNow,
                completedAt: DateTime.UtcNow.AddDays(1),
                description: invalidDescription,
                deadline: DateTime.UtcNow.AddDays(30),
                isDone: false,
                isDeleted: false,
                isPostponed: false,
                parentGoalId: null,
                ownerId: 123
            );

            var ex = Assert.ThrowsException<ValidationException>(() =>
                _goalService.CreateGoal(goalDomain, null)
            );

            Assert.AreEqual("Goal description is too long. Maximum allowed is 50 characters.", ex.Message);
        }

        [TestMethod]
        public void CreateGoal_ShouldCreateGoal_WhenNoSharedUsers()
        {
            int ownerId = 123;
            string username = "testuser";
          

            SetSession(username);

     
            Goal goalDomain = new Goal(
                id: 1,
                category: Category.Week,
                type: GoalType.Education,
                createdAt: DateTime.UtcNow,
                completedAt: DateTime.UtcNow.AddDays(1),
                description: "Learn to program",
                deadline: DateTime.UtcNow.AddDays(30),
                isDone: false,
                isDeleted: false,
                isPostponed: false,
                parentGoalId: null,
                ownerId: ownerId
            );

            GoalEntity entity = new GoalEntity(
                id: 1,
                category: Category.Week,
                type: GoalType.Education,
                createdAt: DateTime.UtcNow,
                completedAt: DateTime.UtcNow.AddDays(1),
                description: "Learn to program",
                deadline: DateTime.UtcNow.AddDays(30),
                isDone: false,
                isDeleted: false,
                isPostponed: false,
                parentGoalId: null,
                ownerId: ownerId
            );

            _userService.Setup(r => r.GetCurrentUserId()).Returns(ownerId);

            _goalRepo.Setup(r => r.InsertGoal(ownerId, It.IsAny<GoalEntity>())).Callback<int, GoalEntity>((u, e) => e.Id = 42);
    
            _goalRepo.Setup(r => r.AssignUsersToGoal(42, new[] { ownerId }));
  
            _goalService.CreateGoal(goalDomain, sharedWithUserIds: null);

            _goalRepo.Verify(r => r.InsertGoal(ownerId, It.IsAny<GoalEntity>()), Times.Once);
            
            _goalRepo.Verify(r => r.AssignUsersToGoal(42, new[] { ownerId }), Times.Once);

            _inviteRepo.Verify(r => r.InsertInvitation(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
           
        }
        [TestMethod]
        public void CreateGoal_ShouldInviteFriends_WhenSharedUsersProvided()
        {
            int ownerId = 123;
            string username = "testuser";
            SetSession(username);

            Goal goalDomain = new Goal(
                id: 1,
                category: Category.Week,
                type: GoalType.Education,
                createdAt: DateTime.UtcNow,
                completedAt: DateTime.UtcNow.AddDays(1),
                description: "Learn to program",
                deadline: DateTime.UtcNow.AddDays(30),
                isDone: false,
                isDeleted: false,
                isPostponed: false,
                parentGoalId: null,
                ownerId: ownerId
            );

            GoalEntity entity = new GoalEntity(
                id: 1,
                category: Category.Week,
                type: GoalType.Education,
                createdAt: DateTime.UtcNow,
                completedAt: DateTime.UtcNow.AddDays(1),
                description: "Learn to program",
                deadline: DateTime.UtcNow.AddDays(30),
                isDone: false,
                isDeleted: false,
                isPostponed: false,
                parentGoalId: null,
                ownerId: ownerId
            );

            List<int> sharedWith = new List<int> { 124, 125 };

            _userService.Setup(r => r.GetCurrentUserId()).Returns(ownerId);

            _goalRepo.Setup(r => r.InsertGoal(ownerId, It.IsAny<GoalEntity>()))
                     .Callback<int, GoalEntity>((u, e) => e.Id = 99);

            _goalService.CreateGoal(goalDomain, sharedWith);

            _goalRepo.Verify(r => r.InsertGoal(ownerId, It.IsAny<GoalEntity>()), Times.Once);

            _goalRepo.Verify(r => r.AssignUsersToGoal(It.IsAny<int>(), It.IsAny<IEnumerable<int>>()), Times.Never);

            _inviteRepo.Verify(r => r.InsertInvitation(99, ownerId, 124), Times.Once);
            _inviteRepo.Verify(r => r.InsertInvitation(99, ownerId, 125), Times.Once);
        }

        [TestMethod]
        public void RespondToInvitation_DeclineOnly_UpdatesStatusAndDoesNotAssign()
        {
            GoalInvitation invitation = new GoalInvitation(23, 10, 1, 55, "Status", DateTime.Now);
            _inviteRepo.Setup(r => r.GetById(123)).Returns(invitation);

            _goalService.RespondToInvitation(invitationId: 123, accept: false, currentUserId: 20);

            _inviteRepo.Verify(r => r.UpdateStatus(123, "Declined"), Times.Once);
            _goalRepo.Verify(g => g.AssignUsersToGoal(It.IsAny<int>(), It.IsAny<IEnumerable<int>>()), Times.Never);
        }

        [TestMethod]
        public void RespondToInvitation_Accept_UpdatesStatusAndAssignsBothUsers()
        {
            GoalInvitation invitation = new GoalInvitation (234, 10, 1, 55, "Status", DateTime.Now);
            _inviteRepo.Setup(r => r.GetById(234)).Returns(invitation);

            _goalService.RespondToInvitation(invitationId: 234, accept: true, currentUserId: 20);

            _inviteRepo.Verify(r => r.UpdateStatus(234, "Accepted"), Times.Once);

            _goalRepo.Verify(g =>
                g.AssignUsersToGoal(
                    55,
                    It.Is<IEnumerable<int>>(users =>
                        users.Count() == 2 &&
                        users.Contains(10) &&
                        users.Contains(20)
                    )
                ),
                Times.Once
            );
        }

        [TestMethod]
        public void RespondToInvitation_AcceptWithSameUserIds_OnlyAssignsOnce()
        {
            
            GoalInvitation invitation = new GoalInvitation(345,42,1, 99, "Status", DateTime.Now);
            _inviteRepo.Setup(r => r.GetById(345)).Returns(invitation);

            _goalService.RespondToInvitation(invitationId: 345, accept: true, currentUserId: 42);

            _inviteRepo.Verify(r => r.UpdateStatus(345, "Accepted"), Times.Once);

            _goalRepo.Verify(g =>
                g.AssignUsersToGoal(
                    99,
                    It.Is<IEnumerable<int>>(users =>
                        users.Count() == 1 &&
                        users.Single() == 42
                    )
                ),
                Times.Once
            );
        }

        [TestMethod]
        public async Task BreakDownGoalAsync_VagueDescription_ThrowsVagueGoalDescriptionException()
        {
            int userId = 1;
            int goalId = 101;

            var entity = new GoalEntity(
                id: goalId,
                category: Category.Week,
                type: GoalType.Education,
                createdAt: DateTime.UtcNow,
                completedAt: DateTime.UtcNow,
                description: "abc", 
                deadline: DateTime.UtcNow.AddDays(7),
                isDone: false,
                isDeleted: false,
                isPostponed: false,
                parentGoalId: null,
                ownerId: userId
            );

            _userService.Setup(s => s.GetCurrentUserId()).Returns(userId);
            _goalRepo.Setup(r => r.GetGoalById(goalId, userId)).Returns(entity);

            await Assert.ThrowsExceptionAsync<VagueGoalDescriptionException>(() =>
                _goalService.BreakDownGoalAsync(goalId));
        }

        [TestMethod]
        public async Task BreakDownGoalAsync_ValidDescription_ReturnsSubGoals()
        {
            int userId = 1;
            int goalId = 101;

            var entity = new GoalEntity(
                id: goalId,
                category: Category.Week,
                type: GoalType.Education,
                createdAt: DateTime.UtcNow,
                completedAt: DateTime.UtcNow,
                description: "Learn programming",
                deadline: DateTime.UtcNow.AddDays(7),
                isDone: false,
                isDeleted: false,
                isPostponed: false,
                parentGoalId: null,
                ownerId: userId
            );

            List<string> subGoalDescriptions = new List<string>
    {
        "Choose language", "Install tools", "Write first app", "Test and debug"
    };

            _userService.Setup(s => s.GetCurrentUserId()).Returns(userId);
            _goalRepo.Setup(r => r.GetGoalById(goalId, userId)).Returns(entity);
            _aIClient.Setup(ai => ai.BreakDownTextIntoGoalsAsync("Learn programming", Category.Week))
                     .ReturnsAsync(subGoalDescriptions);

            List<Goal> result = await _goalService.BreakDownGoalAsync(goalId);

            Assert.AreEqual(4, result.Count);
            CollectionAssert.AreEqual(subGoalDescriptions, result.Select(g => g.Description).ToList());
        }

        [TestMethod]
        public async Task BreakDownGoalAsync_AIFailure_ThrowsAIRequestFailedException()
        {
            int goalId = 123;
            int userId = 99;

            GoalEntity entity = new GoalEntity(
                id: goalId,
                category: Category.Week,
                type: GoalType.Education,
                createdAt: DateTime.UtcNow,
                completedAt: DateTime.UtcNow,
                description: "Learn programming", 
                deadline: DateTime.UtcNow.AddDays(7),
                isDone: false,
                isDeleted: false,
                isPostponed: false,
                parentGoalId: null,
                ownerId: userId
            );

            _userService.Setup(u => u.GetCurrentUserId()).Returns(userId);
            _goalRepo.Setup(r => r.GetGoalById(goalId, userId)).Returns(entity);

            var userMessage = "User-friendly message.";
            var devMessage = "Developer-only technical message.";

            _aIClient.Setup(ai => ai.BreakDownTextIntoGoalsAsync("Learn programming", Category.Week))
                     .ThrowsAsync(new AIRequestFailedException(userMessage, devMessage));

            var ex = await Assert.ThrowsExceptionAsync<AIRequestFailedException>(() =>
                _goalService.BreakDownGoalAsync(goalId));

            Assert.AreEqual(userMessage, ex.UserMessage);
            Assert.AreEqual(devMessage, ex.Message);
        }
    }
}
