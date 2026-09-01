using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tp06.Models;

namespace tp06.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly BD BD = new BD();  // Solo esto

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    public IActionResult Index()
    {
        string emailSession = HttpContext.Session.GetString("UsuarioEmail");
        if (string.IsNullOrEmpty(emailSession))
        {
            return RedirectToAction("Login");
        }
        int salaActual = int.Parse(HttpContext.Session.GetString("SalaActual"));
        //hace un switch para redirigir a la sala correspondiente
        switch (salaActual)
        {
            case 1:
                return RedirectToAction("Minijuego1");
            case 2:
                return RedirectToAction("Minijuego2");
            case 3:
                return RedirectToAction("Minijuego3");
            default:
                return RedirectToAction("Index");
        }
    }



[HttpPost]
public IActionResult VerificarMinijuego(string codigo)
{
    int codigoIngresado = int.Parse(codigo);
    int idSala = int.Parse(HttpContext.Session.GetString("SalaActual"));
    int respuesta = BD.ObtenerRespuestaSala(idSala);
    int salaActual = int.Parse(HttpContext.Session.GetString("SalaActual"));
    if (codigoIngresado == respuesta)
    {
        int idPartida = int.Parse(HttpContext.Session.GetString("UsuarioPartida"));
        BD.ActualizarSalaActual(idPartida);

        int nuevaSala = idSala + 1;
        HttpContext.Session.SetString("SalaActual", nuevaSala.ToString());

        switch (nuevaSala)
        {
            case 2:
                return View("Minijuego2");
            case 3:
                return View("Minijuego3");
            default:
                return View("Victoria");
        }
    }
    else
    {
        ViewBag.Error = "Código incorrecto. Intenta nuevamente.";
        switch (salaActual)
        {
            case 1:
                return View("Minijuego1");
            case 2:
                return View("Minijuego2");
            case 3:
                return View("Minijuego3");
            default:
                return View("Index");
        }
    }
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
        Usuarios usuario = BD.ObtenerUsuarioPorEmail(HttpContext.Session.GetString("UsuarioEmail"));
        if (usuario == null)
        {
            return RedirectToAction("Login");
        }

        return View();
    }

    public IActionResult Tutorial()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string mail, string contrasena)
    {
        if (mail=="" || contrasena=="")
        {
            ViewBag.Error = "Por favor completa todos los campos";
            return View();
        }

        Usuarios usuario = BD.AutenticarUsuario(mail, contrasena);
        
        if (usuario != null)
        {
            // Guardar datos en sesión
            HttpContext.Session.SetString("UsuarioID", usuario.ID.ToString());
            HttpContext.Session.SetString("UsuarioEmail", usuario.mail);
            HttpContext.Session.SetString("UsuarioNombre", usuario.nombre);
            HttpContext.Session.SetString("UsuarioPartida", usuario.idPartida.ToString());
            HttpContext.Session.SetString("SalaActual", BD.ObtenerSalaActual(usuario).ToString());
            
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
