
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
                        var viewModels = type.Value
                            .Select(goal => _mapper.Map<HistoryViewModel>(goal))
                            .ToList();

                        groupedViewModels[category.Key][timeframe.Key][type.Key] = viewModels;
                    }
                }
            }

            return View(groupedViewModels);
        }

    }
}
