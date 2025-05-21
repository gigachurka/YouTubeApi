using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using YouTubeApi.ViewModels;
using YouTubeApi.Models;
using Microsoft.Extensions.Logging;
using YouTubeApi.Services;

namespace YouTubeApi.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly YouTubeVideoLoaderService _videoLoader;

        public AccountController(
            UserManager<User> userManager, 
            SignInManager<User> signInManager,
            ILogger<AccountController> logger,
            YouTubeVideoLoaderService videoLoader)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _videoLoader = videoLoader;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            _logger.LogInformation("Начало процесса регистрации");
            
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Модель валидна, создаем пользователя");
                
                User user = new User
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Year = model.Year,
                    ChannelId = model.ChannelId
                };

                _logger.LogInformation($"Создание пользователя с email: {model.Email}");
                var result = await _userManager.CreateAsync(user, model.Password);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation("Пользователь успешно создан");
                    await _signInManager.SignInAsync(user, false);
                    await _videoLoader.LoadVideosByChannelIdAsync(user.ChannelId);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _logger.LogWarning("Ошибки при создании пользователя:");
                    foreach (var error in result.Errors)
                    {
                        _logger.LogWarning($"Ошибка: {error.Description}");
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
            {
                _logger.LogWarning("Модель невалидна:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning($"Ошибка валидации: {error.ErrorMessage}");
                }
            }
            
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Неверный email или пароль");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Register", "Account");
        }
    }
}