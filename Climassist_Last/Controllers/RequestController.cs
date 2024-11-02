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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserName,UserSurname,Email,ClientType,Phone,RequestType,SparePartType,RecoveryTime,UnitType,Message")] Requests request)
        {
            if (ModelState.IsValid)
            {
                request.Status = "Pending";
                _context.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(request);
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Requests.ToListAsync());
        }
    }
}