using Microsoft.AspNetCore.Mvc;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.BLLBusiness_Logic.Domain;
using BucketProject.UI.ViewModels.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;


namespace BucketProject.UI.BucketProject.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;


        public UserController(IUserService userService, IPasswordHasher passwordHasher, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
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
            var vm = new RegisterViewModel
            {
                Countries = await LoadCountriesAsync()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel user)
        {
            user.Countries = await LoadCountriesAsync();

            if (!ModelState.IsValid)
                return View(user);

            User newUser = _mapper.Map<User>(user);
            try
            {
                if (_userService.Register(newUser))
                    return RedirectToAction("LogIn", "User");
            }
            catch (ApplicationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(user);
            }

            return View(user);
        }


        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LogIn(LogInViewModel user)
        {
            try
            {
                User loggedUser = _mapper.Map<User>(user);
                loggedUser = _userService.LogIn(user.Username, user.Password);

                if (loggedUser != null)
                {
                    HttpContext.Session.SetString("Username", loggedUser.Username);
                    HttpContext.Session.SetString("Role", loggedUser.Role);

                    if (loggedUser.Role == "Manager")
                        return RedirectToAction("Manager", "Manager");  
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ErrorMessage = "Wrong username or password";
                    return View();
                }
            }
            catch (ArgumentException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
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
            _userService.UpdateUsername(newUsername);

            return RedirectToAction("Account", "User");
        }

        [HttpPost]
        public async Task<IActionResult> AddPhoto(IFormFile photoFile)
        {
            await _userService.UpdateProfilePicture(photoFile);
            return RedirectToAction("Account", "User");
        }

        [HttpGet]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LogIn", "User");
        }

    }
}
