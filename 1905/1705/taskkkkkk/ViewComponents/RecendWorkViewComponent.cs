using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFrontToBack.Areas.Admin.ViewModel;
using WebFrontToBack.DAL;
using WebFrontToBack.Models;

namespace WebFrontToBack.ViewComponents
{
    public class RecendWorkViewComponent:ViewComponent
    {
        private readonly AppDbContext _context;

        public RecendWorkViewComponent(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<RecentWorks> recentWorks = await _context.RecentWorks
                .OrderByDescending(s => s.Id)

                .Take(3)

                .ToListAsync();
            return View(recentWorks);

        }

    }
}
