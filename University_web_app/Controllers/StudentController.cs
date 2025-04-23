using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using University_web_app.Data;
using University_web_app.Models;

namespace University_web_app.Controllers
{
    public class StudentController : Controller
    {
        private readonly UniversityContext _context;

        public StudentController(UniversityContext context)
        {
            _context = context;
        }
     
        public async Task<IActionResult> index()
        {
            var programs = await _context.ProgramUnivs.ToListAsync();
            ViewData["Programs"] = programs;  // Passing data to the view
            return View();
        }

        // Get levels based on the selected program
        public async Task<JsonResult> GetLevelsByProgram(Guid programId)
        {
            var levels = await _context.Levels
                                       .Where(l => l.ProgramId == programId)
                                       .ToListAsync();
            return Json(levels);
        }

        // Get students based on the selected level
        public async Task<JsonResult> GetStudentsByLevel(Guid levelId)
        {
            var students = await _context.Students
                                          .Where(s => s.LevelId == levelId)
                                          .ToListAsync();
            return Json(students);
        }



       
        [HttpPost]
        public IActionResult AddStudent(Student student)
        {
            if (!string.IsNullOrEmpty(student.CinId) &&
                !string.IsNullOrEmpty(student.FirstName) &&
                !string.IsNullOrEmpty(student.LastName) &&
                !string.IsNullOrEmpty(student.Email))
            {
                _context.Students.Add(student);
                _context.SaveChanges();
                return Ok("Student added successfully.");
            }

            // Return 400 Bad Request with a message
            return BadRequest("All fields are required.");
        }



        public async Task<IActionResult> DeleteStudent(Guid id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound(new { message = "Student not found." });
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Student deleted successfully." });
        }


        [HttpGet]
        [Route("student/UpdateStudent/{id}")]
        public IActionResult UpdateStudent ( Guid id)
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == id);

            return View(student);

        }

        [HttpPost]
        public async Task<IActionResult> Update(Student student)
        {
            if (string.IsNullOrEmpty(student.CinId) ||
                string.IsNullOrEmpty(student.FirstName) ||
                string.IsNullOrEmpty(student.LastName) ||
                string.IsNullOrEmpty(student.Email))
            {
                ViewBag.ErrorMessage = "All fields are required.";
                return View("UpdateStudent", student);
            }

            var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.Id == student.Id);
            if (existingStudent == null)
            {
                return NotFound();
            }

            existingStudent.FirstName = student.FirstName;
            existingStudent.LastName = student.LastName;
            existingStudent.Email = student.Email;
            existingStudent.CinId = student.CinId;

            await _context.SaveChangesAsync();

            return RedirectToAction("index");
        }



    }
}
