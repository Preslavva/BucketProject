using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.DAL.Models.Entities;

namespace BucketProject.BLL.Business_Logic.InterfacesRepo
{
    public interface IManagerRepo
    {
        List<UserEntity> GetAllUsers();
        List<GoalEntity> GetAllGoals();
        List<GoalEntity> LoadAllPersonalGoals();
        List<GoalEntity> LoadAllSharedGoals();
        int GetActiveUsersCount();
        List<UserEntity> SearchUsers(string query, string gender, string nationality, int? minAge, int? maxAge, DateTime? createdAfter, int page, int pageSize);

        List<string> GetAllDistinctNationalities();
        List<string> GetAllDistinctGenders();
        int CountFilteredUsers(string query, string gender, string nationality, int? minAge, int? maxAge, DateTime? createdAfter);
    }
}
