using Microsoft.EntityFrameworkCore;
using University_web_app.Data;
using University_web_app.Models;

namespace University_web_app.Repositories
{
    public class StudentRepository
    {
        private readonly UniversityContext _context;

        public StudentRepository(UniversityContext context)
        {
            _context = context;
        }

        public async Task<List<ProgramUniv>> GetProgramsAsync()
        {
            return await _context.ProgramUnivs.ToListAsync();
        }

        public async Task<List<Level>> GetLevelsByProgramAsync(Guid programId)
        {
            return await _context.Levels
                .Where(l => l.ProgramId == programId)
                .ToListAsync();
        }

        public async Task<List<Student>> GetStudentsByLevelAsync(Guid levelId)
        {
            return await _context.Students
                .Where(s => s.LevelId == levelId)
                .ToListAsync();
        }

        public async Task<Student?> GetStudentByIdAsync(Guid id)
        {
            return await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddStudentAsync(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStudentAsync(Guid id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateStudentAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

    }
}
