using KTGK.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var userId = HttpContext.Session.GetInt32("UserId");

            // ✅ Lấy cả ResultId theo từng exam
            var doneExams = _context.Results
                .Where(r => r.UserId == userId)
                .Include(r => r.Exam)
                .Select(r => new {
                    ExamId = r.ExamId,
                    Title = r.Exam.Title,
                    ResultId = r.ResultId  // ✅ thêm dòng này
                })
                .ToList();

            var doneExamIds = doneExams.Select(r => r.ExamId).ToList();

            var notDoneExams = _context.Exams
                .Where(e => !doneExamIds.Contains(e.ExamId))
                .ToList();

            ViewBag.DoneExams = doneExams;
            ViewBag.NotDoneExams = notDoneExams;
            return View();
        }
        public IActionResult ResultDetail(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var result = _context.Results
                .Include(r => r.Exam)
                .FirstOrDefault(r => r.ResultId == id && r.UserId == userId.Value); // ✅ filter thẳng trong query

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