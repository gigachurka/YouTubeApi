//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading.Tasks;
//using YouTubeApi.Data;
//using YouTubeApi.Models;

//namespace YouTubeApi.Controllers
//{

//    public class AccountController : Controller
//    {
//        private readonly ApplicationDbContext _db;

//        public AccountController(ApplicationDbContext db)
//        {
//            _db = db;
//        }
//        [HttpGet("mixpanel/engage")]
//        public async Task<IActionResult> GetMixpanelData()
//        {
//            using var httpClient = new HttpClient();
//            var response = await httpClient.GetAsync("https://api-js.mixpanel.com/engage/?verbose=1&ip=1");

//            var content = await response.Content.ReadAsStringAsync();
//            return Content(content, "application/json");
//        }

//        [HttpPost]
//        public async Task<IActionResult> Register([FromForm] string login, [FromForm] string password, [FromForm] string email)

//        {
//            Console.WriteLine("Register action called");
//            if (await _db.Users.AnyAsync(u => u.Login == login))
//                return BadRequest("Пользователь с таким логином уже существует.");

//            if (!IsPasswordStrong(password, out var error))
//                return BadRequest($"Слабый пароль: {error}");

//            var user = new User
//            {
//                Login = login,
//                PasswordHash = HashPassword(password),
//                Email = email
//            };

//            _db.Users.Add(user);
//            await _db.SaveChangesAsync();
//            return RedirectToAction("Index", "Home");
//        }

//        private bool IsPasswordStrong(string password, out string error)
//        {
//            error = string.Empty;

//            if (password.Length < 8)
//            {
//                error = "Пароль должен содержать минимум 8 символов.";
//                return false;
//            }

//            if (!password.Any(char.IsUpper))
//            {
//                error = "Пароль должен содержать хотя бы одну заглавную букву.";
//                return false;
//            }

//            if (!password.Any(char.IsLower))
//            {
//                error = "Пароль должен содержать хотя бы одну строчную букву.";
//                return false;
//            }

//            if (!password.Any(char.IsDigit))
//            {
//                error = "Пароль должен содержать хотя бы одну цифру.";
//                return false;
//            }

//            if (!password.Any(ch => "!@#$%^&*()-_=+[]{}|;:',.<>?/`~".Contains(ch)))
//            {
//                error = "Пароль должен содержать хотя бы один специальный символ.";
//                return false;
//            }

//            return true;
//        }


//        [HttpPost("Login")]
//        public async Task<IActionResult> Login(string login, string password)
//        {
//            var user = await _db.Users.FirstOrDefaultAsync(u => u.Login == login);

//            if (user == null || user.PasswordHash != HashPassword(password))
//                return Unauthorized("Неверный логин или пароль");
//            // Можно сохранить ID в куки или TempData, если нужно передавать между страницами
//            return RedirectToAction("Index", "Home");
//        }

//        private string HashPassword(string password)
//        {
//            using var sha256 = SHA256.Create();
//            var bytes = Encoding.UTF8.GetBytes(password);
//            var hash = sha256.ComputeHash(bytes);
//            return Convert.ToBase64String(hash);
//        }
//    }

//}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CustomIdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using YouTubeApi.ViewModels;
//using YouTubeApi.CustomIdentityApp.ViewModels;

namespace YouTubeApi.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Year = model.Year,
                    ChannelId = model.ChannelId // ← добавь это
                };

                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // установка куки
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
    }
}