using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using University.Areas.Identity.Data;
using University.Data;

namespace University.Models
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<UniversityUser>>();
            IdentityResult roleResult;
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            UniversityUser user = await UserManager.FindByEmailAsync("admin@uni.com");
            if (user == null)
            {
                var User = new UniversityUser();
                User.Email = "admin@uni.com";
                User.UserName = "admin@uni.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
            // creating Teacher role     
            var x = await RoleManager.RoleExistsAsync("Teacher");
            if (!x)
            {
                var role = new IdentityRole();
                role.Name = "Teacher";
                await RoleManager.CreateAsync(role);
            }

            // creating Student role     
            x = await RoleManager.RoleExistsAsync("Student");
            if (!x)
            {
                var role = new IdentityRole();
                role.Name = "Student";
                await RoleManager.CreateAsync(role);
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new UniversityContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<UniversityContext>>()))
            {

                CreateUserRoles(serviceProvider).Wait();

                // Look for any students, teachers, enrollments or courses.
                if (context.Teachers.Any() || context.Students.Any() || context.Courses.Any() || context.Enrollments.Any())
                {
                    return;   // DB has been seeded
                }

                context.Teachers.AddRange(
                    new Teacher { /*TeacherID = 1, */ FirstName = "Irina", LastName = "Ralevska",
                        Degree = "Dr", AcademicRank = "Dr", OfficeNumber = "4", HireDate = DateTime.Parse("2012-5-14") },
                    new Teacher{ /*TeacherID = 2, */
                        FirstName = "Vladimir", LastName = "Dukovski", 
                        Degree = "Dr", AcademicRank = "Dr", OfficeNumber = "12", HireDate = DateTime.Parse("2015-1-26") },
                    new Teacher{ /*TeacherID = 3, */
                        FirstName = "Ivana", LastName = "Tanevska",
                        Degree = "Dr", AcademicRank = "Dr", OfficeNumber = "2", HireDate = DateTime.Parse("2018-2-2") }
                );
                context.SaveChanges();

                context.Students.AddRange(
                    new Student { StudentID = "1/2019", FirstName = "Emilija", LastName = "Chona", 
                        EnrollmentDate = DateTime.Parse("2019-10-15"), AcquiredCredits = 174, CurrentSemester = 6, EducationLevel = "Undergraduate" },
                    new Student { StudentID = "183/2019", FirstName = "ALeksandar", LastName = "Rizev",
                        EnrollmentDate = DateTime.Parse("2019-10-15"), AcquiredCredits = 100, CurrentSemester = 6, EducationLevel = "Undergraduate" },
                    new Student { StudentID = "22/2019", FirstName = "Sofija", LastName = "Petlichkovska",
                        EnrollmentDate = DateTime.Parse("2019-10-15"), AcquiredCredits = 115, CurrentSemester = 6, EducationLevel = "Undergraduate" },
                    new Student { StudentID = "81/2021", FirstName = "Natalija", LastName = "Chona",
                        EnrollmentDate = DateTime.Parse("2021-10-15"), AcquiredCredits = 30, CurrentSemester = 2, EducationLevel = "Undergraduate" },
                    new Student { StudentID = "3/2020", FirstName = "Mihaela", LastName = "Zlateska",
                        EnrollmentDate = DateTime.Parse("2020-10-15"), AcquiredCredits = 110, CurrentSemester = 4, EducationLevel = "Undergraduate" },
                    new Student { StudentID = "196/2019", FirstName = "Leonida", LastName = "Lumburovska",
                        EnrollmentDate = DateTime.Parse("2019-10-15"), AcquiredCredits = 150, CurrentSemester = 6, EducationLevel = "Undergraduate" },
                    new Student { StudentID = "190/2019", FirstName = "Teodora", LastName = "Tanchevska",
                        EnrollmentDate = DateTime.Parse("2019-10-15"), AcquiredCredits = 100, CurrentSemester = 6, EducationLevel = "Undergraduate" },
                    new Student { StudentID = "123/2020", FirstName = "Tereza", LastName = "Grujevska",
                        EnrollmentDate = DateTime.Parse("2020-10-15"), AcquiredCredits = 70, CurrentSemester = 4, EducationLevel = "Undergraduate" }
                );
                context.SaveChanges();

                context.Courses.AddRange(
                    
                    new Course
                        {
             
                            Title = "RSWEB",
                            Credits = 6,
                            Semester = 6,
                            Programme = "KTI",
                            EducationLevel = "Undergraduate",
                            FirstTeacherID = 1,
                            SecondTeacherID = 2
                        },
                    new Course
                        {
                            
                            Title = "MPB",
                            Credits = 6,
                            Semester = 4,
                            Programme = "KTI",
                            EducationLevel = "Undergraduate",
                            FirstTeacherID = 1,
                            SecondTeacherID = 3
                        });
                context.SaveChanges();

                context.Enrollments.AddRange(
                    new Enrollment { CourseID= 1, StudentID = 1, Semester = "leten", Year = 3,
                        Grade = 10, SeminalUrl = "#", ProjectUrl = "#", ExamPoints = 95, SeminalPoints = 60, AdditionalPoints = 10, ProjectPoints = 10,
                        FinishDate = DateTime.Parse("2022-5-29") },
                   new Enrollment { CourseID = 2, StudentID = 2, Semester = "zimski", Year = 2,
                       Grade = 8, SeminalUrl = "#", ProjectUrl = "#", ExamPoints = 60, SeminalPoints = 45, AdditionalPoints = 10, ProjectPoints = 10,
                       FinishDate = DateTime.Parse("2022-5-29") });

                context.SaveChanges();
            }
        }
    }
}
