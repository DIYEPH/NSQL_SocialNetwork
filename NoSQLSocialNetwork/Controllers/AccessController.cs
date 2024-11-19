using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using NoSQLSocialNetwork.Data;
using NoSQLSocialNetwork.Entities;
using System.Security.Claims;

namespace NoSQLSocialNetwork.Controllers
{
    public class AccessController : Controller
    {
        private readonly IMongoCollection<User>? _users;
        public AccessController(MongoDbService mongoDbService)
        {
            _users = mongoDbService.Database?.GetCollection<User>("Users");
        }
        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _users
                .Find(u => u.Email == email && u.PasswordHash == password)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            // Tạo các claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),  // Lưu thông tin quyền của người dùng
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Tạo cookie
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return RedirectToAction("Index", "Home");
        }
    }
}
