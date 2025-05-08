using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using University_web_app.Data;
using University_web_app.Models;
using University_web_app.Repositories;

namespace University_web_app.Controllers
{
    public class SubjectController : Controller
    {

        private readonly SubjectRepository _repository;

        public SubjectController(SubjectRepository repository)
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

        public async Task<JsonResult> GetSubjectsByLevel(Guid levelId)
        {
            var subjects = await _repository.GetSubjectsByLevelAsync(levelId);
            return Json(subjects);
        }

        [HttpPost]
        public async Task<IActionResult> AddSubject(Subject subject)
        {
            if (!string.IsNullOrEmpty(subject.Name) &&
                !double.IsNaN(subject.Coefficient) &&
                !string.IsNullOrEmpty(subject.Semester))
            {
                await _repository.AddSubjectAsync(subject);
                return Ok("Subject added successfully.");
            }

            return BadRequest("All fields are required.");
        }

        public async Task<IActionResult> DeleteSubject(Guid id)
        {
            var subject = await _repository.GetSubjectByIdAsync(id);
            if (subject == null)
                return NotFound(new { message = "Subject not found." });

            await _repository.DeleteSubjectAsync(subject);
            return Ok(new { message = "Subject deleted successfully." });
        }

        [HttpGet("Subject/UpdateSubject/{id}")]
        public async Task<IActionResult> UpdateSubject(Guid id)
        {
            var subject = await _repository.GetSubjectByIdAsync(id);
            if (subject == null)
                return NotFound();

            return View(subject);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Subject subject)
        {
            if (string.IsNullOrEmpty(subject.Name) ||
                double.IsNaN(subject.Coefficient) ||
                string.IsNullOrEmpty(subject.Semester))
            {
                ViewBag.ErrorMessage = "All fields are required.";
                return View("UpdateSubject", subject);
            }

            var existing = await _repository.GetSubjectByIdAsync(subject.Id);
            if (existing == null)
                return NotFound();

            existing.Name = subject.Name;
            existing.Coefficient = subject.Coefficient;
            existing.Semester = subject.Semester;

            await _repository.UpdateSubjectAsync(existing);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> SearchSubjects(string term, Guid levelId)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<object>());

            var results = await _repository.SearchSubjectsAsync(term, levelId);
            return Json(results);
        }




    }
}
