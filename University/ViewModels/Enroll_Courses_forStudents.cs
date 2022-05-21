using Microsoft.AspNetCore.Mvc.Rendering;
using University.Models;

namespace University.ViewModels
{
    public class Enroll_Courses_forStudents
    {
        public Student Student { get; set; }
        public IEnumerable<int>? selectedCourses { get; set; }
        public IEnumerable<SelectListItem>? EnrolledCourses { get; set; }
        public int? year { get; set; }
        public string? semester { get; set; }
    }
}
