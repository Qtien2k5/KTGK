using KTGK.Data;
using KTGK.Models;
using KTGK.Services;
using Microsoft.AspNetCore.Mvc;

namespace KTGK.Controllers
{
    public class UploadController : Controller
    {
        private readonly WordParserService _parserService;

        private readonly ApplicationDbContext _context;

        public UploadController(WordParserService parserService, ApplicationDbContext context)
        {
            _parserService = parserService;
            _context = context;
        }


        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("role") != "Teacher")
                return RedirectToAction("Login", "Auth");

            return View();
        }


        [HttpPost]
        public IActionResult Upload(string Title, int Duration, string Description, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("File không hợp lệ");

            // 1. Tạo Exam
            var exam = new Exam
            {
                Title = Title,
                Duration = Duration,
                Description = Description
            };

            _context.Exams.Add(exam);
            _context.SaveChanges();

            // 2. Lưu file
            var path = Path.Combine("uploads", file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // 3. Parse + lưu câu hỏi
            _parserService.ImportExam(path, exam.ExamId);

            // 4. Quay về thư viện
            return RedirectToAction("Library", "Exam");
        }
    }
}