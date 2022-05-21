using System.ComponentModel.DataAnnotations;
using University.Models;

namespace University.ViewModels
{
    public class EditAsStudent_VM
    {
        public Enrollment enrollment { get; set; }

        [Display(Name = "Seminal File")]
        public IFormFile? seminalUrlFile { get; set; }

        public string? seminalUrlName { get; set; }
    }
}
