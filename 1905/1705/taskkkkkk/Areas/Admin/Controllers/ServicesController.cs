using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using WebFrontToBack.Areas.Admin.ViewModel;
using WebFrontToBack.DAL;
using WebFrontToBack.Models;
using WebFrontToBack.Utilities.Constants;
using WebFrontToBack.Utilities.Extensions;

namespace WebFrontToBack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServicesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly List<Category>  _categories;
        private readonly IWebHostEnvironment _enviroment;
           private string _errorMessages;
      

        public ServicesController(AppDbContext context, IWebHostEnvironment enviroment)
        {
            _context = context;
            _categories = _context.Categories.ToList();
            _enviroment = enviroment;

        }

        public async Task<IActionResult> Index()
        {
            return View(
                await _context.Services
                .Where(s => !s.IsDeleted)
                .OrderByDescending(s => s.Id)
                .Take(3)
                .Include(s => s.Category)
                .Include(s => s.ServiceImages)
               .ToListAsync()
                );
        }
       
        
        public async Task<IActionResult> Create()
        {
            CreateServiceVM createServiceVM = new CreateServiceVM()
            {
                Categories = await _context.Categories.Where(c => !c.IsDeleted).ToListAsync()
            };
            return View(createServiceVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateServiceVM createServiceVM)
        {
            createServiceVM.Categories = _categories;
            if (!ModelState.IsValid) return View();
            foreach (var photo in createServiceVM.Photos) {
                if (!photo.CheckContentType("image/"))
                {

                    ModelState.AddModelError("Photos", $"{photo.FileName} * {Messages.FileTypeMustBeImage}");
                    return View(createServiceVM);


                }
                if (!photo.CheckFileSize(200))
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName} * {Messages.FileSizeMustBe200KB}");
                    return View(createServiceVM);
                }
            }
            List<ServiceImage> images = new List<ServiceImage>();
            foreach (var photo in createServiceVM.Photos)
            {
                string rootPath = Path.Combine(_enviroment.WebRootPath, "assets", "img");
                string filename = await photo.SaveAsync(rootPath);
                ServiceImage serviceImage = new ServiceImage() { Path = filename };
                if (!images.Any(i => i.IsActive))
                {
                    serviceImage.IsActive = true;
                }
                images.Add(serviceImage);
            }
            Service service = new Service()
            {
                Name = createServiceVM.Name,
                CategoryId = createServiceVM.CategoryId,
                Description = createServiceVM.Description,
                Price = createServiceVM.Price,
                ServiceImages = images,
            };
            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }
    }
}
