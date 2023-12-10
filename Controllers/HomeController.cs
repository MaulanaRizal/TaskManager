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
            // Note :
            // Memuat data dari table Task dan mengurutkannya dari
            // tanggal terakhir diupdate terbesar berdasarkan kolom updateAt.

            try
            {
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
            // Note : Melakukan proses menambah data task

            try
            {
                if (!ModelState.IsValid) throw new Exception("Input task is invalid.");
                
                task.CreateAt = DateTime.Now;
                task.UpdateAt = DateTime.Now;

                //throw new Exception(); //Uncomment untuk uji error handling

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
            //Note : Melakuakn penghapusan data berdasarkan id
            try
            {
                var task = _context.Tasks.Where(m=>m.id == id).FirstOrDefault();
                
                _context.Remove(task);
                _context.SaveChanges();
                TempData["Success"] = $"Task {task.title} has been successfully removed.";

                //throw new Exception(); //Uncomment untuk uji error handling
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
            // Note : Melakukan proses update data berdasarkan id

            try
            {
                var task = _context.Tasks.Where(m => m.id == id).FirstOrDefault();
                if(task == null) throw new Exception("Task is doesn't exist.");
                
                var data = new
                {
                    status = "success",
                    task = task,
                };

                //throw new Exception(); //Uncomment untuk uji error handling

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

                //throw new Exception(); //Uncomment untuk uji error handling

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
                // Note : Melakuakn pencarian kata yang tersimpan pada kolom title atau deskripsi

                var q = query == null ? "" : query.ToLower();
                var result = (from t in _context.Tasks
                              let status = t.status.ToString()
                              //where t.title.Contains(q) || t.description.Contains(q) || t.status.ToString().Contains(q, StringComparison.OrdinalIgnoreCase)
                              where t.title.Contains(q) || t.description.Contains(q) 
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

                //throw new Exception(); //Uncomment untuk uji error handling

                return Json(result);
            }catch(Exception ex)
            {
                TempData["Failed"] = $"Failed, {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}