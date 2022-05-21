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
    public class CoursesController : Controller
    {
        private readonly UniversityContext _context;

        public CoursesController(UniversityContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index(string searchTitle, int searchSem, string searchProg)
        {
            IQueryable<Course> courses = _context.Courses.AsQueryable();
            IQueryable<int> query = _context.Courses.OrderBy(m => m.Semester).Select(m => m.Semester).Distinct();

            if (!string.IsNullOrEmpty(searchTitle))
            {
                courses = courses.Where(s => s.Title.ToLower().Contains(searchTitle.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchProg))
            {
                courses = courses.Where(s => s.Programme.ToLower().Contains(searchProg.ToLower()));
            }
            if (searchSem != 0)
            {
                courses = courses.Where(s => s.Semester == searchSem);
            }

            courses = courses.Include(c => c.FirstTeacher).Include(c => c.SecondTeacher);

            var VM = new FilterCourses_VM
            {
                SemsList = new SelectList( await query.ToListAsync()),
                Courses = await courses.ToListAsync()
            };


            return View(VM);
        }




      //??
        public IActionResult Enrolled_Students(int? id)
        {
            IQueryable<Student> students = _context.Students.AsQueryable();

            IQueryable<Enrollment> enrollments = _context.Enrollments.AsQueryable();
            enrollments = enrollments.Include(c => c.Student).Include(c => c.Course);
            enrollments = enrollments.Where(s => s.CourseID == id);

            IEnumerable<long> enrolled = enrollments.OrderBy(e => e.StudentID).Select(e => e.StudentID).Distinct();

            students = students.Include(c => c.Enrollments).ThenInclude(c => c.Course);

            students = students.Where(s => enrolled.Contains(s.ID));

            ViewData["Course"] = _context.Courses.Where(s => s.CourseID == id).Select(s => s.Title).FirstOrDefault();

            return View(students);
        }



        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            ViewData["FirstTeacherID"] = new SelectList(_context.Teachers, "TeacherID", "FirstName");
            ViewData["SecondTeacherID"] = new SelectList(_context.Teachers, "TeacherID", "FirstName");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseID,Title,Credits,Semester,Programme,EducationLevel,FirstTeacherID,SecondTeacherID,Enrollments")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FirstTeacherID"] = new SelectList(_context.Teachers, "TeacherID", "FirstName", course.FirstTeacherID);
            ViewData["SecondTeacherID"] = new SelectList(_context.Teachers, "TeacherID", "FirstName", course.SecondTeacherID);
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course =_context.Courses.Where(m => m.CourseID == id)
                .Include(x => x.Enrollments).First();
            IQueryable<Course> coursesQuery = _context.Courses.AsQueryable();
            coursesQuery = coursesQuery.Where(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }
            var students = _context.Students.AsEnumerable();
            students = students.OrderBy(s => s.FullName);

            Enroll_Students_VM viewmodel = new Enroll_Students_VM
            {
                Course = await coursesQuery.Include(c => c.FirstTeacher).Include(c => c.SecondTeacher).FirstAsync(),
                EnrolledStudents = new MultiSelectList(students, "ID", "FullName"),
                selectedStudents = course.Enrollments.Select(sa => sa.StudentID)
            };

            //ViewData["Teachers"] = new SelectList(_context.Set<Teacher>(), "TeacherID", "FullName");
            ViewData["FirstTeacherID"] = new SelectList(_context.Teachers, "TeacherID", "FullName", course.FirstTeacherID);
            ViewData["SecondTeacherID"] = new SelectList(_context.Teachers, "TeacherID", "FullName", course.SecondTeacherID);
            return View(viewmodel);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Enroll_Students_VM viewmodel)
        {
            if (id != viewmodel.Course.CourseID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewmodel.Course);
                    await _context.SaveChangesAsync();

                    var course = _context.Courses.Where(m => m.CourseID == id).First();
                    string semester;
                    if (course.Semester % 2 == 0)
                    {
                        semester = "leten";
                    }
                    else
                    {
                        semester = "zimski";
                    }
                    IEnumerable<long> selectedStudents = viewmodel.selectedStudents;
                    if (selectedStudents != null)
                    {
                        IQueryable<Enrollment> toBeRemoved = _context.Enrollments.Where(s => !selectedStudents.Contains(s.StudentID) && s.CourseID == id);
                        _context.Enrollments.RemoveRange(toBeRemoved);

                        IEnumerable<long> existEnrollments = _context.Enrollments.Where(s => selectedStudents.Contains(s.StudentID) && s.CourseID == id).Select(s => s.StudentID);
                        IEnumerable<long> newEnrollments = selectedStudents.Where(s => !existEnrollments.Contains(s));

                        foreach (long studentId in newEnrollments)
                            _context.Enrollments.Add(new Enrollment { StudentID = studentId, CourseID = id, Semester = semester, Year = viewmodel.Year });

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        IQueryable<Enrollment> toBeRemoved = _context.Enrollments.Where(s => s.CourseID == id);
                        _context.Enrollments.RemoveRange(toBeRemoved);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(viewmodel.Course.CourseID))
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
            return View(viewmodel);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseID == id);
        }

        // GET: Courses/CoursesTeaching/5
      //  [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> CoursesTeaching(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(m => m.TeacherID == id);
            ViewBag.Message = teacher.FullName;
            IQueryable<Course> coursesQuery = _context.Courses.Where(m => m.FirstTeacherID == id || m.SecondTeacherID == id);
            await _context.SaveChangesAsync();
            if (teacher == null)
            {
                return NotFound();
            }
            var CourseTitleVM = new FilterCourses_VM
            {
                Courses = await coursesQuery.ToListAsync(),
            };

            return View(CourseTitleVM);
        }
    }
}
