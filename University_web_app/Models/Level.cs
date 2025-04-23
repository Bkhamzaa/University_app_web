namespace University_web_app.Models
{
    public class Level
    {
        public Guid Id { get; set; } = Guid.NewGuid();  
        public string Name { get; set; }

        public Guid ProgramId { get; set; }
        public ProgramUniv Program { get; set; }

        // Navigation properties
        public ICollection<Student> Students { get; set; }
        public ICollection<Subject> Subjects { get; set; }
    }
}
