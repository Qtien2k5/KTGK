using KTGK.Data;
using KTGK.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace KTGK.Controllers
{
    public class ExamController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Library()
        {
            var exams = _context.Exams
                .Select(e => new KTGK.ViewModels.ExamViewModel
                {
                    ExamId = e.ExamId,
                    Title = e.Title,
                    Description = e.Description,
                    Duration = e.Duration,
                    StudentCount = _context.Results
                        .Count(r => r.ExamId == e.ExamId)
                })
                .ToList();

            return View(exams);
        }
        public IActionResult DoExam(int id)
        {
            var exam = _context.Exams
                .Where(e => e.ExamId == id)
                .Select(e => new
                {
                    e.ExamId,
                    e.Title,
                    Questions = e.Questions.Select(q => new
                    {
                        q.QuestionId,
                        q.Content,
                        Answers = q.Answers.ToList()
                    }).ToList()
                })
                .FirstOrDefault();

            if (exam == null)
                return Content("Không tìm thấy đề");

            return View(exam);
        }

        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("role") != "Teacher")
                return RedirectToAction("Login", "Auth");

            var exam = _context.Exams.Find(id);

            if (exam == null)
                return NotFound();

            return View(exam);
        }

        [HttpPost]
        public IActionResult Edit(Exam exam)
        {
            if (HttpContext.Session.GetString("role") != "Teacher")
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(exam);

            _context.Exams.Update(exam);
            _context.SaveChanges();

            return RedirectToAction("Library");
        }

        public IActionResult Delete(int id)
        {
            // check quyền
            if (HttpContext.Session.GetString("role") != "Teacher")
                return RedirectToAction("Login", "Auth");

            var exam = _context.Exams.Find(id);
            if (exam == null)
                return NotFound();

            // 🔥 XÓA ANSWERS → QUESTIONS → EXAM
            var questions = _context.Questions
                .Where(q => q.ExamId == id)
                .ToList();

            foreach (var q in questions)
            {
                var answers = _context.Answers
                    .Where(a => a.QuestionId == q.QuestionId);

                _context.Answers.RemoveRange(answers);
            }

            _context.Questions.RemoveRange(questions);
            _context.Exams.Remove(exam);

            _context.SaveChanges();

            return RedirectToAction("Library");
        }

    }
}