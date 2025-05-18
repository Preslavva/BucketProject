using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BucketProject.BLLBusiness_Logic.Domain;

namespace BucketProject.BLL.Business_Logic.InterfacesService;

public interface IStatsService
{
    List<StatsDTO> GetGoalTypeStatistics();
    List<StatsDTO> GetGoalCategoryStatistics();
    List<StatsDTO> GetGoalAmountStatisticsWeekly();
    List<StatsDTO> GetGoalAmountStatisticsMonthly();
    List<StatsDTO> GetGoalAmountStatisticsYearly();
    double GetAverageCompletionTimeInDays();
    StatsDTO GetGoalSummaryStats();
    List<StatsDTO> GetUserRegistrationsPerMonth();
    List<StatsDTO> GetGoalsPerMonth();
    StatsDTO GetGoalSummaryStatsManager();
    List<StatsDTO> GetGoalTypeStatisticsManager();
     List<StatsDTO> GetGoalCategoryStatisticsManager();
  
    List<StatsDTO> GetUserAgeGroupStatistics();
    List<StatsDTO> GetUsersGenderStatistics();
    List<StatsDTO> GetUsersNationalityStatistics();
    List<User> SearchUsers(string query, string gender, string nationality, int? minAge, int? maxAge, DateTime? createdAfter);
    List<string> GetAllGenders();
    List<string> GetAllNationalities();



}
