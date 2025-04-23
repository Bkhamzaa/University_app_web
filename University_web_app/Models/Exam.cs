using System.ComponentModel.DataAnnotations;

namespace University_web_app.Models
{
    public class Exam
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string CinId { get; set; } // link to Student

        [Required]
        public Guid SubjectId { get; set; } // link to Subject

        public double? DS { get; set; } // midterm, optional
        public double? FinalExam { get; set; } // final, optional

        // Navigation properties
        public Student Student { get; set; }
        public Subject Subject { get; set; }
    }

}