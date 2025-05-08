using Microsoft.EntityFrameworkCore;
using University_web_app.Data;
using University_web_app.Models;

namespace University_web_app.Repositories
{
    public class ExamRepository
    {
        private readonly UniversityContext _context;

        public ExamRepository(UniversityContext context)
        {
            _context = context;
        }

        public async Task<List<ProgramUniv>> GetProgramsAsync()
        {
            return await _context.ProgramUnivs.ToListAsync();
        }

        public async Task<List<Level>> GetLevelsByProgramAsync(Guid programId)
        {
            return await _context.Levels.Where(l => l.ProgramId == programId).ToListAsync();
        }

        public async Task<List<Subject>> GetSubjectsByLevelAsync(Guid levelId)
        {
            return await _context.Subjects.Where(s => s.LevelId == levelId).ToListAsync();
        }

        public async Task<List<Exam>> GetExamsBySubjectAsync(Guid subjectId)
        {
            return await _context.Exams
                .Where(e => e.SubjectId == subjectId)
                .Include(e => e.Student)
                .Include(e => e.Subject)
                .ToListAsync();
        }

        public async Task<Student?> GetStudentByIdAsync(Guid id)
        {
            return await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Student?> GetStudentByCinAsync(string cinId)
        {
            return await _context.Students.FirstOrDefaultAsync(s => s.CinId == cinId);
        }

        public async Task<Subject?> GetSubjectByIdAsync(Guid id)
        {
            return await _context.Subjects.FindAsync(id);
        }

        public async Task AddExamsAsync(List<Exam> exams)
        {
            _context.Exams.AddRange(exams);
            await _context.SaveChangesAsync();
        }

    }
}
