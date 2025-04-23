using Microsoft.EntityFrameworkCore;
using University_web_app.Models;

namespace University_web_app.Data
{
    public class UniversityContext : DbContext
    {
        public UniversityContext( DbContextOptions<UniversityContext> option ) :base(option) {
        
        
        
        }


        public DbSet<ProgramUniv> ProgramUnivs { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Exam> Exams { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Level>()
                .HasOne(l => l.Program)
                .WithMany(p => p.Levels)
                .HasForeignKey(l => l.ProgramId)
                .OnDelete(DeleteBehavior.Restrict); // prevent Program delete if it has Levels

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Level)
                .WithMany(l => l.Students)
                .HasForeignKey(s => s.LevelId)
                .OnDelete(DeleteBehavior.Cascade);
           

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Program)
                .WithMany(p => p.Students)
                .HasForeignKey(s => s.ProgramId)
                .OnDelete(DeleteBehavior.Restrict); // prevent Program delete if it has Students

            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Level)
                .WithMany(l => l.Subjects)
                .HasForeignKey(s => s.LevelId)
                .OnDelete(DeleteBehavior.SetNull); // prevent Level delete if it has Subjects

            modelBuilder.Entity<Exam>()
    .HasOne(e => e.Student)
    .WithMany()
    .HasForeignKey(e => e.CinId)
    .HasPrincipalKey(s => s.CinId)
    .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Exam>()
    .HasOne(e => e.Subject)
    .WithMany()
    .HasForeignKey(e => e.SubjectId)
    .OnDelete(DeleteBehavior.Cascade); // Automatically delete exams when a subject is deleted

        }






        public static class DbInitializer
        {
            public static void Seed(UniversityContext context)
            {
                // Seed ProgramUnivs if they don't exist
                if (!context.ProgramUnivs.Any())
                {
                    var masterId = Guid.NewGuid();
                    var bachelorId = Guid.NewGuid();

                    var programs = new[]
                    {
                    new ProgramUniv { Id = masterId, Name = "Master" },
                    new ProgramUniv { Id = bachelorId, Name = "Bachelors" }
                };

                    context.ProgramUnivs.AddRange(programs);

                    // Seed Levels related to Master and Bachelors Program
                    var levels = new[]
                    {
                    // Master levels
                    new Level { Name = "Mastère professionnel en Data Science and Software Development", ProgramId = masterId },
                    new Level { Name = "Mastère professionnel en E-Business", ProgramId = masterId },
                    new Level { Name = "Mastère professionnel en Veille et Intelligence compétitive", ProgramId = masterId },
                    new Level { Name = "Mastère professionnel en E-Business (à distance)", ProgramId = masterId },
                    new Level { Name = "Mastère professionnel en Contrôle de Gestion et Business Intelligence", ProgramId = masterId },

                    // Bachelor levels
                    new Level { Name = "L1", ProgramId = bachelorId },
                    new Level { Name = "L2", ProgramId = bachelorId },
                    new Level { Name = "L3", ProgramId = bachelorId }
                };

                    context.Levels.AddRange(levels);

                    // Commit changes to database
                    context.SaveChanges();
                }
            }
        }
    }
}
