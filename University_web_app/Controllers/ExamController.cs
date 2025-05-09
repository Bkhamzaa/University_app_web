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






        [HttpPost]
        public async Task<IActionResult> ImportCsv(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                TempData["Error"] = "Please select a valid CSV file.";
                return RedirectToAction("Index");
            }

            using var reader = new StreamReader(csvFile.OpenReadStream());
            var exams = new List<Exam>();

            await reader.ReadLineAsync(); // Skip header

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                var values = line.Split(',');

                if (values.Length < 4)
                    continue;

                string cinId = values[0].Trim();
                string subjectIdStr = values[1].Trim();
                string dsStr = values[2].Trim();
                string finalStr = values[3].Trim();

                if (!Guid.TryParse(subjectIdStr, out Guid subjectId))
                    continue;

                var student = await _examRepository.GetStudentByCinAsync(cinId);
                var subject = await _examRepository.GetSubjectByIdAsync(subjectId);

                if (student == null || subject == null)
                    continue;

                var exam = new Exam
                {
                    Id = Guid.NewGuid(),
                    CinId = cinId,
                    SubjectId = subjectId,
                    DS = double.TryParse(dsStr, out var dsVal) ? dsVal : null,
                    FinalExam = double.TryParse(finalStr, out var finalVal) ? finalVal : null,
                    Student = student,
                    Subject = subject
                };

                exams.Add(exam);
            }

            if (exams.Any())
            {
                await _examRepository.AddExamsAsync(exams);
                TempData["Success"] = $"{exams.Count} exams imported successfully.";
            }
            else
            {
                TempData["Warning"] = "No valid data found in the CSV.";
            }

            return RedirectToAction("Index");
        }

    }
}
