using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Views.Shared.Components.FomInput
{
    public class FormInput : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View(new Models.Tasks());
        }
    }
}
