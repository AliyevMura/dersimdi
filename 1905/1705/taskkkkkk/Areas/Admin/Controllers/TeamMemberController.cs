using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.IO;
using WebFrontToBack.Areas.Admin.ViewModel;
using WebFrontToBack.DAL;
using WebFrontToBack.Models;
using WebFrontToBack.Utilities.Constants;
using WebFrontToBack.Utilities.Extensions;

namespace WebFrontToBack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TeamMemberController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public object?[]? Id { get; private set; }

        public TeamMemberController(AppDbContext context,IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            ICollection<TeamMember> teamMembers = await _context.TeamMembers.ToListAsync();
            return View(teamMembers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTeamMemberVM member)
        {


            if (!ModelState.IsValid)
            {
                return View();
            }
            if (!member.Photo.CheckContentType("image/"))
            {
                ModelState.AddModelError("Photo", $"{member.Photo.FileName} {Messages.FileTypeMustBeImage}");
                return View();
            }
            if (!member.Photo.CheckFileSize(200))
            {
                ModelState.AddModelError("Photo", $"{member.Photo.FileName} - file type must be size less than 200kb");
                return View();
            }
            string root = _webHostEnvironment.WebRootPath;
            string filename = Guid.NewGuid().ToString()+ member.Photo.FileName;
            string resultPath = Path.Combine(root, "assets", "img",filename);


            using (FileStream fileStream = new FileStream(resultPath, FileMode.Create))
            {
              await member.Photo.CopyToAsync(fileStream);
            }

            TeamMember teamMember = new TeamMember()
            {

                Name= member.Name,
                Image=filename,
                Profession = member.Profession
            };
            await _context.TeamMembers.AddAsync(teamMember);
            await _context.SaveChangesAsync();
          
            return RedirectToAction(nameof(Index));

            //return Json(Environment.CurrentDirectory);

            //return Json($"{teamMember.Photo.FileName} {teamMember.Photo.ContentType} {teamMember.Photo.Length / 1024}");
            return Json(filename);

            //bool isExists = await _context.TeamMembers.AnyAsync(c =>
            //c.Name.ToLower().Trim() == teamMember.Name.ToLower().Trim());


            //if (isExists)
            //{
            //    ModelState.AddModelError("Name", "Team member name already exists");
            //    return View();
            //}
            //await _context.TeamMembers.AddAsync(teamMember);
            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int Id)
        {
            TeamMember? teamMember = _context.TeamMembers.Find(Id);

            if (teamMember == null)
            {
                return NotFound();
            }

            return View(teamMember);
        }

        [HttpPost]
        public IActionResult Update(TeamMember teamMember)
        {
            TeamMember? editedTeamMember = _context.TeamMembers.Find(teamMember.Id);
            if (editedTeamMember == null)
            {
                return NotFound();
            }
            editedTeamMember.Name = teamMember.Name;
            editedTeamMember.Image = teamMember.Image;
            editedTeamMember.Profession = teamMember.Profession;
            _context.TeamMembers.Update(editedTeamMember);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        //public IActionResult Delete(int Id)
        //{
        //    TeamMember? teamMember = _context.TeamMembers.Find(Id);
        //    if (teamMember == null)
        //    {
        //        return NotFound();
        //    }
        //    _context.TeamMembers.Remove(teamMember);
        //    _context.SaveChanges();
        //    return RedirectToAction(nameof(Index));
        //}
        public async Task<IActionResult> Delete(int id)
        {
            TeamMember? teamMember = _context.TeamMembers.Find(Id);
            if (teamMember == null)
            {
                return NotFound();
            }
            _context.TeamMembers.Remove(teamMember);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
            TeamMember teamMember1 = await _context.TeamMembers.FindAsync(id);
            if (teamMember == null) return NotFound();
            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", teamMember.Image);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _context.TeamMembers.Remove(teamMember);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



    }
}
