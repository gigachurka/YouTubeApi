using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using YouTubeApi.ViewModels;
using YouTubeApi.Models;
using Microsoft.Extensions.Logging;
using YouTubeApi.Services;
using System;
using MailKit.Net.Smtp;
using MimeKit;
using System.Linq;

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
                _logger.LogInformation("Модель валидна, формируем токен и отправляем письмо");
                var token = Guid.NewGuid().ToString();
                var expiry = DateTime.UtcNow.AddHours(24); // 24 часа на подтверждение


                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ViewBag.Message = "Пользователь с таким email уже зарегистрирован.";
                    return View("Register");
                }

                var user = new User
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Year = model.Year,
                    ChannelId = model.ChannelId ?? string.Empty,
                    EmailConfirmed = false,
                    EmailConfirmationToken = token,
                    RegistrationTokenExpiry = expiry,
                    RegistrationEmail = model.Email,
                    RegistrationUsername = model.Email,
                    RegistrationYear = model.Year,
                    RegistrationChannelId = model.ChannelId,
                    RegistrationPassword = model.Password
                };
                await _userManager.CreateAsync(user, model.Password);

                var confirmationLink = Url.Action("ConfirmEmail", "Account", new
                {
                    token = token
                },
                protocol: HttpContext.Request.Scheme);
                var messageBody = $"Пожалуйста, подтвердите ваш email, перейдя по ссылке: <a href='{confirmationLink}'>Подтвердить Email</a> <br>Надёжно, как Кужеленов Кирилл.";

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("YouTubeApi", "kuzhelinovk@gmail.com"));
                message.To.Add(new MailboxAddress("", model.Email));
                message.Subject = "Подтверждение email";
                message.Body = new TextPart("html") { Text = messageBody };

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);
                    client.Authenticate("kuzhelinovk@gmail.com", "mdea rgsk qvmo zqcg");
                    client.Send(message);
                    client.Disconnect(true);
                }

                ViewBag.Message = "Письмо с подтверждением отправлено на ваш email. Пожалуйста, перейдите по ссылке в письме для завершения регистрации.";
                return View("Register");
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

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Message = "Некорректная ссылка подтверждения.";
                return View();
            }
            var user = _userManager.Users.FirstOrDefault(u => u.EmailConfirmationToken == token);
            if (user == null || user.RegistrationTokenExpiry == null || user.RegistrationTokenExpiry < DateTime.UtcNow)
            {
                ViewBag.Message = "Данные для подтверждения не найдены или срок действия истек. Зарегистрируйтесь заново.";
                return View();
            }
            if (user.EmailConfirmed)
            {
                ViewBag.Message = "Email уже подтверждён.";
                return View();
            }
            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.RegistrationTokenExpiry = null;
            user.RegistrationEmail = null;
            user.RegistrationUsername = null;
            user.RegistrationYear = null;
            user.RegistrationChannelId = null;
            user.RegistrationPassword = null;
            await _userManager.UpdateAsync(user);
            await _signInManager.SignInAsync(user, false);
            await _videoLoader.LoadVideosByChannelIdAsync(user.ChannelId);
            ViewBag.Message = "Email успешно подтвержден и регистрация завершена!";
            return View();
        }
    }
}