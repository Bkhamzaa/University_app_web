namespace University_web_app.Models
{
    public class Subject
    {
        public Guid Id { get; set; } = Guid.NewGuid();  // GUID as Primary Key
        public string Name { get; set; }
        public float Coefficient { get; set; }
        public string Semester { get; set; }

        public Guid? LevelId { get; set; }
        public Level Level { get; set; }
    }
}
