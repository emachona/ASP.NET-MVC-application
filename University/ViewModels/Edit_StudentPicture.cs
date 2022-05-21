using System.ComponentModel.DataAnnotations;
using University.Models;

namespace University.ViewModels
{
    public class Edit_StudentPicture
    {
        public Student? Student { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile? ProfileImage { get; set; }

        [Display(Name = "Student Name")]
        public string? Desc { get; set; }
    }
}
