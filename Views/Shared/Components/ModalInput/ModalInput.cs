using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Views.Shared.Components.ModalInput
{
    public class ModalInput : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View(new Models.Tasks());
        }
    }
}
