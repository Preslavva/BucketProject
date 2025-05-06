
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
using BucketProject.BLL.Business_Logic.InterfacesService;


namespace BucketsTests
{
    [TestClass]
    public class GoalServiceTest
    {
        private readonly Mock<IGoalRepo> _goalRepo;
        private readonly Mock<IGoalInviteRepo> _inviteRepo;
        private readonly Mock<IHttpContextAccessor> _contextAccessor;
        private readonly Mock<IAIClient> _aIClient;

        private readonly IMapper _mapper;
        private readonly GoalService _goalService;

        public GoalServiceTest()
        {
            _goalRepo = new Mock<IGoalRepo>();
            _inviteRepo = new Mock<IGoalInviteRepo>();
            _aIClient = new Mock<IAIClient>();
            _contextAccessor = new Mock<IHttpContextAccessor>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperBL());  // Ensure your AutoMapper profile is set up correctly
            });
            _mapper = config.CreateMapper();

            _goalService = new GoalService(_goalRepo.Object, _contextAccessor.Object, _mapper, _aIClient.Object, _inviteRepo.Object);
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
        public void CreateGoal_ShouldCreateGoal_WhenNoSharedUsers()
        {
            int ownerId = 123;
            string username = "testuser";
            //UserEntity user = new UserEntity(ownerId, username, new byte[] { 0x01 });

            SetSession(username);

      

            // Create a Goal instance with sample data
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

            // Mock the repository behavior
            var entity = new GoalEntity(
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

            _goalRepo.Setup(r => r.GetIdOfUser(username)).Returns(ownerId);
            // Mock the InsertGoal method to simulate insertion and setting the ID
            _goalRepo.Setup(r => r.InsertGoal(ownerId, It.IsAny<GoalEntity>())).Callback<int, GoalEntity>((u, e) => e.Id = 42);

            // Mock AssignUsersToGoal
            _goalRepo.Setup(r => r.AssignUsersToGoal(42, new[] { ownerId }));

            // Act
            _goalService.CreateGoal(goalDomain, sharedWithUserIds: null);

            // Assert
            // Ensure InsertGoal was called once with the ownerId
            _goalRepo.Verify(r => r.InsertGoal(ownerId, It.IsAny<GoalEntity>()), Times.Once);
            // Ensure AssignUsersToGoal was called with the goal ID and ownerId
            _goalRepo.Verify(r => r.AssignUsersToGoal(42, new[] { ownerId }), Times.Once);
            // Ensure no invitations were sent because sharedWithUserIds is null

            _inviteRepo.Verify(r => r.InsertInvitation(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }
    }
}
