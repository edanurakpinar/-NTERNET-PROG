using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using project.Data;
using project.Models;
using System;
using System.Linq;

namespace project.Controllers
{
    public class CourseController : Controller
    {
        private readonly SchoolContext _context;

        public CourseController(SchoolContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetCourses()
        {
            var courses = _context.Courses
                .Include(c => c.Teacher)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description,
                    c.Credits,
                    c.TeacherId,
                    TeacherName = c.Teacher.FirstName + " " + c.Teacher.LastName
                })
                .ToList();

            return Json(courses);
        }

        [HttpGet]
        public JsonResult GetTeachersForDropdown()
        {
            var teachers = _context.Teachers
                .Select(t => new
                {
                    Value = t.Id,
                    Text = t.FirstName + " " + t.LastName
                })
                .ToList();

            return Json(teachers);
        }

        [HttpPost]
        public JsonResult AddCourse(Course course)
        {
            try
            {
                _context.Courses.Add(course);
                _context.SaveChanges();
                return Json(new { success = true, message = "Ders başarıyla eklendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateCourse(Course course)
        {
            try
            {
                _context.Entry(course).State = EntityState.Modified;
                _context.SaveChanges();
                return Json(new { success = true, message = "Ders bilgileri güncellendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteCourse(int id)
        {
            var course = _context.Courses.Find(id);
            if (course != null)
            {
                try
                {
                    _context.Courses.Remove(course);
                    _context.SaveChanges();
                    return Json(new { success = true, message = "Ders silindi." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
                }
            }
            return Json(new { success = false, message = "Ders bulunamadı." });
        }

        public IActionResult Details(int id)
        {
            var course = _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
                .FirstOrDefault(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }
    }
}