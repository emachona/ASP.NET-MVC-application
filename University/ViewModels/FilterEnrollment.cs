using Microsoft.AspNetCore.Mvc.Rendering;
using University.Models;

namespace University.ViewModels
{
    public class FilterEnrollment
    {
        public IList<Enrollment> Enrollments { get; set; }
        public SelectList listYears { get; set; }
        public int Year { get; set; }
    }
}
