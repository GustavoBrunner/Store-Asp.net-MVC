using Microsoft.AspNetCore.Mvc;

namespace EstoqueWeb.Controllers;
public class HomeController : Controller{
    public IActionResult Index(){
        return View();
    }
}