using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebFrontToBack.DAL;
using WebFrontToBack.ViewModel;

namespace WebFrontToBack.Controllers
{
    public class BasketController : Controller
    {
        private const string COOKIES_BASKET="basketVm";
        private readonly AppDbContext _appDbContext;
        private IEnumerable<BasketVm> basketVms;

        public IActionResult Index()
        {
            List<BasketItemVm> basketItemVMs = new List<BasketItemVm>();
            List<BasketVm> basketVMS = JsonConvert.DeserializeObject<List<BasketVm>>(Request.Cookies[COOKIES_BASKET]);
            foreach (BasketVm item in basketVms)
            {
                BasketItemVm basketItemVM = _appDbContext.Services
                                                .Where(s => !s.IsDeleted && s.Id == item.ServiceId)
                                                .Include(s => s.Category)
                                                .Include(s => s.ServiceImages)
                                                .Select(s => new BasketItemVm
                                                {
                                                    Name = s.Name,
                                                    Id = s.Id,
                                                    CategoryName = s.Category.Name,
                                                    IsDeleted = s.IsDeleted,
                                                    Price = s.Price,
                                                    ServiceCount = item.Count,
                                                    ImagePath = s.ServiceImages.FirstOrDefault(i => i.IsActive).Path
                                                }).FirstOrDefault();
                basketItemVMs.Add(basketItemVM);
            }

            return View(basketItemVMs);
        }


        public IActionResult AddBasket(int Id) {
            List<BasketVm> basketVms1;
           
            if (Request.Cookies[COOKIES_BASKET] != null)
            {
                basketVms1 = JsonConvert.DeserializeObject<List<BasketVm>>(Request.Cookies[COOKIES_BASKET]);
            }
            else
            {
                basketVms1 = new List<BasketVm> { };
            }
            BasketVm cookiesBasket = basketVms1.Where(s => s.ServiceId == Id).FirstOrDefault();
            if (cookiesBasket != null)
            {
                cookiesBasket.Count++;
            }
            else
            {
                BasketVm basketVm = new BasketVm() { ServiceId = Id, Count = 1 };
                basketVms1.Add(basketVm);
            }

            //List<BasketVm> basketVms = new List<BasketVm>();

            Response.Cookies.Append(COOKIES_BASKET, JsonConvert.SerializeObject(basketVms1.OrderBy(s => s.ServiceId)));
            return RedirectToAction("Index", "Services");
        }
    }
}
