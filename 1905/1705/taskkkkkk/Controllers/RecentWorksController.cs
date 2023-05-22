using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFrontToBack.DAL;

namespace WebFrontToBack.Controllers
{
    public class RecentWorksController : Controller
    {
        protected readonly AppDbContext _context;

        public RecentWorksController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.RecentWorks
                .OrderByDescending(s => s.Id)
                
                .Take(3)
                
                .ToListAsync());
        }
    }
}
