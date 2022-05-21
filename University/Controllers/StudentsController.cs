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
    public class StudentsController : Controller
    {
        private readonly UniversityContext _context;

        public StudentsController(UniversityContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult>Index(string searchIme, string searchStudentId)
        {
            IEnumerable<Student> students = _context.Students.AsEnumerable();

            if (!string.IsNullOrEmpty(searchIme))
            {
                students = students.Where(s => s.FullName.ToLower().Contains(searchIme.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchStudentId))
            {
                students = students.Where(s => s.StudentID.Contains(searchStudentId));
            }

            var VM2 = new FilterStudents_VM
            {
                Students = students
            };


            return View(VM2);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            Edit_StudentPicture viewmodel = new Edit_StudentPicture
            {
                Student = student,
                Desc = student.Picture
            };

            return View(viewmodel);
        }

        public async Task<IActionResult> EditPicture(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = _context.Students.Where(m => m.ID == id).Include(m => m.Enrollments).First();
            if (student == null)
            {
                return NotFound();
            }

            var courses = _context.Courses.AsEnumerable();
            courses = courses.OrderBy(s => s.Title);

            Edit_StudentPicture viewmodel = new Edit_StudentPicture
            {
                Student = student,
                Desc = student.Picture
            };

            return View(viewmodel);
        }

        private string UploadedFile(Edit_StudentPicture viewmodel)
        {
            string uniqueFileName = null;

            if (viewmodel.ProfileImage != null)
            {
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPicture(long id, Edit_StudentPicture viewmodel)
        {
            if (id != viewmodel.Student.ID)
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
                        viewmodel.Student.Picture = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.Student.Picture = viewmodel.Desc;
                    }

                    _context.Update(viewmodel.Student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(viewmodel.Student.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = viewmodel.Student.ID });
            }
            return View(viewmodel);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,StudentID,FirstName,LastName,EnrollmentDate,AcquiredCredits,CurrentSemester,EducationLevel,Enrollments,Picture")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = _context.Students.Where(x => x.ID == id).Include(x => x.Enrollments).First();
            if (student == null)
            {
                return NotFound();
            }

            var courses = _context.Courses.AsEnumerable();
            courses = courses.OrderBy(s => s.Title);

            Enroll_Courses_forStudents viewmodel = new Enroll_Courses_forStudents
            {
                Student = student,
                EnrolledCourses = new MultiSelectList(courses, "CourseID", "Title"),
                selectedCourses = student.Enrollments.Select(x => x.CourseID)
            };

            ViewData["Courses"] = new SelectList(_context.Set<Course>(), "CourseID", "Title");
            return View(viewmodel);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Enroll_Courses_forStudents viewmodel)
        {
            if (id != viewmodel.Student.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewmodel.Student);
                    await _context.SaveChangesAsync();

                    var student = _context.Students.Where(x => x.ID == id).First();

                    IEnumerable<int> selectedCourses = viewmodel.selectedCourses;
                    if (selectedCourses != null)
                    {
                        IQueryable<Enrollment> toBeRemoved = _context.Enrollments.Where(s => !selectedCourses.Contains(s.CourseID) && s.StudentID == id);
                        _context.Enrollments.RemoveRange(toBeRemoved);

                        IEnumerable<int> existEnrollments = _context.Enrollments.Where(s => selectedCourses.Contains(s.CourseID) && s.StudentID == id).Select(s => s.CourseID);
                        IEnumerable<int> newEnrollments = selectedCourses.Where(s => !existEnrollments.Contains(s));

                        foreach (int courseId in newEnrollments)
                            _context.Enrollments.Add(new Enrollment { StudentID = id, CourseID = courseId, Semester = viewmodel.semester, Year = viewmodel.year });

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        IQueryable<Enrollment> toBeRemoved = _context.Enrollments.Where(s => s.StudentID == id);
                        _context.Enrollments.RemoveRange(toBeRemoved);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(viewmodel.Student.ID))
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

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var student = await _context.Students.FindAsync(id);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(long id)
        {
            return _context.Students.Any(e => e.ID == id);
        }

        // GET: Students/StudentsEnrolled/5
        public async Task<IActionResult> StudentsEnrolled(int? id, string? fullName, string? studentId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.CourseID == id);
            ViewBag.Message = course.Title;
            IQueryable<Student> studentsQuery = _context.Enrollments.Where(x => x.CourseID == id).Select(x => x.Student);
            await _context.SaveChangesAsync();
            if (course == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(fullName))
            {
                if (fullName.Contains(" "))
                {
                    string[] names = fullName.Split(" ");
                    studentsQuery = studentsQuery.Where(x => x.FirstName.Contains(names[0]) || x.LastName.Contains(names[1]) ||
                    x.FirstName.Contains(names[1]) || x.LastName.Contains(names[0]));
                }
                else
                {
                    studentsQuery = studentsQuery.Where(x => x.FirstName.Contains(fullName) || x.LastName.Contains(fullName));
                }
            }
            if (!string.IsNullOrEmpty(studentId))
            {
                studentsQuery = studentsQuery.Where(x => x.StudentID.Contains(studentId));
            }

            var studentFilterVM = new FilterStudents_VM
            {
                Students = await studentsQuery.ToListAsync(),
            };

            return View(studentFilterVM);
        }
    }
}

