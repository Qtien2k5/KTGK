using KTGK.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            var result = _context.Results
                .Include(r => r.Exam)
                .FirstOrDefault(r => r.ResultId == id);

            if (result == null) return NotFound();

            var totalQuestions = _context.Questions.Count(q => q.ExamId == result.ExamId);

            var details = _context.ResultDetails
                .Where(r => r.ResultId == id)
                .Select(r => new
                {
                    QuestionId = r.QuestionId,
                    QuestionContent = r.Question.Content,
                    Answers = r.Question.Answers.ToList(),
                    SelectedAnswerId = r.SelectedAnswerId
                })
                .ToList();

            int correct = details.Count(d =>
                d.Answers.Any(a => a.IsCorrect && a.AnswerId == d.SelectedAnswerId));
            int wrong = details.Count(d =>
                d.SelectedAnswerId != 0 &&
                !d.Answers.Any(a => a.IsCorrect && a.AnswerId == d.SelectedAnswerId));
            int skipped = details.Count(d => d.SelectedAnswerId == 0);

            ViewBag.Result = result;
            ViewBag.Correct = correct;
            ViewBag.Wrong = wrong;
            ViewBag.Skipped = skipped;
            ViewBag.Total = totalQuestions;
            ViewBag.TimeTaken = result.TimeTaken;

            return View("~/Views/Teacher/ResultDetail.cshtml", details);
        }
    }
}