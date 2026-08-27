using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tp06.Models;

namespace tp06.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly BD _bd;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
        _bd = new BD();
    }

    public IActionResult Index()
    {
        string emailSession = HttpContext.Session.GetString("UsuarioEmail");
        if (string.IsNullOrEmpty(emailSession))
        {
            return RedirectToAction("Login");
        }
        string salaActual = HttpContext.Session.GetString("SalaActual");
        return RedirectToAction(salaActual);
    }

    public IActionResult Minijuego1()
    {
        string emailSession = HttpContext.Session.GetString("UsuarioEmail");
        if (string.IsNullOrEmpty(emailSession))
        {
            return RedirectToAction("Login");
        }
        return View();
    }

    public IActionResult Minijuego2()
    {
                string emailSession = HttpContext.Session.GetString("UsuarioEmail");
        if (string.IsNullOrEmpty(emailSession))
        {
            return RedirectToAction("Login");
        }

        return View();
    }

    public IActionResult Minijuego3()
    {
                string emailSession = HttpContext.Session.GetString("UsuarioEmail");
        if (string.IsNullOrEmpty(emailSession))
        {
            return RedirectToAction("Login");
        }

        return View();
    }

    public IActionResult Tutorial()
    {
        int? salaActual = HttpContext.Session.GetInt32("SalaActual");
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
        return View();
    }

    [HttpPost]
    public IActionResult Login(string mail, string password)
    {
        if (mail=="" || password=="")
        {
            ViewBag.Error = "Por favor completa todos los campos";
            return View();
        }

        var usuario = _bd.AutenticarUsuario(mail, password);
        
        if (usuario != null)
        {
            // Guardar datos en sesión
            HttpContext.Session.SetInt32("UsuarioID", usuario.ID);
            HttpContext.Session.SetString("UsuarioEmail", usuario.mail);
            HttpContext.Session.SetString("UsuarioNombre", usuario.nombre);
            HttpContext.Session.SetInt32("UsuarioPartida", usuario.idPartida);
            HttpContext.Session.SetInt32("SalaActual", _bd.ObtenerSalaActual(usuario));
            
            return RedirectToAction("Index");
        }
        else
        {
            ViewBag.Error = "Email o contraseña inválidas";
            return View();
        }
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
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
