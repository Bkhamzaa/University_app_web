using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University_web_app.Data;
using University_web_app.Models;
using University_web_app.Repositories;
using University_web_app.Service;

namespace University_web_app.Controllers
{
    public class ExamController : Controller
    {
        private readonly ExamRepository _examRepository;
        private readonly EmailService _emailService;

        public ExamController(ExamRepository examRepository, EmailService emailService)
        {
            _examRepository = examRepository;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            var programs = await _examRepository.GetProgramsAsync();
            ViewData["Programs"] = programs;
            return View();
        }

        public async Task<JsonResult> GetLevelsByProgram(Guid programId)
        {
            var levels = await _examRepository.GetLevelsByProgramAsync(programId);
            return Json(levels);
        }

        public async Task<JsonResult> GetSubjectsByLevel(Guid levelId)
        {
            var subjects = await _examRepository.GetSubjectsByLevelAsync(levelId);
            return Json(subjects);
        }

        public async Task<JsonResult> GetExamsBySubject(Guid subjectId)
        {
            var exams = await _examRepository.GetExamsBySubjectAsync(subjectId);
            return Json(exams);
        }

        public async Task<IActionResult> SendEmail(Guid Id, string Subject, string ds, string finalExam)
        {
            if (Id == Guid.Empty)
                return BadRequest("Id is required.");

            var student = await _examRepository.GetStudentByIdAsync(Id);
            if (student == null || string.IsNullOrWhiteSpace(student.Email))
                return NotFound("Student not found or email missing.");

            var subjectLine = $"Your Exam Results: {Subject}";
            var messageBody = $@"
Dear {student.FirstName},

Here are your exam results for the subject: {Subject}

Midterm (DS): {(string.IsNullOrWhiteSpace(ds) ? "N/A" : ds)}
Final Exam: {(string.IsNullOrWhiteSpace(finalExam) ? "N/A" : finalExam)}

Best regards,
University Exam Office
";

            await _emailService.SendEmailAsync(student.Email, subjectLine, messageBody);
            TempData["Success"] = $"Email sent to {student.Email}";
            return RedirectToAction("Index");
        }



    }
}
