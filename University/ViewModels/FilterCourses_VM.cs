using Microsoft.AspNetCore.Mvc.Rendering;
using University.Models;

namespace University.ViewModels
{
    public class FilterCourses_VM
    {
        public IList<Course> Courses { get; set; }

        public string searchTitle { get; set; }
        public SelectList SemsList { get; set; }
        public int searchSem { get; set; }
        public string searchProg { get; set; }
        public SelectList Programmes { get; set; }
    }
}
