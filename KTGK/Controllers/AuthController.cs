using Microsoft.AspNetCore.Mvc;
using KTGK.Data;
using KTGK.Models;
using System.Linq;

public class AuthController : Controller
{
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var user = _context.Users
            .FirstOrDefault(u => u.Username == username && u.Password == password);

        if (user == null)
        {
            ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
            return View();
        }

        HttpContext.Session.SetString("role", user.Role);
        HttpContext.Session.SetString("username", user.Username);
        HttpContext.Session.SetInt32("UserId", user.UserId);

        if (user.Role == "Teacher")
            return RedirectToAction("Dashboard", "Teacher");

        return RedirectToAction("Library", "Exam");

        if (user.Role == "Student")
        
            return RedirectToAction("Dashboard", "Student");
        
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}