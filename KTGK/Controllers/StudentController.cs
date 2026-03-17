using Microsoft.AspNetCore.Mvc;
using KTGK.Data;
using System.Linq;

namespace KTGK.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var username = HttpContext.Session.GetString("username");

            var doneExamIds = _context.Results
                .Where(r => r.StudentName == username)
                .Select(r => r.ExamId)
                .ToList();

            var doneExams = _context.Exams
                .Where(e => doneExamIds.Contains(e.ExamId))
                .ToList();

            var notDoneExams = _context.Exams
                .Where(e => !doneExamIds.Contains(e.ExamId))
                .ToList();

            ViewBag.DoneExams = doneExams;
            ViewBag.NotDoneExams = notDoneExams;

            return View();
        }
    }
}