using Microsoft.AspNetCore.Mvc;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Views.Shared.Components.ModalUpdate
{
    public class ModalUpdate : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View(new Models.Tasks());
        }

    }
}
