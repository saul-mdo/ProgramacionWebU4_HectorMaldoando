using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Actividad1ControlCuentasUsuario.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Actividad1ControlCuentasUsuario.Controllers
{
    public class HomeController : Controller
    {
        [Authorize(Roles = "UsuarioRegistrado")]
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> IniciarSesion(string correo, string contraseña, bool recuerdame)
        {
            controlusuariosContext context = new controlusuariosContext();
            var user = context.Usuario.FirstOrDefault(x => x.Correo.ToUpper() == correo.ToUpper());
            if (user != null)
            {
                if (user.Activo == 0)
                {
                    ModelState.AddModelError("", $"La cuenta de {user.NombreUsuario} no está activa.");
                    return View();
                }
                else
                {
                    if (user.Correo.ToUpper() == correo.ToUpper() && user.Contrasena.ToUpper() == contraseña.ToUpper())
                    {
                        List<Claim> Informacion = new List<Claim>();
                        Informacion.Add(new Claim(ClaimTypes.Name, "Usuario Registrado"));
                        Informacion.Add(new Claim(ClaimTypes.Role, "UsuarioRegistrado"));
                        Informacion.Add(new Claim("NombreUsuario", user.NombreUsuario));
                        var claimsIdentity = new ClaimsIdentity(Informacion, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties { IsPersistent = recuerdame });
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "El usuario o la contraseña no coincide.");
                        return View();
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "No hay ningun usuario registado con ese correo electronico.");
                return View();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public IActionResult Denegado()
        {
            return View();
        }










        [AllowAnonymous]
        public IActionResult CrearCuenta()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult CrearCuenta(Usuario u, string confirmacionContraseña)
        {
            //  LO AGREGO A LA BD PERO CON EL CAMPO ACTIVO EN FALSO.

            // MANDO EL CORREO CON UN CODIGO GENERADO RANDOM.

            // LO MANDO A OTRA VISTA, UNA ESPERA MIENTRAS INTRODUCE EL CODIGO QUE LLEGÓ AL CORREO. LA VISTA DE ESPERA TENDRÁ UN INPUT PARA INTRODUCIR EL CODIGO.

            // VALIDA SI ES EL MISMO CODIGO. SI ES EL MISMO, ENTONCES HACE UN UPDATE A ACTIVO=TRUE.
            return View();
        }
    }
}
