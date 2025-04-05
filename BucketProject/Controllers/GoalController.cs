
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using BucketProject.BLL.Business_Logic.InterfacesService;
using AutoMapper;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Text;
using Newtonsoft.Json;
using BucketProject.BLL.Business_Logic.Domain;
using BucketProject.UI.ViewModels.ViewModels;

namespace BucketProject.UI.BucketProject.Controllers
{
    public class GoalController : Controller
    {
        private readonly IGoalService _goalService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public GoalController(IGoalService goalService, IMapper mapper, IConfiguration configuration)
        {
            _goalService = goalService;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult CreateMonthGoal(GoalViewModel viewModel)
        {
            viewModel.Category = "Month";

            if (!ModelState.IsValid)
            {
            ViewBag.AvailableTypes = GetAvailableTypes();

            }

            _goalService.CreateGoal(viewModel); 

            return RedirectToAction("MonthGoals");
        }


        [HttpPost]
        public IActionResult CreateYearGoal(GoalViewModel viewModel)
        {

            viewModel.Category = "Year";

            if (!ModelState.IsValid)
            {
                ViewBag.AvailableTypes = GetAvailableTypes();

            }

            _goalService.CreateGoal(viewModel);

            return RedirectToAction("YearGoals");
        }

        [HttpPost]
        public IActionResult CreateBucketListGoal(GoalViewModel viewModel)
        {
            viewModel.Category = "Bucket_list";

            if (!ModelState.IsValid)
            {
            ViewBag.AvailableTypes = GetAvailableTypes();

            }

            _goalService.CreateGoal(viewModel);

            return RedirectToAction("BucketList");
        }

        [HttpPost]
        public IActionResult CreateWeekGoal(GoalViewModel viewModel)
        {
            viewModel.Category = "Week";

            if (!ModelState.IsValid)
            {
                ViewBag.AvailableTypes = GetAvailableTypes();

            }

            _goalService.CreateGoal(viewModel);
            return RedirectToAction("WeekGoals");
        }




        [HttpPost]
        public IActionResult EditWeekGoal(GoalViewModel viewModel)
        { 

            _goalService.UpdateGoal(viewModel.Id, viewModel);

            return RedirectToAction("WeekGoals");
        }


        [HttpPost]
        public IActionResult EditMonthGoal(GoalViewModel viewModel)
        {

            _goalService.UpdateGoal(viewModel.Id, viewModel);

            return RedirectToAction("MonthGoals");
        }


        [HttpPost]
        public IActionResult EditYearGoal(GoalViewModel viewModel)
        {

            _goalService.UpdateGoal(viewModel.Id, viewModel);

            return RedirectToAction("YearGoals");
        }


        [HttpPost]
        public IActionResult EdiBucketListGoal(GoalViewModel viewModel)
        {

            _goalService.UpdateGoal(viewModel.Id, viewModel);

            return RedirectToAction("YearGoals");
        }

        [HttpPost]
        public IActionResult DeleteGoalWeek(int id)
        {
            _goalService.DeleteGoal(id);

            return RedirectToAction("WeekGoals");
        }


        [HttpPost]
        public IActionResult DeleteGoalYear(int id)
        {
            _goalService.DeleteGoal(id);
            return RedirectToAction("YearGoals");
        }
        [HttpPost]
        public IActionResult DeleteGoalMonth(int id)
        {
            _goalService.DeleteGoal(id);
            return RedirectToAction("MonthGoals");
        }

        [HttpPost]
        public IActionResult DeleteGoalBucketList(int id)
        {
            _goalService.DeleteGoal(id);
            return RedirectToAction("BucketList");
        }


        [HttpGet]
        public IActionResult WeekGoals()
        {
            List<GoalViewModel> goals = _goalService.LoadGoalsByCategory("Week");
            ViewBag.AvailableTypes = GetAvailableTypes();


            return View(goals); 
        }



        [HttpGet]
        public IActionResult MonthGoals()
        {

            List<GoalViewModel> goals = _goalService.LoadGoalsByCategory("Month");

            ViewBag.AvailableTypes = GetAvailableTypes();


            return View(goals);
        }

        [HttpPost]
        public IActionResult ChangeGoalStatusWeek(int id, bool isDone)
        {
            _goalService.ChangeGoalStatus(id, isDone);
            return RedirectToAction("WeekGoals");

        }

        [HttpPost]
        public IActionResult ChangeGoalStatusMonth(int id, bool isDone)
        {
            _goalService.ChangeGoalStatus(id, isDone);
            return RedirectToAction("MonthGoals");

        }
        [HttpPost]
        public IActionResult ChangeGoalStatusYear(int id, bool isDone)
        {
            _goalService.ChangeGoalStatus(id, isDone);
            return RedirectToAction("YearGoals");

        }

        [HttpPost]
        public IActionResult ChangeGoalStatusBucketList(int id, bool isDone)
        {
            _goalService.ChangeGoalStatus(id, isDone);
            return RedirectToAction("BucketList");

        }


        [HttpGet]
        public IActionResult YearGoals()
        {
            List<GoalViewModel> goals = _goalService.LoadGoalsByCategory("Year");
            ViewBag.AvailableTypes = GetAvailableTypes();


            return View(goals);
        }

        [HttpGet]
        public IActionResult BucketList()
        {

            List<GoalViewModel> goals = _goalService.LoadGoalsByCategory("Bucket_list");

            ViewBag.AvailableTypes = GetAvailableTypes();

            return View(goals);
        }

        [HttpGet]
        public IActionResult WeekGoalsPreview()
        { 
            List<GoalViewModel> goals = _goalService.LoadGoalsByCategory("Week");
            return View(goals);
        }

        [HttpGet]
        public IActionResult MonthGoalsPreview()
        {
            List<GoalViewModel> goals = _goalService.LoadGoalsByCategory("Month");

            return View(goals);
        }

        [HttpGet]
        public IActionResult YearGoalsPreview()
        { 
            List<GoalViewModel> goals = _goalService.LoadGoalsByCategory("Year");
            return View(goals);
        }

        [HttpPost]
        public async Task<IActionResult> BreakDownGoal(Goal goal)
        {
            if (goal == null || string.IsNullOrWhiteSpace(goal.Description))
                return BadRequest("Invalid goal submitted.");

            var prompt = $"Break down the following goal into 4 smaller, actionable sub-goals:\n\"{goal.Description}\"";

            var apiKey = _configuration["OpenAI:ApiKey"];
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
            new { role = "user", content = prompt }
        }
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

            if (!response.IsSuccessStatusCode)
                return BadRequest("OpenAI call failed.");

            var resultJson = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(resultJson);
            string breakdownText = result.choices[0].message.content;

            var subGoals = Regex.Split(breakdownText.Trim(), @"\n\d+\.\s")
                                .Where(s => !string.IsNullOrWhiteSpace(s))
                                .ToList();

            TempData["SubGoals"] = JsonConvert.SerializeObject(subGoals);
            TempData["SubGoalForId"] = goal.Id;

            return RedirectToAction("WeekGoals");
        }

        private List<string> GetAvailableTypes()
        {
            return new List<string>
    {
          "Fitness",
            "Career",
            "Education",
            "Finance",
            "Organization",
            "Relationships",
            "Social",
            "Travel",
            "Hobbies",
            "Psychology",
            "Digital",
            "Order"
    };
        }

    }

   
}
