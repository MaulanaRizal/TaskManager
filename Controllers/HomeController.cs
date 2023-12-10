using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly dataContext _context;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, dataContext dataContext, IConfiguration configuration)
        {
            _logger = logger;
            _context = dataContext;
            _configuration = configuration;
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

        private class modelTask
        {
            public int id { get; set; }

            public string? title { get; set; }

            public string? description { get; set; }

            public string status { get; set; }

            public DateTime CreateAt { get; set; }

            public DateTime UpdateAt { get; set; }
        }

        public async Task<IActionResult> Search(string query)
        {
            try
            {
                // Note :
                // Melakuakn pencarian kata yang tersimpan pada kolom title atau deskripsi atau status

                var q = query == null ? "" : query.ToLower();
                string conn = _configuration.GetConnectionString("DefaultConnection");
                var TaskList = new List<modelTask>();

                // InProgress = 0, Pending = 1, Completed = 2
                // ↓ proses generate query apabila parameter query mirip dengan status ↓
                string[] statusList = { "In Progress", "Pending", "Completed" };
                string indicesString = string.Join(",", statusList
                                             .Select((value, index) => new { Value = value, Index = index })
                                             .Where(item => item.Value.ToLower().Contains(q))
                                             .Select(item => item.Index));
                string querySearchStatus = indicesString.Length > 0 ? $"OR status in ({indicesString})" : "";

                using (MySqlConnection connection = new MySqlConnection(conn))
                {
                    connection.Open();

                    string sql = $"SELECT * FROM `tasks` where (title like @Param1 OR description like @Param2) {querySearchStatus} ";

                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Param1", $"%{q}%");
                        cmd.Parameters.AddWithValue("@Param2", $"%{q}%");

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            var temp = reader;
                            while (reader.Read())
                            {
                                var TaskItem = new modelTask();

                                TaskItem.id = (int)reader["id"];
                                TaskItem.title = reader["title"].ToString();
                                TaskItem.status = statusList[(int)reader["status"]];
                                TaskItem.description = reader["description"].ToString();
                                TaskItem.CreateAt =  DateTime.Parse(reader["CreateAt"].ToString());
                                TaskItem.UpdateAt = DateTime.Parse(reader["UpdateAt"].ToString());

                                TaskList.Add(TaskItem);

                            }
                        }
                    }
                    connection.Close();

                }

                //throw new Exception(); //Uncomment untuk uji error handling

                return Json(TaskList.OrderByDescending(m=>m.UpdateAt));
            }catch(Exception ex)
            {
                TempData["Failed"] = $"Failed, {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}