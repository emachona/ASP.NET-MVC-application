#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using University.Data;
using University.Models;
using University.ViewModels;

namespace University.Controllers
{
    public class TeachersController : Controller
    {
        private readonly UniversityContext _context;

        public TeachersController(UniversityContext context)
        {
            _context = context;
        }

        // GET: Teachers
        public async Task<IActionResult> Index(string searchIme, string searchPrezime, string searchDegree, string searchRank)
        {
            IQueryable<Teacher> teachers = _context.Teachers.AsQueryable();
            IQueryable<string> query = _context.Teachers.OrderBy(m => m.Degree).Select(m => m.Degree).Distinct();
            IQueryable<string> query1 = _context.Teachers.OrderBy(m => m.AcademicRank).Select(m => m.AcademicRank).Distinct();

            if (!string.IsNullOrEmpty(searchDegree))
            {
                teachers = teachers.Where(s => s.Degree.ToLower() == searchDegree.ToLower());
            }
            if (!string.IsNullOrEmpty(searchRank))
            {
                teachers = teachers.Where(s => s.AcademicRank.ToLower() == searchRank.ToLower());
            }
            if (!string.IsNullOrEmpty(searchIme))
            {
                teachers = teachers.Where(s => s.FirstName.ToLower().Contains(searchIme.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchPrezime))
            {
                teachers = teachers.Where(s => s.LastName.ToLower().Contains(searchPrezime.ToLower()));
            }

            var VM1 = new FilterTeachers_VM
            {
                DegreeList = new SelectList(query.AsEnumerable()),
                RankList = new SelectList(query1.AsEnumerable()),
                Teachers = teachers.AsEnumerable()
            };


            return View(VM1);
        }

        public IActionResult Courses(int? id)
        {
            IEnumerable<Course> courses = (IEnumerable<Course>)_context.Courses
                .Where(m => m.FirstTeacherID == id || m.SecondTeacherID == id);

            ViewData["Teacher"] = _context.Teachers.Where(s => s.TeacherID == id).Select(s => s.FullName).FirstOrDefault();

            return View(courses);
        }

        // GET: Teachers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(m => m.TeacherID == id);
            if (teacher == null)
            {
                return NotFound();
            }

            Edit_TeacherPicture viewmodel = new Edit_TeacherPicture
            {
                Teacher = teacher,
                Desc = teacher.ProfilePicture
            };

            return View(viewmodel);
        }

        // GET: Teachers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TeacherID,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate,ProfilePicture")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TeacherID,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate,ProfilePicture")] Teacher teacher)
        {
            if (id != teacher.TeacherID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.TeacherID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(m => m.TeacherID == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.TeacherID == id);
        }

        public async Task<IActionResult> EditPicture(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = _context.Teachers.Where(x => x.TeacherID == id).First();
            if (teacher == null)
            {
                return NotFound();
            }

            Edit_TeacherPicture viewmodel = new Edit_TeacherPicture
            {
                Teacher = teacher,
                Desc = teacher.ProfilePicture   
            };

            return View(viewmodel);
        }

        // POST: Teachers/EditPicture/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPicture(long id, Edit_TeacherPicture viewmodel)
        {
            if (id != viewmodel.Teacher.TeacherID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (viewmodel.ProfileImage != null)
                    {
                        string uniqueFileName = UploadedFile(viewmodel);
                        viewmodel.Teacher.ProfilePicture = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.Teacher.ProfilePicture = viewmodel.Desc;
                    }

                    _context.Update(viewmodel.Teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(viewmodel.Teacher.TeacherID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = viewmodel.Teacher.TeacherID });
            }
            return View(viewmodel);
        }
        private string UploadedFile(Edit_TeacherPicture viewmodel)
        {
            string uniqueFileName = null;

            if (viewmodel.ProfileImage != null)
            {
                //string uploadPath = Server.MapPath("~/images");
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(viewmodel.ProfileImage.FileName);
                string fileNameWithPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create)) 
                {
                    viewmodel.ProfileImage.CopyTo(stream);
                }
            }
            return uniqueFileName;
        }
    }
}
