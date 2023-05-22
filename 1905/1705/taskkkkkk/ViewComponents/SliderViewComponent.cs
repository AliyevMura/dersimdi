using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFrontToBack.DAL;

namespace WebFrontToBack.ViewComponents
{
    public class SliderViewComponent: ViewComponent
    {
        private readonly AppDbContext _appContext;

        public SliderViewComponent(AppDbContext appContext)
        {
            _appContext = appContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _appContext.Sliders.ToListAsync());
        }
    }
}
