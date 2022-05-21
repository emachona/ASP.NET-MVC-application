using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University.Models
{
    public class Enrollment
    {
        public long EnrollmentID { get; set; }

        [Required]
        public int CourseID { get; set; }
        public Course? Course { get; set; }

        [Required]
        public long StudentID { get; set; }
        public Student? Student { get; set; }

        [StringLength(10)]
        public string? Semester { get; set; }

        public int? Year { get; set; }
        public int? Grade { get; set; }

        [StringLength(255)]
        [Display(Name = "Seminal Url")]
        public string? SeminalUrl { get; set; }

        [Display(Name = "Project Url")]
        [StringLength(255)]
        public string? ProjectUrl { get; set; }

        [Display(Name = "Exam Points")]
        public int? ExamPoints { get; set; }

        [Display(Name = "Seminal Points")]
        public int? SeminalPoints { get; set; }

        [Display(Name = "Project Points")]
        public int? ProjectPoints { get; set; }

        [Display(Name = "Additional Points")]
        public int? AdditionalPoints { get; set; }

        [Display(Name = "Finish Date")]
        public Nullable<DateTime> FinishDate { get; set; }


        [Display(Name = "Total Points")]
        public int TotalPoints
        {
            get
            {
                if (ExamPoints == null)
                    ExamPoints = 0;
                if (SeminalPoints == null)
                    SeminalPoints = 0;
                if (ProjectPoints == null)
                    ProjectPoints = 0;
                if (AdditionalPoints == null)
                    AdditionalPoints = 0;
                int points = (int)(ExamPoints + SeminalPoints + ProjectPoints + AdditionalPoints);
                return (int)points;
            }
        }
    }
}
