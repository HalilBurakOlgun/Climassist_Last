using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Climassist_Last.Data;
using Climassist_Last.Models;

namespace Climassist_Last.Controllers
{
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Requests
        public async Task<IActionResult> Index()
        {
            return View(await _context.Requests.ToListAsync());
        }

        // GET: Requests/Create
        public IActionResult Create()
        {
            // Session'dan kullanıcı bilgilerini al
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");

            // Yeni bir Requests nesnesi oluştur ve varsayılan değerleri ata
            var request = new Requests
            {
                UserName = userName ?? "",
                UserSurname = userSurname ?? ""
            };

            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserName,UserSurname,Email,ClientType,Phone,RequestType,SparePartType,RecoveryTime,UnitType,Message")] Requests request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    request.Status = "Beklemede";
                    request.CreatedAt = DateTime.Now;
                    _context.Add(request);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Talebiniz başarıyla oluşturuldu.";
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Talep oluşturulurken bir hata oluştu: " + ex.Message);
                }
            }
            return View(request);
        }
    }
}