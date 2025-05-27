using BucketProject.BLL.Business_Logic.InterfacesRepo;
using Moq;
using AutoMapper;
using BucketProject.BLL.Business_Logic.Services;
using BucketProject.BLL.Business_Logic.Mapping;
using BucketProject.DAL.Models.Entities;

namespace BucketsTests
{
    [TestClass]
    public class SocialServiceTests
    {
        private readonly Mock<ISocialRepo> _socialRepo;
        private readonly IMapper _mapper;  
        private readonly SocialService _socialService;

        public SocialServiceTests()
        {
            _socialRepo = new Mock<ISocialRepo>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperBL());  
            });
            _mapper = config.CreateMapper();  

            _socialService = new SocialService(_socialRepo.Object, _mapper); 
        }

        [TestMethod]
        public void GetFriends_ShouldReturnMappedFriends_WhenFriendsExist()
        {
            int userId = 1;

            var friendEntities = new List<UserEntity>
        {
            new UserEntity(2, "friend1", "friend1@example.com", "password1", new byte[] { 0x01 }, "salt1", "Country1", DateTime.Now, "Male", new DateOnly(2010, 1, 1), "User"),
            new UserEntity(3, "friend2", "friend2@example.com", "password2", new byte[] { 0x02 }, "salt2", "Country2", DateTime.Now, "Female", new DateOnly(2012, 1, 1), "User")
        };

            _socialRepo.Setup(repo => repo.LoadFriends(userId)).Returns(friendEntities);

            var result = _socialService.GetFriends(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);  
            Assert.AreEqual("friend1", result[0].Username);
            Assert.AreEqual("friend2", result[1].Username);
            _socialRepo.Verify(repo => repo.LoadFriends(userId), Times.Once); 
        }

        [TestMethod]
        public void GetFriends_ShouldReturnEmptyList_WhenNoFriendsExist()
        {
            int userId = 1;

            _socialRepo.Setup(repo => repo.LoadFriends(userId)).Returns(new List<UserEntity>());

            var result = _socialService.GetFriends(userId);

            Assert.IsNotNull(result);     
            Assert.AreEqual(0, result.Count);  
            _socialRepo.Verify(repo => repo.LoadFriends(userId), Times.Once); 
        }
    }
}
