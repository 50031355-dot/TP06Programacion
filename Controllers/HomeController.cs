using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tp06.Models;

namespace tp06.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly BD BD = new BD();

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    private int ObtenerSesionInt(string clave)
    {
        string valor = HttpContext.Session.GetString(clave);
        if (string.IsNullOrEmpty(valor))
        {
            return 0;
        }

        return int.Parse(valor);
    }

    public int ObtenerPistasUsadas(int salaActual)
    {
        string key = "PistasSala" + salaActual;
        string valor = HttpContext.Session.GetString(key);

        if (string.IsNullOrEmpty(valor))
        {
            return 0;
        }
        int cantidad;
        return int.TryParse(valor, out cantidad) ? cantidad : 0;
    }

    private void GuardarPistasUsadas(int salaActual, int cantidad)
    {
        string key = "PistasSala" + salaActual;
        HttpContext.Session.SetString(key, cantidad.ToString());
    }

    public IActionResult Index()
    {
        string emailSession = HttpContext.Session.GetString("UsuarioEmail");
        if (string.IsNullOrEmpty(emailSession))
        {
            return RedirectToAction("Login");
        }

        int salaActual = ObtenerSesionInt("SalaActual");
        if (salaActual == 0)
        {
            salaActual = 1;
        }

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
    public IActionResult PedirPista()
    {
        string emailSession = HttpContext.Session.GetString("UsuarioEmail");
        if (string.IsNullOrEmpty(emailSession))
        {
            return RedirectToAction("Login");
        }

        int salaActual = ObtenerSesionInt("SalaActual");
        if (salaActual == 0)
        {
            salaActual = 1;
        }

        int pistasUsadas = ObtenerPistasUsadas(salaActual);

        if (pistasUsadas >= 3)
        {
            ViewBag.ErrorPista = "Ya pediste el máximo de 3 pistas para esta sala.";
            ViewBag.PistasUsadas = pistasUsadas;
            return View("Minijuego" + salaActual);
        }

        int siguienteNumero = pistasUsadas + 1;
        string pista = BD.ObtenerPistaSala(salaActual, siguienteNumero);

        GuardarPistasUsadas(salaActual, siguienteNumero);
        ViewBag.Pista = pista;
        ViewBag.PistaNumero = siguienteNumero;
        ViewBag.PistasUsadas = siguienteNumero;

        return View("Minijuego" + salaActual);
    }

    [HttpPost]
    public IActionResult VerificarMinijuego(string codigo)
    {
        int codigoIngresado = 0;
        if (!string.IsNullOrEmpty(codigo))
        {
            codigoIngresado = int.Parse(codigo);
        }

        int idSala = ObtenerSesionInt("SalaActual");
        if (idSala == 0)
        {
            idSala = 1;
        }

        int respuesta = BD.ObtenerRespuestaSala(idSala);
        int salaActual = idSala;

        if (codigoIngresado == respuesta)
        {
            int idPartida = ObtenerSesionInt("UsuarioPartida");
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

        int salaActual = ObtenerSesionInt("SalaActual");
        if (salaActual == 0)
        {
            salaActual = 1;
        }

        ViewBag.PistasUsadas = ObtenerPistasUsadas(salaActual);
        return View();
    }

    public IActionResult Minijuego2()
    {
        string emailSession = HttpContext.Session.GetString("UsuarioEmail");
        if (string.IsNullOrEmpty(emailSession))
        {
            return RedirectToAction("Login");
        }

        int salaActual = ObtenerSesionInt("SalaActual");
        if (salaActual == 0)
        {
            salaActual = 1;
        }

        ViewBag.PistasUsadas = ObtenerPistasUsadas(salaActual);
        return View();
    }

    public IActionResult Minijuego3()
    {
        Usuarios usuario = BD.ObtenerUsuarioPorEmail(HttpContext.Session.GetString("UsuarioEmail"));
        if (usuario == null)
        {
            return RedirectToAction("Login");
        }

        int salaActual = ObtenerSesionInt("SalaActual");
        if (salaActual == 0)
        {
            salaActual = 1;
        }

        ViewBag.PistasUsadas = ObtenerPistasUsadas(salaActual);
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
    public IActionResult Login(string mail)
    {
        if (mail == "" )
        {
            ViewBag.Error = "Por favor completa todos los campos";
            return View();
        }

        Usuarios usuario = BD.AutenticarUsuario(mail);

        if (usuario != null)
        {
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
