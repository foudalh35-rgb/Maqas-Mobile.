using Microsoft.AspNetCore.Mvc;

namespace Autoparts.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
