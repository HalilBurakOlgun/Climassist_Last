using Microsoft.AspNetCore.Mvc;
using Climassist_Last.Models; // Model referansı
using Climassist_Last.Data;   // DbContext referansı
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
                // E-posta adresinin daha önce kayıtlı olup olmadığını kontrol et
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Bu e-posta adresi zaten kayıtlı.");
                    return View(user);
                }

                user.UserType = "Customer"; // Varsayılan olarak Customer
                user.CreatedAt = DateTime.Now;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Account");
            }
            return View(user);
        }

        // GET: Account/Login
        // GET: Account/Login
        public IActionResult Login()
        {
            // Eğer remember me cookie'si varsa ve geçerliyse otomatik giriş yap
            var email = Request.Cookies["UserEmail"];
            var password = Request.Cookies["UserPassword"];

            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                return Login(email, password, true).Result;
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe = false)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Email == email && m.Password == password);

            if (user == null)
            {
                ModelState.AddModelError("", "Geçersiz email veya şifre");
                return View(new LoginViewModel { Email = email });
            }

            // Remember Me işlemleri
            if (rememberMe)
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30), // 30 gün geçerli olacak
                    HttpOnly = true,
                    Secure = true // HTTPS üzerinden erişim için
                };

                Response.Cookies.Append("UserEmail", email, cookieOptions);
                Response.Cookies.Append("UserPassword", password, cookieOptions);
            }
            else
            {
                // Remember Me seçili değilse varolan cookie'leri sil
                Response.Cookies.Delete("UserEmail");
                Response.Cookies.Delete("UserPassword");
            }

            // Session'a kullanıcı bilgilerini kaydet
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("UserSurname", user.SurName);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserType", user.UserType);

            return RedirectToAction("Index", "Home");
        }

        // Logout action'ını da günceleyelim
        public IActionResult Logout()
        {
            // Session'ı temizle
            HttpContext.Session.Clear();

            // Remember Me cookie'lerini sil
            Response.Cookies.Delete("UserEmail");
            Response.Cookies.Delete("UserPassword");

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
        public async Task<IActionResult> UserList(string userType)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin")
            {
                return Forbid();
            }

            var query = _context.Users.AsQueryable();

            // Filtreleme
            if (!string.IsNullOrEmpty(userType))
            {
                query = query.Where(u => u.UserType == userType);
            }

            var users = await query.ToListAsync();

            return View(users);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UserList));
        }
    }
}