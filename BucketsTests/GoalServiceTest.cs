
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
                cfg.AddProfile(new AutoMapperBL());  
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
          
            _goalRepo.Setup(r => r.InsertGoal(ownerId, It.IsAny<GoalEntity>())).Callback<int, GoalEntity>((u, e) => e.Id = 42);

            
            _goalRepo.Setup(r => r.AssignUsersToGoal(42, new[] { ownerId }));

            
            _goalService.CreateGoal(goalDomain, sharedWithUserIds: null);

            
            _goalRepo.Verify(r => r.InsertGoal(ownerId, It.IsAny<GoalEntity>()), Times.Once);
            
            _goalRepo.Verify(r => r.AssignUsersToGoal(42, new[] { ownerId }), Times.Once);

            _inviteRepo.Verify(r => r.InsertInvitation(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            //test 4
        }
    }
}
