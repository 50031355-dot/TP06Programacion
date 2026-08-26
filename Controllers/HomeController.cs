using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tp06.Models;

namespace tp06.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ViewBag.salaActual = HttpContext.Session.GetInt32("SalaActual");
        return View();
    }

    public IActionResult Minijuego1()
    {
        
        return View();
    }

    public IActionResult Minijuego2()
    {
        return View();
    }

    public IActionResult Minijuego3()
    {
        return View();
    }

    publicIActionResult Tutorial()
    {
        
            if (salaActual == 2)
            {
                return RedirectToAction("Minijuego2");
            }
            else if (salaActual == 3)
            {
                return RedirectToAction("Minijuego3");
            }
        return View();
    }

    public IActionResult Login()
    {
        HttpContext.Session.SetInt32("SalaActual", idSala);
        return View();
    }

    public IActionResult Victoria()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
