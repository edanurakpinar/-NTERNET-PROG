using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Data;
using project.Models;
using System.Diagnostics;

namespace project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SchoolContext _context;

        public HomeController(ILogger<HomeController> logger, SchoolContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetDashboardStats()
        {
            var studentCount = _context.Students.Count();
            var teacherCount = _context.Teachers.Count();
            var courseCount = _context.Courses.Count();
            var enrollmentCount = _context.Enrollments.Count();

            return Json(new
            {
                studentCount,
                teacherCount,
                courseCount,
                enrollmentCount
            });
        }

        [HttpGet]
        public JsonResult GetRecentStudents()
        {
            var students = _context.Students
                .OrderByDescending(s => s.Id)
                .Take(5)
                .Select(s => new {
                    s.Id,
                    s.FirstName,
                    s.LastName,
                    s.Email
                })
                .ToList();

            return Json(students);
        }

        [HttpGet]
        public JsonResult GetActiveCourses()
        {
            var courses = _context.Courses
                .Include(c => c.Teacher)
                .Take(5)
                .Select(c => new {
                    c.Id,
                    c.Name,
                    TeacherName = c.Teacher.FirstName + " " + c.Teacher.LastName
                })
                .ToList();

            return Json(courses);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}