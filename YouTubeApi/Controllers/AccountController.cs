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
                // Генерируем токен подтверждения
                var token = Guid.NewGuid().ToString();

                // Сохраняем данные пользователя во временное хранилище (TempData)
                TempData[$"reg_email_{token}"] = model.Email;
                TempData[$"reg_username_{token}"] = model.Email;
                TempData[$"reg_year_{token}"] = model.Year.ToString();
                TempData[$"reg_channel_{token}"] = model.ChannelId;
                TempData[$"reg_password_{token}"] = model.Password;

                // Формируем ссылку для подтверждения
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { token = token }, protocol: HttpContext.Request.Scheme);
                var messageBody = $"Пожалуйста, подтвердите ваш email, перейдя по ссылке: <a href='{confirmationLink}'>Подтвердить Email</a>";

                // Отправка письма через MailKit
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
            // Извлекаем данные из TempData
            var email = TempData[$"reg_email_{token}"] as string;
            var username = TempData[$"reg_username_{token}"] as string;
            var yearStr = TempData[$"reg_year_{token}"] as string;
            var channelId = TempData[$"reg_channel_{token}"] as string;
            var password = TempData[$"reg_password_{token}"] as string;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(yearStr) || string.IsNullOrEmpty(password))
            {
                ViewBag.Message = "Данные для подтверждения не найдены или срок действия истек. Зарегистрируйтесь заново.";
                return View();
            }

            if (!int.TryParse(yearStr, out int year))
            {
                ViewBag.Message = "Ошибка данных года рождения.";
                return View();
            }

            // Проверяем, не существует ли уже пользователь с таким email
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                ViewBag.Message = "Пользователь с таким email уже зарегистрирован.";
                return View();
            }

            var user = new User
            {
                Email = email,
                UserName = username,
                Year = year,
                ChannelId = channelId ?? string.Empty,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                await _videoLoader.LoadVideosByChannelIdAsync(user.ChannelId);
                ViewBag.Message = "Email успешно подтвержден и регистрация завершена!";
            }
            else
            {
                ViewBag.Message = "Ошибка при создании пользователя: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }
            return View();
        }
    }
}