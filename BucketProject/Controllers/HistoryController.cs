
using BucketProject.BLL.Business_Logic.InterfacesService;
using Microsoft.AspNetCore.Mvc;
using BucketProject.UI.ViewModels.ViewModels;
using AutoMapper;

namespace BucketProject.Controllers
{
    public class HistoryController : Controller
    {
        private readonly IGoalService _goalService;
        private readonly IMapper _mapper;

        public HistoryController(IGoalService goalService, IMapper mapper)
        {
            _goalService = goalService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult History()
        {
            var groupedGoals = _goalService.LoadGroupedExpiredGoals();

            var groupedViewModels = new Dictionary<string, Dictionary<string, Dictionary<string, List<HistoryViewModel>>>>();

            foreach (var category in groupedGoals)
            {
                if (!groupedViewModels.ContainsKey(category.Key))
                    groupedViewModels[category.Key] = new Dictionary<string, Dictionary<string, List<HistoryViewModel>>>();

                foreach (var timeframe in category.Value)
                {
                    if (!groupedViewModels[category.Key].ContainsKey(timeframe.Key))
                        groupedViewModels[category.Key][timeframe.Key] = new Dictionary<string, List<HistoryViewModel>>();

                    foreach (var type in timeframe.Value)
                    {
                        // First map all goals
                        var allViewModels = type.Value
                            .Select(goal => _mapper.Map<HistoryViewModel>(goal))
                            .ToList();

                        // Build a lookup dictionary: Id -> ViewModel
                        var viewModelDict = allViewModels.ToDictionary(vm => vm.Id, vm => vm);

                        // Assign child goals to their parents
                        foreach (var vm in allViewModels)
                        {
                            if (vm.ParentGoalId.HasValue && viewModelDict.ContainsKey(vm.ParentGoalId.Value))
                            {
                                viewModelDict[vm.ParentGoalId.Value].ChildGoals.Add(vm);
                            }
                        }

                        // Only return top-level (parent) goals
                        var topLevelGoals = allViewModels
                            .Where(vm => !vm.ParentGoalId.HasValue)
                            .ToList();

                        groupedViewModels[category.Key][timeframe.Key][type.Key] = topLevelGoals;
                    }
                }
            }

            return View(groupedViewModels);
        }


    }
}
