using University.Models;

namespace University.ViewModels
{
    public class FilterStudents_VM
    {
        public IEnumerable<Student> Students { get; set; }
        public string searchStudentId { get; set; }
        public string searchIme { get; set; }
    }
}
