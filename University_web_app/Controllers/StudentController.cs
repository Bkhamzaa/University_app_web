using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using University_web_app.Data;
using University_web_app.Models;
using University_web_app.Repositories;

namespace University_web_app.Controllers
{
    public class StudentController : Controller
    {
        private readonly StudentRepository _repository;

        public StudentController(StudentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var programs = await _repository.GetProgramsAsync();
            ViewData["Programs"] = programs;
            return View();
        }

        public async Task<JsonResult> GetLevelsByProgram(Guid programId)
        {
            var levels = await _repository.GetLevelsByProgramAsync(programId);
            return Json(levels);
        }

        public async Task<JsonResult> GetStudentsByLevel(Guid levelId)
        {
            var students = await _repository.GetStudentsByLevelAsync(levelId);
            return Json(students);
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent(Student student)
        {
            if (string.IsNullOrEmpty(student.CinId) ||
                string.IsNullOrEmpty(student.FirstName) ||
                string.IsNullOrEmpty(student.LastName) ||
                string.IsNullOrEmpty(student.Email))
            {
                return BadRequest("All fields are required.");
            }

            await _repository.AddStudentAsync(student);
            return Ok("Student added successfully.");
        }

        public async Task<IActionResult> DeleteStudent(Guid id)
        {
            var student = await _repository.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound(new { message = "Student not found." });

            await _repository.DeleteStudentAsync(id);
            return Ok(new { message = "Student deleted successfully." });
        }

        [HttpGet("student/UpdateStudent/{id}")]
        public async Task<IActionResult> UpdateStudent(Guid id)
        {
            var student = await _repository.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound();

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

            var existing = await _repository.GetStudentByIdAsync(student.Id);
            if (existing == null)
                return NotFound();

            existing.FirstName = student.FirstName;
            existing.LastName = student.LastName;
            existing.Email = student.Email;
            existing.CinId = student.CinId;

            await _repository.UpdateStudentAsync(existing);
            return RedirectToAction("Index");
        }


    }
}
