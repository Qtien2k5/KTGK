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

            // 🔒 kiểm tra file Word
            if (!file.FileName.EndsWith(".docx"))
                return Content("Chỉ chấp nhận file .docx");

            // 1️⃣ Tạo Exam
            var exam = new Exam
            {
                Title = Title,
                Duration = Duration,
                Description = Description
            };

            _context.Exams.Add(exam);
            _context.SaveChanges();

            // 2️⃣ Tạo folder nếu chưa có
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            Directory.CreateDirectory(uploadFolder);

            // 3️⃣ Tạo tên file tránh trùng
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(uploadFolder, fileName);

            // 4️⃣ Lưu file
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // 5️⃣ Parse + lưu DB
            try
            {
                _parserService.ImportExam(fullPath, exam.ExamId);
            }
            catch (Exception ex)
            {
                return Content("Lỗi đọc file: " + ex.Message);
            }

            // 6️⃣ Redirect
            return RedirectToAction("Library", "Exam");
        }
    }
}