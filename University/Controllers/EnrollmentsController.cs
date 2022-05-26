#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using University.Data;
using University.Models;
using University.ViewModels;

namespace University.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly UniversityContext _context;
        public EnrollmentsController(UniversityContext context)
        {
            _context = context;
        }

        // GET: Enrollments
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> Index()
        {
            var universityContext = _context.Enrollments.Include(e => e.Student).Include(e => e.Course).ThenInclude(e=>e.FirstTeacher);
            universityContext = universityContext.Include(e => e.Course).ThenInclude(e => e.SecondTeacher);
           
            return View(await universityContext.ToListAsync());
        }

        //Create
        [Authorize(Roles="Admin")]
        public IActionResult Index1()
        {
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "CourseID");
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "ID");

            return View();
        }

        [Authorize(Roles="Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index1([Bind("EnrollmentID,CourseID,StudentID,Semester,Year")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "CourseID", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "ID", enrollment.StudentID);
            return View(enrollment);
        }

        // GET: Enrollments/Details/5
        [Authorize(Roles="Admin,Student,Teacher")]
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.EnrollmentID == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: Enrollments/Create
        //[Authorize(Roles="Admin")] ??nz dali mi treba ovaa akcija
        public IActionResult Create()
        {
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title");
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FirstName");
            return View();
        }

        // POST: Enrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize(Roles="Admin")] ??
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnrollmentID,CourseID,StudentID,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FirstName", enrollment.StudentID);
            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FirstName", enrollment.StudentID);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles="Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("EnrollmentID,CourseID,StudentID,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (id != enrollment.EnrollmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.EnrollmentID))
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
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FirstName", enrollment.StudentID);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.EnrollmentID == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [Authorize(Roles="Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnrollmentExists(long id)
        {
            return _context.Enrollments.Any(e => e.EnrollmentID == id);
        }

        [Authorize(Roles="Admin,Teacher")]
        public async Task<IActionResult> ListEnrolledStudents(int? id, string teacher, int year)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.CourseID== id);

            string[] names = teacher.Split(" ");
            var teacherModel = await _context.Teachers.FirstOrDefaultAsync(m => m.FirstName == names[0] && m.LastName == names[1]);
            ViewBag.teacher = teacher;
            ViewBag.course = course.Title;
            var enrollment = _context.Enrollments.Where(x => x.CourseID == id && (x.Course.FirstTeacherID == teacherModel.TeacherID || x.Course.SecondTeacherID == teacherModel.TeacherID))
            .Include(e => e.Course)
            .Include(e => e.Student);
            await _context.SaveChangesAsync();
            IQueryable<int?> yearsQuery = _context.Enrollments.OrderBy(m => m.Year).Select(m => m.Year).Distinct();
            IQueryable<Enrollment> enrollmentQuery = enrollment.AsQueryable();
            if (year != null && year != 0)
            {
                enrollmentQuery = enrollmentQuery.Where(x => x.Year == year);
            }
            else
            {
                enrollmentQuery = enrollmentQuery.Where(x => x.Year == yearsQuery.Max());
            }

            if (enrollment == null)
            {
                return NotFound();
            }

            FilterEnrollment viewmodel = new FilterEnrollment
            {
                Enrollments = await enrollmentQuery.ToListAsync(),
                listYears = new SelectList(await yearsQuery.ToListAsync())
            };

            return View(viewmodel);
        }

        // GET: Enrollments/EditAsTeacher/5
       [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> EditAsTeacher(long? id, string teacher)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewBag.teacher = teacher;
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FirstName", enrollment.StudentID);
            return View(enrollment);
        }


        [Authorize(Roles = "Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsTeacher(long id, string teacher, [Bind("EnrollmentID,CourseID,StudentID,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (id != enrollment.EnrollmentID)
            {
                return NotFound();
            }
            string temp = teacher;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.EnrollmentID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("StudentsEnrolledAtCourse", new { id = enrollment.CourseID, teacher = temp, year = enrollment.Year });
            }
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FirstName", enrollment.StudentID);
            return View(enrollment);
        }

        // GET: Enrollments/StudentCourses/5
       [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> StudentCourses(int? id, string? userID)
        {
            if (userID != null)
            {
                var stu = await _context.Students.FirstOrDefaultAsync(x => x.userId == userID);
                id = (int?)stu.ID;
            }
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.ID == id);

            ViewBag.student = student.FullName;

            IQueryable<Enrollment> enrollment = _context.Enrollments.Where(x => x.StudentID == id)
            .Include(e => e.Course)
            .Include(e => e.Student);
            await _context.SaveChangesAsync();

            if (enrollment == null)
            {
                return NotFound();
            }

            return View(await enrollment.ToListAsync());
        }

        [Authorize(Roles="Student")]
        public async Task<IActionResult> EditAsStudent(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = _context.Enrollments.Where(m => m.EnrollmentID == id).Include(x => x.Student).Include(x => x.Course).First();
            IQueryable<Enrollment> enrollmentQuery = _context.Enrollments.AsQueryable();
            enrollmentQuery = enrollmentQuery.Where(m => m.EnrollmentID == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            EditAsStudent_VM viewmodel = new EditAsStudent_VM
            {
                enrollment = await enrollmentQuery.Include(x => x.Student).Include(x => x.Course).FirstAsync(),
                seminalUrlName = enrollment.SeminalUrl
            };
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FirstName", enrollment.StudentID);
            return View(viewmodel);
        }


        [Authorize(Roles="Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsStudent(long id, EditAsStudent_VM viewmodel)
        {
            if (id != viewmodel.enrollment.EnrollmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    if (viewmodel.seminalUrlFile != null)
                    {
                        string uniqueFileName = UploadedFile(viewmodel);
                        viewmodel.enrollment.SeminalUrl = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.enrollment.SeminalUrl = viewmodel.seminalUrlName;
                    }

                    _context.Update(viewmodel.enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(viewmodel.enrollment.EnrollmentID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("StudentCourses", new { id = viewmodel.enrollment.StudentID });
            }

            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title", viewmodel.enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FirstName", viewmodel.enrollment.StudentID);
            return View(viewmodel);
        }

        //[Authorize(Roles="Student")]
        private string UploadedFile(EditAsStudent_VM viewmodel)
        {
            string uniqueFileName = null;

            if (viewmodel.seminalUrlFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/seminals");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(viewmodel.seminalUrlFile.FileName);
                string fileNameWithPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewmodel.seminalUrlFile.CopyTo(stream);
                }
            }
            return uniqueFileName;
        }

    }
}
