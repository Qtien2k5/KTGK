using Microsoft.AspNetCore.Mvc;
using KTGK.Data;
using KTGK.Models;
using KTGK.Parsers;

namespace KTGK.Controllers
{
    public class UploadController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public UploadController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("role") != "Teacher")
                return RedirectToAction("Login", "Auth");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, string customTitle, int? customDuration, string customDescription)
        {
            if (HttpContext.Session.GetString("role") != "Teacher")
                return RedirectToAction("Login", "Auth");

            if (file == null || file.Length == 0)
            {
                ViewBag.Error = "Vui lòng chọn file Word (.docx)";
                return View("Index");
            }

            if (!file.FileName.EndsWith(".docx"))
            {
                ViewBag.Error = "Chỉ hỗ trợ file .docx";
                return View("Index");
            }

            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsDir);
            var filePath = Path.Combine(uploadsDir, Guid.NewGuid() + "_" + file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            try
            {
                var parser = new ToeicWordParser();
                var parsed = parser.Parse(filePath);

                var exam = new Exam
                {
                    // Ưu tiên: form nhập > file > tên file
                    Title = !string.IsNullOrWhiteSpace(customTitle)
                        ? customTitle
                        : (!string.IsNullOrWhiteSpace(parsed.Title) ? parsed.Title : file.FileName),

                    // Ưu tiên: form nhập > file > mặc định 75
                    Duration = customDuration.HasValue && customDuration > 0
                        ? customDuration.Value
                        : (parsed.Duration > 0 ? parsed.Duration : 75),

                    Description = !string.IsNullOrWhiteSpace(customDescription)
                        ? customDescription
                        : $"Đề thi TOEIC Reading - {parsed.Questions.Count} câu"
                };

                _context.Exams.Add(exam);
                await _context.SaveChangesAsync();

                // Lưu Passages
                var passageIdMap = new Dictionary<int, int>();
                foreach (var p in parsed.Passages)
                {
                    var passage = new Passage { Content = p.Content };
                    _context.Passages.Add(passage);
                    await _context.SaveChangesAsync();
                    passageIdMap[p.TempId] = passage.Id;
                }

                // Lưu Questions + Answers
                foreach (var q in parsed.Questions)
                {
                    var question = new Question
                    {
                        ExamId = exam.ExamId,
                        Content = q.Content,
                        PassageId = q.PassageTempId.HasValue && passageIdMap.ContainsKey(q.PassageTempId.Value)
                            ? passageIdMap[q.PassageTempId.Value]
                            : null
                    };
                    _context.Questions.Add(question);
                    await _context.SaveChangesAsync();

                    for (int i = 0; i < q.Options.Count; i++)
                    {
                        _context.Answers.Add(new Answer
                        {
                            QuestionId = question.QuestionId,
                            Content = q.Options[i],
                            IsCorrect = (i == q.CorrectIndex)
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                System.IO.File.Delete(filePath);

                TempData["Success"] = $"Upload thành công! Đã thêm {parsed.Questions.Count} câu hỏi.";
                return RedirectToAction("Library", "Exam");
            }
            catch (Exception ex)
            {
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                ViewBag.Error = "Lỗi parse file: " + ex.Message;
                return View("Index");
            }
        }
    }
}