using Microsoft.AspNetCore.Mvc;
using University_web_app.Repositories;

namespace University_web_app.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DashboardRepository _dashboardRepository;

        public DashboardController(DashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.StudentCount = await _dashboardRepository.GetStudentCountAsync();
            ViewBag.ExamCount = await _dashboardRepository.GetExamCountAsync();
            ViewBag.SubjectCount = await _dashboardRepository.GetSubjectCountAsync();
            ViewBag.StudentsPerLevel = await _dashboardRepository.GetStudentsPerLevelAsync();

            return View();
        }
    }
}
