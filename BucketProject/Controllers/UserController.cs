using Microsoft.AspNetCore.Mvc;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.BLL.Business_Logic.DTOs;
using BucketProject.UI.ViewModels.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using Exceptions.Exceptions;


namespace BucketProject.UI.BucketProject.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<UserController> _logger;


        public UserController(IUserService userService, IMapper mapper, IHttpClientFactory httpClientFactory, ILogger<UserController> logger)
        {
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }

        private static readonly Uri CountriesNowEndpoint =
     new("https://countriesnow.space/api/v0.1/countries");

        private async Task<List<SelectListItem>> LoadCountriesAsync()
        {
            var client = _httpClientFactory.CreateClient();

            using var response = await client.GetAsync(CountriesNowEndpoint, HttpCompletionOption.ResponseHeadersRead);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Failed to load countries (status {(int)response.StatusCode} {response.ReasonPhrase}).");
            }

            await using var contentStream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(contentStream);

            var dataArray = doc.RootElement
                               .GetProperty("data")
                               .EnumerateArray();

            var countries = dataArray
                .Select(el => new SelectListItem
                {
                    Value = el.GetProperty("country").GetString()!,
                    Text = el.GetProperty("country").GetString()!
                })
                .OrderBy(x => x.Text)
                .ToList();

            if (countries.Count == 0)
            {
                throw new InvalidOperationException(
                    "The countries list came back empty – the API payload contained no elements.");
            }

            return countries;
        }



        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var vm = new RegisterViewModel();
            await PopulateCountriesAsync(vm);  
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            await PopulateCountriesAsync(vm);

            if (!ModelState.IsValid)
                return View(vm);

            var user = _mapper.Map<User>(vm);

            try
            {
                if (_userService.Register(user))
                    return RedirectToAction("LogIn", "User");

                ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
                return View(vm);
            }
            catch (ValidationExceptionCollection vex)
            {
                foreach (var err in vex.Errors)
                    ModelState.AddModelError(string.Empty, err);
                return View(vm);
            }
            catch (DuplicateUserException dex)
            {
                ModelState.AddModelError(string.Empty, dex.Message);
                return View(vm);
            }
        }

        private async Task PopulateCountriesAsync(RegisterViewModel vm)
        {
            try
            {
                vm.Countries = await LoadCountriesAsync();
                vm.CountriesLoaded = vm.Countries.Any();
            }
            catch (HttpRequestException ex)          
            {
                _logger.LogError(ex, "Could not reach api");
                ModelState.AddModelError(
                   string.Empty,
                   "Sorry – we couldn’t load the list of countries just now. "); ;
                vm.Countries = new List<SelectListItem>();    
            }
            catch (InvalidOperationException ex)    
            {
                _logger.LogWarning(ex, "Countries payload invalid or empty");
                ModelState.AddModelError(
                  string.Empty,
                  "Sorry – we couldn’t load the list of countries just now. "); ;
                vm.Countries = new List<SelectListItem>();
            }
              
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LogIn(LogInViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);
            try
            {
                var loggedUser = _userService.LogIn(vm.Username, vm.Password);

                HttpContext.Session.SetString("Username", loggedUser.Username);
                HttpContext.Session.SetString("Role", loggedUser.Role);

                return loggedUser.Role == "Manager"
                    ? RedirectToAction("ManagerSearch", "Manager")
                    : RedirectToAction("Index", "Home");
            }
            catch (ValidationExceptionCollection ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            catch (InvalidLoginException lex)
            {
                ModelState.AddModelError(string.Empty, lex.Message);
            }

            return View(vm);
        }




        [HttpGet]
        public IActionResult Account()
        {

            User userDomain = _userService.GetUserByUsername();

            UserViewModel userViewModel = _mapper.Map<UserViewModel>(userDomain);

            return View(userViewModel);
        }

        [HttpPost]
        public IActionResult UpdateUsername(string newUsername)
        {
            try
            {
                _userService.UpdateUsername(newUsername);

                return RedirectToAction("Account", "User");
            }
            catch (ValidationException ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
                User userDomain = _userService.GetUserByUsername();
                UserViewModel vm = _mapper.Map<UserViewModel>(userDomain);
                return View("Account", vm);
            }
            catch(DuplicateUsernameException dex)
            {
                ModelState.AddModelError(string.Empty, dex.Message);
                User userDomain = _userService.GetUserByUsername();
                UserViewModel vm = _mapper.Map<UserViewModel>(userDomain);
                return View("Account", vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPhoto(IFormFile photoFile)
        {
            try
            {
                await _userService.UpdateProfilePicture(photoFile);
                return RedirectToAction("Account", "User");
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                User userDomain = _userService.GetUserByUsername();
                UserViewModel vm = _mapper.Map<UserViewModel>(userDomain);

                return View("Account", vm);
            }
        }

            [HttpGet]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LogIn", "User");
        }

    }
}
