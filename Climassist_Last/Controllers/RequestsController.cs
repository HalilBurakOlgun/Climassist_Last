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
            var userId = HttpContext.Session.GetString("UserId");
            var userType = HttpContext.Session.GetString("UserType");

            if (string.IsNullOrEmpty(userId))
            {
                // Giriş yapmamış kullanıcıları TrackRequest sayfasına yönlendir
                return RedirectToAction("TrackRequest");
            }

            if (userType == "Customer")
            {
                // Customer sadece kendi taleplerini görür
                var userName = HttpContext.Session.GetString("UserName");
                var userSurname = HttpContext.Session.GetString("UserSurname");
                var requests = await _context.Requests
                    .Where(r => r.UserName == userName && r.UserSurname == userSurname)
                    .ToListAsync();

                return View(requests);
            }
            else if (userType == "Admin" || userType == "Staff")
            {
                // Admin ve Staff tüm talepleri görür
                var allRequests = await _context.Requests.ToListAsync();
                return View(allRequests);
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Requests/Create
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var request = new Requests();

            if (!string.IsNullOrEmpty(userId))
            {
                // Giriş yapmış kullanıcılar için bilgileri otomatik doldur
                request.UserName = HttpContext.Session.GetString("UserName") ?? "";
                request.UserSurname = HttpContext.Session.GetString("UserSurname") ?? "";
                request.Email = HttpContext.Session.GetString("UserEmail") ?? "";
            }

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
                    var userId = HttpContext.Session.GetString("UserId");
                    request.Status = "Beklemede";
                    request.CreatedAt = DateTime.Now;

                    if (string.IsNullOrEmpty(userId))
                    {
                        // Giriş yapmamış kullanıcılar için benzersiz takip kodu oluştur
                        string trackingCode;
                        do
                        {
                            trackingCode = new Random().Next(100000, 999999).ToString();
                        } while (await _context.Requests.AnyAsync(r => r.TrackingCode == trackingCode));

                        request.TrackingCode = trackingCode;
                        TempData["TrackingCode"] = trackingCode;
                    }

                    _context.Add(request);
                    await _context.SaveChangesAsync();

                    if (string.IsNullOrEmpty(userId))
                    {
                        return RedirectToAction("TrackingCodeSuccess");
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Talebiniz başarıyla oluşturuldu.";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Talep oluşturulurken bir hata oluştu: " + ex.Message);
                }
            }
            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var userType = HttpContext.Session.GetString("UserType");

            if (userType != "Admin" && userType != "Staff")
            {
                return Forbid();
            }

            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            request.Status = newStatus;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelRequest(int id)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("TrackRequest");
            }

            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            if (request.Status == "Beklemede")
            {
                request.Status = "İptal Edildi";
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Talebiniz başarıyla iptal edildi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Sadece beklemedeki talepler iptal edilebilir.";
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult TrackRequest()
        {
            // Eğer kullanıcı giriş yapmışsa Index'e yönlendir
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TrackRequest(string trackingCode)
        {
            if (string.IsNullOrEmpty(trackingCode))
            {
                TempData["ErrorMessage"] = "Lütfen takip kodu giriniz.";
                return View();
            }

            var request = await _context.Requests
                .FirstOrDefaultAsync(r => r.TrackingCode == trackingCode);

            if (request == null)
            {
                TempData["ErrorMessage"] = "Geçersiz takip kodu.";
                return View();
            }

            return View("TrackRequestResult", request);
        }

        public IActionResult TrackingCodeSuccess()
        {
            var trackingCode = TempData["TrackingCode"]?.ToString();
            if (string.IsNullOrEmpty(trackingCode))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}