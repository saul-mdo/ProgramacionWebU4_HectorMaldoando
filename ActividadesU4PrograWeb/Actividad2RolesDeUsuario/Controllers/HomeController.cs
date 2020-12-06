using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Actividad2RolesDeUsuario.Models;
using Microsoft.AspNetCore.Authentication;
using Actividad2RolesDeUsuario.Helpers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Actividad2RolesDeUsuario.Controllers
{
    public class HomeController : Controller
    {
        rolesusuarioContext context;
        public HomeController(rolesusuarioContext ctx)
        {
            context = ctx;
        }

        [Authorize(Roles = "Maestro,Director")]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> IniciarSesion(string correo, string contraseña, bool recuerdame)
        {
            if (correo == "CorreoDirector@hotmail.com")
            {
                var usuario = context.Director.FirstOrDefault(x => x.Correo.ToUpper() == correo.ToUpper());
                if (usuario != null)
                {
                    if (usuario.Correo.ToUpper() == correo.ToUpper() && usuario.Contrasena == HashingHelper.GetHash(contraseña))
                    {
                        List<Claim> Informacion = new List<Claim>();
                        Informacion.Add(new Claim(ClaimTypes.Name, "Director"));
                        Informacion.Add(new Claim(ClaimTypes.Role, "Director"));
                        Informacion.Add(new Claim("NombreUsuario", usuario.Nombre));
                        Informacion.Add(new Claim("IdUsuario", usuario.Id.ToString()));
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
                else
                {
                    ModelState.AddModelError("", "No hay ningun usuario registado con ese correo electronico.");
                    return View();
                }
            }
            else // ES DOCENTE
            {
                var usuario = context.Maestro.FirstOrDefault(x => x.Correo.ToUpper() == correo.ToUpper());
                if (usuario != null)
                {
                    if (usuario.Activo == 1)
                    {
                        if (usuario.Correo.ToUpper() == correo.ToUpper() && usuario.Contrasena == HashingHelper.GetHash(contraseña))
                        {
                            List<Claim> Informacion = new List<Claim>();
                            Informacion.Add(new Claim(ClaimTypes.Name, "Maestro"));
                            Informacion.Add(new Claim(ClaimTypes.Role, "Maestro"));
                            Informacion.Add(new Claim("NombreUsuario", usuario.Nombre));
                            Informacion.Add(new Claim("IdUsuario", usuario.Id.ToString()));
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
                    else
                    {
                        ModelState.AddModelError("", $"La cuenta del usuario {usuario.Nombre} no está activa.");
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "No hay ningun usuario registado con ese correo electronico.");
                    return View();
                }
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }

        //[AllowAnonymous]
        //public IActionResult Denegado()
        //{
        //    return View();
        //}

        //[Authorize(Roles = "Director")]
        //public IActionResult CambiarContraseñaMestro()
        //{
        //    return View();
        //}

        //[Authorize(Roles = "Director")]
        //public IActionResult AltaDocente()
        //{
        //    return View();
        //}


        //[Authorize(Roles = "Director")]
        //public IActionResult ModificarDocente()
        //{
        //    return View();
        //}

        //[Authorize(Roles = "Director,Maestro")]
        //public IActionResult AgregarAlumno()
        //{
        //    return View();
        //}

        //[Authorize(Roles = "Director,Maestro")]
        //public IActionResult EliminarAlumno()
        //{
        //    return View();
        //}

        //[Authorize(Roles = "Director,Maestro")]
        //public IActionResult EditarAlumno()
        //{
        //    return View();
        //}

    }
}
