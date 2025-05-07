using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using YouTubeApi.Data;
using YouTubeApi.Models;

namespace YouTubeApi.Controllers
{

    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AccountController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost("user")]
        public async Task<IActionResult> Register(string login, string password, string email)
        {
            if (await _db.Users.AnyAsync(u => u.Login == login))
                return BadRequest("Пользователь с таким логином уже существует.");

            var user = new User
            {
                Login = login,
                PasswordHash = HashPassword(password),
                Email = email
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return Ok("Регистрация прошла успешно");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string login, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Login == login);

            if (user == null || user.PasswordHash != HashPassword(password))
                return Unauthorized("Неверный логин или пароль");

            // можно сохранить сессию или куки при желании
            return Ok("Успешный вход");
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }

}
