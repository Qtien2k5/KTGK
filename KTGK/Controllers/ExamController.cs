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

        public IActionResult Library(string search, string filter, string sort)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var query = _context.Exams.AsQueryable();

            // 🔍 SEARCH
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Title.Contains(search));
            }

            // 📊 FILTER
            if (filter == "done" && userId != null)
            {
                query = query.Where(e =>
                    _context.Results.Any(r => r.ExamId == e.ExamId && r.UserId == userId));
            }
            else if (filter == "notdone" && userId != null)
            {
                query = query.Where(e =>
                    !_context.Results.Any(r => r.ExamId == e.ExamId && r.UserId == userId));
            }

            // 🔽 SORT
            if (sort == "newest")
            {
                query = query.OrderByDescending(e => e.ExamId);
            }
            else if (sort == "oldest")
            {
                query = query.OrderBy(e => e.ExamId);
            }

            // ✅ CHỈ MAP 1 LẦN DUY NHẤT Ở CUỐI
            var exams = query
                .Select(e => new KTGK.ViewModels.ExamViewModel
                {
                    ExamId = e.ExamId,
                    Title = e.Title,
                    Description = e.Description,
                    Duration = e.Duration,
                    StudentCount = _context.Results
                        .Count(r => r.ExamId == e.ExamId),
                    IsDone = _context.Results
                        .Any(r => r.ExamId == e.ExamId && r.UserId == userId)
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
        public IActionResult ResultDetail(int id)
        {
            var role = HttpContext.Session.GetString("role");
            var userId = HttpContext.Session.GetInt32("UserId");

            var result = _context.Results.FirstOrDefault(r => r.ResultId == id);

            if (result == null)
                return NotFound();

            if (role != "Teacher" && result.UserId != userId)
            {
                return Content("Bạn không có quyền xem bài này");
            }

            var data = _context.ResultDetails
                .Where(r => r.ResultId == id)
                .Select(r => new
                {
                    Question = r.Question.Content,
                    Answers = r.Question.Answers.ToList(),
                    SelectedAnswerId = r.SelectedAnswerId
                })
                .ToList();

            return View(data);
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
        public IActionResult Submit(int examId, Dictionary<int, int> answers)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var questions = _context.Questions
                .Where(q => q.ExamId == examId)
                .ToList();

            int score = 0;

            var result = new Result
            {
                ExamId = examId,
                UserId = userId.Value,
                SubmitTime = DateTime.Now,
                Score = 0
            };

            _context.Results.Add(result);
            _context.SaveChanges();

            foreach (var q in questions)
            {
                if (answers.ContainsKey(q.QuestionId))
                {
                    var selected = answers[q.QuestionId];

                    var detail = new ResultDetail
                    {
                        ResultId = result.ResultId,
                        QuestionId = q.QuestionId,
                        SelectedAnswerId = selected
                    };

                    _context.ResultDetails.Add(detail);

                    // tính điểm
                    var correct = _context.Answers
                        .FirstOrDefault(a => a.QuestionId == q.QuestionId && a.IsCorrect);

                    if (correct != null && correct.AnswerId == selected)
                    {
                        score++;
                    }
                }
            }

            result.Score = score;
            _context.SaveChanges();

            return RedirectToAction("Results", "Teacher");

        }
    }
}