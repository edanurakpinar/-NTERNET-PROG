using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Data;
using project.Models;
using System;
using System.Linq;

namespace project.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly SchoolContext _context;

        public EnrollmentController(SchoolContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetEnrollments()
        {
            var enrollments = _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ThenInclude(c => c.Teacher)
                .Select(e => new
                {
                    e.Id,
                    e.EnrollmentDate,
                    e.Grade,
                    e.StudentId,
                    StudentName = e.Student.FirstName + " " + e.Student.LastName,
                    e.CourseId,
                    CourseName = e.Course.Name,
                    TeacherName = e.Course.Teacher.FirstName + " " + e.Course.Teacher.LastName
                })
                .ToList();

            return Json(enrollments);
        }

        [HttpGet]
        public JsonResult GetStudentsForDropdown()
        {
            var students = _context.Students
                .Select(s => new
                {
                    Value = s.Id,
                    Text = s.FirstName + " " + s.LastName
                })
                .ToList();

            return Json(students);
        }

        [HttpGet]
        public JsonResult GetCoursesForDropdown()
        {
            var courses = _context.Courses
                .Select(c => new
                {
                    Value = c.Id,
                    Text = c.Name
                })
                .ToList();

            return Json(courses);
        }

        [HttpPost]
        public JsonResult AddEnrollment(Enrollment enrollment)
        {
            try
            {
                var existingEnrollment = _context.Enrollments
                    .Any(e => e.StudentId == enrollment.StudentId && e.CourseId == enrollment.CourseId);

                if (existingEnrollment)
                {
                    return Json(new { success = false, message = "Bu öğrenci zaten bu derse kayıtlı." });
                }

                _context.Enrollments.Add(enrollment);
                _context.SaveChanges();
                return Json(new { success = true, message = "Kayıt başarıyla eklendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateEnrollment(Enrollment enrollment)
        {
            try
            {
                _context.Entry(enrollment).State = EntityState.Modified;
                _context.SaveChanges();
                return Json(new { success = true, message = "Kayıt bilgileri güncellendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteEnrollment(int id)
        {
            var enrollment = _context.Enrollments.Find(id);
            if (enrollment != null)
            {
                try
                {
                    _context.Enrollments.Remove(enrollment);
                    _context.SaveChanges();
                    return Json(new { success = true, message = "Kayıt silindi." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
                }
            }
            return Json(new { success = false, message = "Kayıt bulunamadı." });
        }
    }
}