using Microsoft.AspNetCore.Mvc;
using KTGK.Data;
using System.Linq;

namespace KTGK.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TeacherController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("role") != "Teacher")
                return RedirectToAction("Login", "Auth");

            ViewBag.ExamCount = _context.Exams.Count();
            ViewBag.StudentCount = _context.Users.Count(u => u.Role == "Student");
            ViewBag.ResultCount = _context.Results.Count();

            return View();
        }

        public IActionResult Results()
        {
            if (HttpContext.Session.GetString("role") != "Teacher")
            return RedirectToAction("Login", "Auth");
            var results = _context.Results.ToList();
            return View(results);
        }

        public IActionResult Exams()
        {
            if (HttpContext.Session.GetString("role") != "Teacher")
            return RedirectToAction("Login", "Auth");
            var exams = _context.Exams.ToList();
            return RedirectToAction("Library", "Exam");
        }

        public IActionResult ResultDetail(int id)
        {
            if (HttpContext.Session.GetString("role") != "Teacher")
            return RedirectToAction("Login", "Auth");
            var details = _context.ResultDetails
                .Where(r => r.ResultId == id)
                .ToList();

            return View(details);
        }
    }
}