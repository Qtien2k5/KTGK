using Microsoft.AspNetCore.Mvc;
using KTGK.Services;

namespace KTGK.Controllers
{
    public class UploadController : Controller
    {
        private readonly WordParserService _parserService;

        public UploadController(WordParserService parserService)
        {
            _parserService = parserService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(string Title, int Duration, string Description, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("File không hợp lệ");

            var path = Path.Combine("uploads", file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            _parserService.ImportExam(path);

            return Content("Import đề TOEIC thành công");
        }
    }
}