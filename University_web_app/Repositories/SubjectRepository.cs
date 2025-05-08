using Microsoft.EntityFrameworkCore;
using University_web_app.Data;
using University_web_app.Models;

namespace University_web_app.Repositories
{
    public class SubjectRepository
    {
        private readonly UniversityContext _context;

        public SubjectRepository(UniversityContext context)
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

        public async Task<List<Subject>> GetSubjectsByLevelAsync(Guid levelId)
        {
            return await _context.Subjects
                .Where(s => s.LevelId == levelId)
                .ToListAsync();
        }

        public async Task AddSubjectAsync(Subject subject)
        {
            subject.Level = await _context.Levels.FindAsync(subject.LevelId);
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();
        }

        public async Task<Subject?> GetSubjectByIdAsync(Guid id)
        {
            return await _context.Subjects.FindAsync(id);
        }

        public async Task DeleteSubjectAsync(Subject subject)
        {
            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSubjectAsync(Subject subject)
        {
            _context.Subjects.Update(subject);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Subject>> SearchSubjectsAsync(string term, Guid levelId)
        {
            return await _context.Subjects
                .Where(s => s.LevelId == levelId && s.Name.Contains(term))
                .ToListAsync();
        }

    }
}
