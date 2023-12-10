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
                // memuat data dari table Task dan mengurutkannya dari tanggal terakhir diupdate terbesar berdasarkan kolom updateAt
                var tasks = _context.Tasks.OrderByDescending(m=>m.UpdateAt);
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
        public IActionResult Insert(Models.Tasks task)
        {
            try
            {
                if (!ModelState.IsValid) throw new Exception("Input task is invalid.");
                
                task.CreateAt = DateTime.Now;
                task.UpdateAt = DateTime.Now;

                _context.Add(task);
                _context.SaveChanges();

                TempData["Success"] = $"Task {task.title} has been successfully saved.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Failed"] = $"Failed, {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Delete(int id)
        {
            try
            {
                var task = _context.Tasks.Where(m=>m.id == id).FirstOrDefault();
                
                _context.Remove(task);
                _context.SaveChanges();
                TempData["Success"] = $"Task {task.title} has been successfully removed.";

                //throw new Exception();
                return RedirectToAction("Index");
            }catch(Exception ex)
            {
                TempData["Failed"] = $"Failed, {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            try
            {
                
                var task = _context.Tasks.Where(m => m.id == id).FirstOrDefault();
                if(task == null) throw new Exception("Task is doesn't exist.");
                
                var data = new
                {
                    status = "success",
                    task = task,
                };

                return Json(data);
            }catch (Exception ex)
            {
                var data = new
                {
                    status = "failed",
                    message = ex.Message,
                };
                
                return Json(data);
            }
        }

        [HttpPost]
        public IActionResult Update(int id, Tasks task)
        {
            try
            {

                var newTask = _context.Tasks.Where(m => m.id == id).FirstOrDefault();

                if (!ModelState.IsValid) throw new Exception("Input task is invalid.");
                if (newTask == null) throw new Exception("Please update your task again.");

                newTask.title = task.title;
                newTask.description = task.description;
                newTask.status = task.status;
                newTask.UpdateAt = DateTime.Now;

                _context.Update(newTask);
                _context.SaveChanges();

                TempData["Success"] = $"Task {task.title} has been successfully saved.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Failed"] = $"Failed, {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Search(string query)
        {
            try
            {
                var q = query == null ? "" : query.ToLower();
                var temp = _context.Tasks;
                var result = (from t in _context.Tasks
                              let status = t.status.ToString()
                              where t.title.Contains(q) || t.description.Contains(q) || t.status.ToString().Contains(q, StringComparison.OrdinalIgnoreCase)
                              select new
                              {
                                  t.id,
                                  t.title,
                                  status,
                                  t.description,
                                  t.CreateAt,
                                  t.UpdateAt

                              })
                              .OrderByDescending(m=>m.UpdateAt);

                return Json(result);
            }catch(Exception ex)
            {
                TempData["Failed"] = $"Failed, {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}