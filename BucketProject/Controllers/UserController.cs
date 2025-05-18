using Microsoft.AspNetCore.Mvc;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.BLLBusiness_Logic.Domain;
using BucketProject.UI.ViewModels.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;


namespace BucketProject.UI.BucketProject.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;


        public UserController(IUserService userService, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            _userService = userService;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }

        private async Task<List<SelectListItem>> LoadCountriesAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var json = await client.GetStringAsync("https://restcountries.com/v3.1/all");
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement
                      .EnumerateArray()
                      .Select(el => {
                          var name = el.GetProperty("name")
                                       .GetProperty("common")
                                       .GetString()!;
                          return new SelectListItem { Value = name, Text = name };
                      })
                      .OrderBy(x => x.Text)
                      .ToList();
        }


        [HttpGet]
        public async Task<IActionResult> Register()
        {
            RegisterViewModel vm = new RegisterViewModel
            {
                Countries = await LoadCountriesAsync()
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            vm.Countries = await LoadCountriesAsync();

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
            catch (ValidationException vex)
            {
                ModelState.AddModelError(string.Empty, vex.Message);
                return View(vm);
            }
            catch (ApplicationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
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
            try
            {
                User? loggedUser = _userService.LogIn(vm.Username, vm.Password);

                HttpContext.Session.SetString("Username", loggedUser.Username);
                HttpContext.Session.SetString("Role", loggedUser.Role);

                return loggedUser.Role == "Manager"
                    ? RedirectToAction("ManagerSearch", "Manager")
                    : RedirectToAction("Index", "Home");
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
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

                var userDomain = _userService.GetUserByUsername();
                var vm = _mapper.Map<UserViewModel>(userDomain);

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
