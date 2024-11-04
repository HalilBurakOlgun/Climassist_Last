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
                user.UserType = "Customer"; // Varsayılan olarak Customer
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
            HttpContext.Session.SetString("UserSurname", user.SurName);
            HttpContext.Session.SetString("UserEmail", user.Email); // Email'i session'a ekliyoruz
            HttpContext.Session.SetString("UserType", user.UserType);

            return RedirectToAction("Index", "Home");
        }
        // GET: Account/Logout
        public IActionResult Logout()
        {
            // Tüm session verilerini temizle
            HttpContext.Session.Clear();

            // Ana sayfaya yönlendir
            return RedirectToAction("Index", "Home");
        }
        // GET: Account/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Account/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,SurName,Email,Password,User Type")] User user)
        {
            if (ModelState.IsValid)
            {
                user.CreatedAt = DateTime.Now;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index"); // Kullanıcılar listesine yönlendirin
            }
            return View(user);
        }
    }
}