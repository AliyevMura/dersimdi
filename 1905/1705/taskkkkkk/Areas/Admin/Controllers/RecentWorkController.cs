using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFrontToBack.DAL;
using WebFrontToBack.Models;

namespace WebFrontToBack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RecentWorkController : Controller
    {
        private readonly AppDbContext _context;



        public RecentWorkController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ICollection<RecentWorks> services = await _context.RecentWorks.ToListAsync();
            return View(services);
        }
        



        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RecentWorks recentWorks)
        {


            if (!ModelState.IsValid)
            {
                return View();
            }

            bool isExists = await _context.RecentWorks.AnyAsync(c =>
            c.Name.ToLower().Trim() == recentWorks.Name.ToLower().Trim());


            if (isExists)
            {
                ModelState.AddModelError("Name", "Recent works name already exists");
                return View();
            }
            await _context.RecentWorks.AddAsync(recentWorks);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int Id)
        {
            RecentWorks? recentWorks = _context.RecentWorks.Find(Id);

            if (recentWorks == null)
            {
                return NotFound();
            }

            return View(recentWorks);
        }

        [HttpPost]
        public IActionResult Update(RecentWorks recentWorks)
        {
            RecentWorks? editedRecentWorks = _context.RecentWorks.Find(recentWorks.Id);
            if (editedRecentWorks == null)
            {
                return NotFound();
            }
            editedRecentWorks.Name = recentWorks.Name;
            editedRecentWorks.ImagePath = recentWorks.ImagePath;
            editedRecentWorks.Description = recentWorks.Description;
            _context.RecentWorks.Update(editedRecentWorks);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int Id)
        {
            RecentWorks? recentWorks = _context.RecentWorks.Find(Id);
            if (recentWorks == null)
            {
                return NotFound();
            }
            _context.RecentWorks.Remove(recentWorks);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


    }
}
