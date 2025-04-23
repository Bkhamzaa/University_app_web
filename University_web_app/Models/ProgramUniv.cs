namespace University_web_app.Models
{
    public class ProgramUniv
    {
        public Guid Id { get; set; } = Guid.NewGuid(); 
        public string Name { get; set; }

        // Navigation
        public ICollection<Level> Levels { get; set; }
        public ICollection<Student> Students { get; set; }
    }
}
