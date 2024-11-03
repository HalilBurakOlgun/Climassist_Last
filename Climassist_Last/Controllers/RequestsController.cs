using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Climassist_Last.Data;
using Climassist_Last.Models;
using System;

namespace Climassist_Last.Controllers
{
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            // Session'dan ad ve soyadı çek
            ViewBag.UserName = HttpContext.Session.GetString("Name");
            ViewBag.UserSurname = HttpContext.Session.GetString("Surname");
            ViewBag.IsLoggedIn = !string.IsNullOrEmpty(ViewBag.UserName); // Login kontrolü

            return View();
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
                    ModelState.AddModelError("", "Talep oluşturulurken bir hata oluştu.");
                }
            }
            return View(request);
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Requests.ToListAsync());
        }
    }
}