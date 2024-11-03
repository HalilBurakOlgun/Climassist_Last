using Microsoft.AspNetCore.Mvc;
using Climassist_Last.Models;
using Climassist_Last.Data;
using Microsoft.EntityFrameworkCore;

namespace Climassist_Last.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Name,SurName,Email,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                // Otomatik olarak Customer rolü atama
                user.UserType = "Customer";
                user.CreatedAt = DateTime.Now;

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Account");
            }
            return View(user);
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Email == email && m.Password == password);

            if (user == null)
            {
                ModelState.AddModelError("", "Geçersiz email veya şifre");
                return View();
            }

            // Session'a kullanıcı bilgilerini kaydet
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("UserSurname", user.SurName);  // Soyadı da ekliyoruz
            HttpContext.Session.SetString("UserType", user.UserType);

            return RedirectToAction("Index", "Home");
        }
    }
}