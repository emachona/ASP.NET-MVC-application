using System.ComponentModel.DataAnnotations;

namespace University.Models
{
    public class Student
    {
        [Required]
        public long ID { get; set; }

        [Required]
        [StringLength(10)]
        public string StudentID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name ="Last Name")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Enrollment Date")]
        public Nullable <DateTime> EnrollmentDate { get; set; }
        
        [Display(Name = "Acquired Credits")]
        public Nullable <int> AcquiredCredits { get; set; }
       
        [Display(Name = "Current Semester")]
        public Nullable<int> CurrentSemester { get; set; }
       
        [Display(Name = "Education Level")]
        [StringLength(25)]
        public string? EducationLevel { get; set; }


        [Display(Name = "Full Name")]
        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        public ICollection<Enrollment>? Enrollments { get; set; }

        public string? Picture { get; set; }
    }
}
