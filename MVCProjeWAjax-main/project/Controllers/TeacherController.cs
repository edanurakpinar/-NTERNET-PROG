using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Data;
using project.Models;
using System;
using System.Linq;

namespace project.Controllers
{
    public class TeacherController : Controller
    {
        private readonly SchoolContext _context;

        public TeacherController(SchoolContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetTeachers()
        {
            var teachers = _context.Teachers.ToList();
            return Json(teachers);
        }

        [HttpPost]
        public JsonResult AddTeacher(Teacher teacher)
        {
            try
            {
                _context.Teachers.Add(teacher);
                _context.SaveChanges();
                return Json(new { success = true, message = "Öğretmen başarıyla eklendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateTeacher(Teacher teacher)
        {
            try
            {
                _context.Entry(teacher).State = EntityState.Modified;
                _context.SaveChanges();
                return Json(new { success = true, message = "Öğretmen bilgileri güncellendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteTeacher(int id)
        {
            var teacher = _context.Teachers.Find(id);
            if (teacher != null)
            {
                try
                {
                    _context.Teachers.Remove(teacher);
                    _context.SaveChanges();
                    return Json(new { success = true, message = "Öğretmen silindi." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
                }
            }
            return Json(new { success = false, message = "Öğretmen bulunamadı." });
        }

        public IActionResult Details(int id)
        {
            var teacher = _context.Teachers
                .Include(t => t.Courses)
                .FirstOrDefault(t => t.Id == id);

            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }
    }
}