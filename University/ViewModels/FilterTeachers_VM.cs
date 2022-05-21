using Microsoft.AspNetCore.Mvc.Rendering;
using University.Models;

namespace University.ViewModels
{
    public class FilterTeachers_VM
    {
        public IEnumerable<Teacher> Teachers { get; set; }
        public string searchIme { get; set; }
        public string searchPrezime { get; set; }
        public string searchDegree { get; set; }
        public SelectList DegreeList { get; set; }
        public string searchRank { get; set; }
        public SelectList RankList { get; set; }
    }
}
