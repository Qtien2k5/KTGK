using KTGK.Data;
using KTGK.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KTGK.Controllers
{
    public class ExamController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ───────────────────────────────────────────
        // THƯ VIỆN ĐỀ THI
        // ───────────────────────────────────────────
        public IActionResult Library(string search, string filter, string sort)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var query = _context.Exams.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(e => e.Title.Contains(search));

            if (filter == "done" && userId != null)
                query = query.Where(e => _context.Results.Any(r => r.ExamId == e.ExamId && r.UserId == userId));
            else if (filter == "notdone" && userId != null)
                query = query.Where(e => !_context.Results.Any(r => r.ExamId == e.ExamId && r.UserId == userId));

            if (sort == "newest")
                query = query.OrderByDescending(e => e.ExamId);
            else if (sort == "oldest")
                query = query.OrderBy(e => e.ExamId);

            var exams = query
                .Select(e => new KTGK.ViewModels.ExamViewModel
                {
                    ExamId = e.ExamId,
                    Title = e.Title,
                    Description = e.Description,
                    Duration = e.Duration,
                    StudentCount = _context.Results.Count(r => r.ExamId == e.ExamId),
                    IsDone = _context.Results.Any(r => r.ExamId == e.ExamId && r.UserId == userId)
                })
                .ToList();

            return View(exams);
        }

        // ───────────────────────────────────────────
        // LÀM BÀI THI
        // ───────────────────────────────────────────
        public IActionResult DoExam(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var exam = _context.Exams
                .Include(e => e.Questions)
                    .ThenInclude(q => q.Answers)
                .Include(e => e.Questions)
                    .ThenInclude(q => q.Passage)
                .FirstOrDefault(e => e.ExamId == id);

            if (exam == null) return Content("Không tìm thấy đề");

            return View(exam);
        }

        // ───────────────────────────────────────────
        // NỘP BÀI
        // ───────────────────────────────────────────
        [HttpPost]
        public IActionResult Submit(int examId, Dictionary<int, int> answers, int timeTaken)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var questions = _context.Questions
                .Where(q => q.ExamId == examId)
                .ToList();

            int correct = 0, wrong = 0, skipped = 0;

            var result = new Result
            {
                ExamId = examId,
                UserId = userId.Value,
                SubmitTime = DateTime.Now,
                Score = 0,
                TimeTaken = timeTaken,
                StudentName = HttpContext.Session.GetString("username") ?? "" // ← thêm dòng này
            };
            _context.Results.Add(result);
            _context.SaveChanges();

            foreach (var q in questions)
            {
                if (answers.ContainsKey(q.QuestionId) && answers[q.QuestionId] != 0)
                {
                    var selected = answers[q.QuestionId];
                    _context.ResultDetails.Add(new ResultDetail
                    {
                        ResultId = result.ResultId,
                        QuestionId = q.QuestionId,
                        SelectedAnswerId = selected
                    });

                    var correctAnswer = _context.Answers
                        .FirstOrDefault(a => a.QuestionId == q.QuestionId && a.IsCorrect);

                    if (correctAnswer != null && correctAnswer.AnswerId == selected)
                        correct++;
                    else
                        wrong++;
                }
                else
                {
                    skipped++;
                    _context.ResultDetails.Add(new ResultDetail
                    {
                        ResultId = result.ResultId,
                        QuestionId = q.QuestionId,
                        SelectedAnswerId = 0
                    });
                }
            }

            result.Score = correct;
            _context.SaveChanges();

            return RedirectToAction("ResultDetail", new { id = result.ResultId });
        }

        // ───────────────────────────────────────────
        // KẾT QUẢ
        // ───────────────────────────────────────────
        public IActionResult ResultDetail(int id)
        {
            var role = HttpContext.Session.GetString("role");
            var userId = HttpContext.Session.GetInt32("UserId");

            var result = _context.Results
                .Include(r => r.Exam)
                .FirstOrDefault(r => r.ResultId == id);

            if (result == null) return NotFound();
            if (role != "Teacher" && result.UserId != userId)
                return Content("Bạn không có quyền xem bài này");

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

        // ───────────────────────────────────────────
        // EDIT / DELETE (Teacher)
        // ───────────────────────────────────────────
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("role") != "Teacher")
                return RedirectToAction("Login", "Auth");
            var exam = _context.Exams.Find(id);
            if (exam == null) return NotFound();
            return View(exam);
        }

        [HttpPost]
        public IActionResult Edit(Exam exam)
        {
            if (HttpContext.Session.GetString("role") != "Teacher")
                return RedirectToAction("Login", "Auth");
            if (!ModelState.IsValid) return View(exam);
            _context.Exams.Update(exam);
            _context.SaveChanges();
            return RedirectToAction("Library");
        }

        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("role") != "Teacher")
                return RedirectToAction("Login", "Auth");

            var exam = _context.Exams.Find(id);
            if (exam == null) return NotFound();

            var questions = _context.Questions.Where(q => q.ExamId == id).ToList();
            foreach (var q in questions)
                _context.Answers.RemoveRange(_context.Answers.Where(a => a.QuestionId == q.QuestionId));

            _context.Questions.RemoveRange(questions);
            _context.Exams.Remove(exam);
            _context.SaveChanges();

            return RedirectToAction("Library");
        }
    }
}