using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly dataContext _context;

        public HomeController(ILogger<HomeController> logger, dataContext dataContext)
        {
            _logger = logger;
            _context = dataContext;
        }

        public IActionResult Index()
        {
            try
            {
                //ViewBag.Success = TempData["Message"];
                // memuat data dari table Task dan mengurutkannya dari id terbesar berdasarkan kolom createAt
                var tasks = _context.Tasks.OrderByDescending(m=>m.CreateAt).ToList();
                var data = new {
                    Status = "Success",
                    Tasks = tasks,
            };

                //throw new Exception(); // ← note: uncomment untuk test error handling

                return View(data);
            }catch (Exception ex)
            {
                var data = new
                {
                    Status = "Failed",
                    Message = ex.Message,
                };
                return View(data);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Insert(Models.Tasks tasks)
        {
            try
            {
                if (!ModelState.IsValid) throw new Exception("Input task is invalid.");
                
                _context.Add(tasks);
                _context.SaveChanges();

                TempData["Success"] = $"Task {tasks.title} has been successfully saved.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Failed"] = $"Failed, {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}