using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Data;
using project.Models;
using System;
using System.Linq;

namespace project.Controllers
{
    public class StudentController : Controller
    {
        private readonly SchoolContext _context;

        public StudentController(SchoolContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetStudents()
        {
            var students = _context.Students.ToList();
            return Json(students);
        }

        [HttpPost]
        public JsonResult AddStudent(Student student)
        {
            try
            {
                _context.Students.Add(student);
                _context.SaveChanges();
                return Json(new { success = true, message = "Öğrenci başarıyla eklendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateStudent(Student student)
        {
            try
            {
                _context.Entry(student).State = EntityState.Modified;
                _context.SaveChanges();
                return Json(new { success = true, message = "Öğrenci bilgileri güncellendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteStudent(int id)
        {
            var student = _context.Students.Find(id);
            if (student != null)
            {
                try
                {
                    _context.Students.Remove(student);
                    _context.SaveChanges();
                    return Json(new { success = true, message = "Öğrenci silindi." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
                }
            }
            return Json(new { success = false, message = "Öğrenci bulunamadı." });
        }

        public IActionResult Details(int id)
        {
            var student = _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .FirstOrDefault(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }
    }
}