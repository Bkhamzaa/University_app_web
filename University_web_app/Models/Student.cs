namespace University_web_app.Models
{
    public class Student
    {
        public Guid Id { get; set; } = Guid.NewGuid();  // GUID as Primary Key
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CinId { get; set; }

        public Guid ProgramId { get; set; }
        public ProgramUniv Program { get; set; }

        public Guid LevelId { get; set; }
        public Level Level { get; set; }
    }
}
