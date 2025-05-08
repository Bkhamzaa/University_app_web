using Microsoft.EntityFrameworkCore;
using University_web_app.Data;

namespace University_web_app.Repositories
{
    public class DashboardRepository
    {
        private readonly UniversityContext _context;

        public DashboardRepository(UniversityContext context)
        {
            _context = context;
        }

        public async Task<int> GetStudentCountAsync() =>
            await _context.Students.CountAsync();

        public async Task<int> GetExamCountAsync() =>
            await _context.Exams.CountAsync();

        public async Task<int> GetSubjectCountAsync() =>
            await _context.Subjects.CountAsync();

        public async Task<Dictionary<string, int>> GetStudentsPerLevelAsync()
        {
            return await _context.Levels
                .Include(l => l.Students)
                .ToDictionaryAsync(l => l.Name, l => l.Students.Count);
        }

    }
}
