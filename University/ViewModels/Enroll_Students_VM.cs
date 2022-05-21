using Microsoft.AspNetCore.Mvc.Rendering;
using University.Models;

namespace University.ViewModels
{
    public class Enroll_Students_VM
    {
        public Course Course { get; set; }
        public IEnumerable<long>? selectedStudents { get; set; }
        public IEnumerable<SelectListItem>? EnrolledStudents { get; set; }
        public int? Year { get; set; }
    }
}
