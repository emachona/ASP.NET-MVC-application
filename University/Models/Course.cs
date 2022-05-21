using System.ComponentModel.DataAnnotations;

namespace University.Models
{
    public class Course
    {
        [Required]
        public int CourseID { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public int Credits { get; set; }
        public int Semester { get; set; }

        [StringLength(100)]
        public string? Programme { get; set; }

        [Display(Name = "Education Level")]
        [StringLength(25)]
        public string? EducationLevel { get; set; }

        public int? FirstTeacherID { get; set; }
        //[ForeignKey("FirstTeacherID")]
        [Display(Name = "First Teacher")]
        public Teacher? FirstTeacher { get; set; }
 
        public int? SecondTeacherID { get; set; }
        //[ForeignKey("SecondTeacherID")]
        [Display(Name = "Second Teacher")]
        public Teacher? SecondTeacher { get; set; }

        public ICollection<Enrollment>? Enrollments { get; set; }

    }
}
