using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using NoSQLSocialNetwork.Data;
using NoSQLSocialNetwork.Entities;
using NoSQLSocialNetwork.ViewModels;
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
        public async Task<IActionResult> Login(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            var user = await _users
                .Find(u => u.Email == model.Email && u.PasswordHash == model.Password)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                ViewData["ErrorMessage"] = "Không có tài khoản này!";
                return View(model); 
            }

            // Tạo các claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) 
            };

            if (!string.IsNullOrEmpty(user.FullName))
                claims.Add(new Claim(ClaimTypes.Name, user.FullName));

            if (!string.IsNullOrEmpty(user.Email))
                claims.Add(new Claim(ClaimTypes.Email, user.Email));

            if (!string.IsNullOrEmpty(user.Role))
                claims.Add(new Claim(ClaimTypes.Role, user.Role));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : null
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
            if (string.Equals(user.Role, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            if (Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }
            else
            {
                return Redirect("/");
            }
        }
        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Signup(SignupVM model)
        {
            if (ModelState.IsValid)
            {
                var existingAccount = await _users
                 .Find(u => u.Email == model.Email)
                 .FirstOrDefaultAsync();
                if (existingAccount != null)
                {
                    @ViewData["ErrorMessage"] = "Tên đăng nhập đã tồn tại";
                    return View(model);
                }

                var user = new User
                {
                    FullName = model.Fullname,
                    Email = model.Email,
                    PasswordHash = model.Password,
                    Role = "user",
                    CreateAt = DateTime.UtcNow,
                    AvatarUrl = "/img/avatardefault.png"
				};
                if (_users != null)
                    await _users.InsertOneAsync(user);
                TempData["SuccessMessage"] = "Đăng ký tài khoản thành công!";
                return RedirectToAction("Login", "Access");
            }
            return View();
        }
    }
}
