using Microsoft.AspNetCore.Mvc;
using TaskManager.Data;

namespace TaskManager.Views.Shared.Components.DeleteTask
{
    public class DeleteTask : ViewComponent
    {
        public dataContext _context { get; }
        public DeleteTask(dataContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(int id)
        {
            var task = _context.Tasks.Where(m=>m.id == id).FirstOrDefault();
            return View(task);
        }
    }
}
