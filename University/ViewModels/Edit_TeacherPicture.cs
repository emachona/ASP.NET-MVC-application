﻿using System.ComponentModel.DataAnnotations;
using University.Models;

namespace University.ViewModels
{
    public class Edit_TeacherPicture
    {
        public Teacher? Teacher { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile? ProfileImage { get; set; }

        [Display(Name ="Teacher Name")]
        public string? Desc { get; set; }
    }
}
