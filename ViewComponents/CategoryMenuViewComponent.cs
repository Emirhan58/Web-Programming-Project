using WebProgrammingProject.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace WebProgrammingProject.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IEnumerable categories) {
            if(RouteData.Values["action"].ToString() == "Index") {
                ViewBag.SelectedCategory = RouteData?.Values["id"];
            }
            return View(categories);
        }
    }
}