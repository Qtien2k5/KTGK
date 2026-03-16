using Microsoft.AspNetCore.Mvc;
using KTGK.Data;
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
            var exams = _context.Exams.ToList();
            return View(exams);
        }
    }
}