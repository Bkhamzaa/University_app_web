using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University_web_app.Data;
using University_web_app.Models;
using University_web_app.Service;

namespace University_web_app.Controllers
{
    public class ExamController : Controller
    {

        private readonly UniversityContext _context;
        private readonly EmailService _emailService;


        public ExamController(UniversityContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;

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

        public async Task<JsonResult> GetExamsBySubject(Guid subjectId)
        {
            var exams = await _context.Exams
                                      .Where(e => e.SubjectId == subjectId)
                                      .Include(e => e.Student) // Include student details if needed
                                      .Include(e => e.Subject) // Include subject details if needed
                                      .ToListAsync();

            return Json(exams);
        }



        [HttpPost]
        public async Task<IActionResult> SendEmail(Guid Id, string Subject, string ds, string finalExam)
        {
            if (Id == Guid.Empty)
                return BadRequest("Id is required.");

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == Id);
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

            await _emailService.SendEmailAsync(
                student.Email,
                subjectLine,
                messageBody
            );

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

            // Skip the header line
            await reader.ReadLineAsync();

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                var values = line.Split(',');

                if (values.Length < 4)
                    continue; // Not enough data

                string cinId = values[0].Trim();
                string subjectIdStr = values[1].Trim();
                string dsStr = values[2].Trim();
                string finalStr = values[3].Trim();

                if (!Guid.TryParse(subjectIdStr, out Guid subjectId))
                    continue; // Invalid subject ID

                var exam = new Exam
                {
                    Id = Guid.NewGuid(),
                    CinId = cinId,
                    SubjectId = subjectId,
                    DS = double.TryParse(dsStr, out var dsVal) ? dsVal : null,
                    FinalExam = double.TryParse(finalStr, out var finalVal) ? finalVal : null,
                    Student = await _context.Students.FirstOrDefaultAsync(s => s.CinId == cinId),

                    Subject = await _context.Subjects.FindAsync(subjectId)


                };

                exams.Add(exam);
            }

            if (exams.Any())
            {
                _context.Exams.AddRange(exams);
                await _context.SaveChangesAsync();
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
