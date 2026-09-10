using Microsoft.AspNetCore.Mvc;

namespace Maqas.MVC.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Redirect("/login/login.html");
    }
}
