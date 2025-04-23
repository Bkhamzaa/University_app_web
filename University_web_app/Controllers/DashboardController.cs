using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University_web_app.Data;

namespace University_web_app.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UniversityContext _context;

        public DashboardController(UniversityContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var studentCount = await _context.Students.CountAsync();
            var examCount = await _context.Exams.CountAsync();
            var subjectCount = await _context.Subjects.CountAsync();

            var studentsPerLevel = await _context.Levels
                .Include(l => l.Students)
                .ToDictionaryAsync(l => l.Name, l => l.Students.Count);

            ViewBag.StudentCount = studentCount;
            ViewBag.ExamCount = examCount;
            ViewBag.SubjectCount = subjectCount;
            ViewBag.StudentsPerLevel = studentsPerLevel;

            return View();
        }
    }
}
