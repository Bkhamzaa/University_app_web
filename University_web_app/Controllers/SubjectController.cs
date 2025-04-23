using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using University_web_app.Data;
using University_web_app.Models;

namespace University_web_app.Controllers
{
    public class SubjectController : Controller
    {


        private readonly UniversityContext _context;

        public SubjectController(UniversityContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var programs = await _context.ProgramUnivs.ToListAsync();
            ViewData["Programs"] = programs;  // Passing data to the view
            return View();
        }



        public async Task<JsonResult> GetLevelsByProgram(Guid programId)
        {
            var levels = await _context.Levels
                                       .Where(l => l.ProgramId == programId)
                                       .ToListAsync();
            return Json(levels);
        }


        public async Task<JsonResult> GetSubjectsByLevel(Guid levelId)
        {
            var subjects = await _context.Subjects
                                          .Where(s => s.LevelId == levelId)
                                          .ToListAsync();
            return Json(subjects);
        }


        [HttpPost]
        public async Task<IActionResult> AddSubject(Subject subject)
        {
            subject.Level = await _context.Levels.FindAsync(subject.LevelId);

            if (!string.IsNullOrEmpty(subject.Name) &&
               !double.IsNaN(subject.Coefficient)   &&
               !string.IsNullOrEmpty(subject.Semester))
            {
               
                _context.Subjects.Add(subject);
                await _context.SaveChangesAsync();
                return Ok("Subject added successfully.");
            }

            return BadRequest("All fields are required.");


        }

       // [HttpDelete]
        //[Route("Subject/DeleteSubject/{id}")]
        public async Task<IActionResult> DeleteSubject(Guid id)
        {
            var Subject = await _context.Subjects.FindAsync(id);
            if (Subject == null)
            {
                return NotFound(new { message = "Subject not found." });
            }

            _context.Subjects.Remove(Subject);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Subject deleted successfully." });
        }


        [HttpGet]
        [Route("Subject/UpdateSubject/{id}")]
        public IActionResult UpdateSubject(Guid id)
        {
            var subject = _context.Subjects.FirstOrDefault(s => s.Id == id);

            return View(subject);

        }


          [HttpPost]
        public async Task<IActionResult> Update(Subject subject)
        {
            if (string.IsNullOrEmpty(subject.Name) &&
                 double.IsNaN(subject.Coefficient) &&
                 string.IsNullOrEmpty(subject.Semester))
            {
                ViewBag.ErrorMessage = "All fields are required.";
                return View("UpdateSubject", subject);
            }

            var existingSubject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == subject.Id);
            if (existingSubject == null)
            {
                return NotFound();
            }

            existingSubject.Name = subject.Name;
            existingSubject.Coefficient = subject.Coefficient;
            existingSubject.Semester = subject.Semester;
        

            await _context.SaveChangesAsync();

            return RedirectToAction("index");
        }
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> SearchSubjects(string term, Guid levelId)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<object>());

            var results = await _context.Subjects
                .Where(s => s.LevelId == levelId && s.Name.Contains(term))
                .ToListAsync();

            return Json(results);
        }




    }
}
