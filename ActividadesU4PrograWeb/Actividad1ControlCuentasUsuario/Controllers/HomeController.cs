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
using System.Net.Mail;
using System.Net;
using Actividad1ControlCuentasUsuario.Models.ViewModels;
using Actividad1ControlCuentasUsuario.Repositories;
using Actividad1ControlCuentasUsuario.Helpers;

namespace Actividad1ControlCuentasUsuario.Controllers
{
    public class HomeController : Controller
    {
        controlusuariosContext context;

        public HomeController(controlusuariosContext ctx)
        {
            context = ctx;
        }


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
            var user = context.Usuario.FirstOrDefault(x => x.Correo.ToUpper() == correo.ToUpper());
            if (user != null)
            {
                if (user.Activo == 0)
                {
                    ModelState.AddModelError("", $"La cuenta del usuario {user.NombreUsuario} no está activa.");
                    return View();
                }
                else
                {
                    if (user.Correo.ToUpper() == correo.ToUpper() && user.Contrasena == HashingHelper.GetHash(contraseña))
                    {
                        List<Claim> Informacion = new List<Claim>();
                        Informacion.Add(new Claim(ClaimTypes.Name, "Usuario Registrado"));
                        Informacion.Add(new Claim(ClaimTypes.Role, "UsuarioRegistrado"));
                        Informacion.Add(new Claim("NombreUsuario", user.NombreUsuario));
                        Informacion.Add(new Claim("IdUsuario",user.Id.ToString()));
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
        public IActionResult CrearCuenta(CrearCuentaViewModel vm)
        {
            //  LO AGREGO A LA BD PERO CON EL CAMPO ACTIVO EN FALSO.
            try
            {
                if (vm.ConfirmarContraseña == vm.Usuario.Contrasena)
                {
                    Repository repos = new Repository(context);
                    var hashContraseña = HashingHelper.GetHash(vm.Usuario.Contrasena);
                    vm.Usuario.Contrasena = hashContraseña;

                    var codigo = GenerarCodigoHelper.GenerarCodigo();

                    vm.Usuario.Codigo = codigo;

                    repos.Insert(vm.Usuario);


                    // MANDO EL CORREO CON UN CODIGO GENERADO RANDOM.
                    MailMessage message = new MailMessage();
                    message.From = new MailAddress("sistemascomputacionales7g@gmail.com", "Sistemas171");
                    message.To.Add(vm.Usuario.Correo);
                    message.Subject = "Confirmación de Registro";
                    message.Body = $"¡Bienvenido a Sistemas171!<br/> Introduzca el siguiente codigo en la ventana de confirmación para activar su cuenta: {codigo}";
                    message.IsBodyHtml = true;
                    SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("sistemascomputacionales7g@gmail.com", "sistemas7g");
                    client.Send(message);

                    return RedirectToAction("ActivarCuenta", "Home", new { Id= vm.Usuario.Id });
                }
                else
                {
                    ModelState.AddModelError("", "Las contraseñas no coiniciden.");
                    return View(vm);
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }

        [AllowAnonymous]
        public IActionResult ActivarCuenta(int Id)
        {
            ActivacionCuentaViewModel avm = new ActivacionCuentaViewModel();
            avm.Id = Id;
            return View(avm);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ActivarCuenta(ActivacionCuentaViewModel avm)
        {
            Repository repos = new Repository(context);
            var original = repos.GetUsuarioById(avm.Id);
            if (original != null)
            {
                if (original.Codigo == avm.codigoConfirmacion)
                {
                    original.Activo = 1;
                    repos.Update(original);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("","El codigo no coincide. No se ha podido activar la cuenta.");
                    return View(avm);
                }
            }
            else
            {
                return RedirectToAction("IniciarSesion");
            }
        }
        
        [Authorize(Roles = "UsuarioRegistrado")]
        public IActionResult EliminarCuenta(int id)
        {
            Repository repos = new Repository(context);
            var u = repos.GetUsuarioById(id);
            if (u != null)
            {
                return View(u);
            }
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "UsuarioRegistrado")]
        public IActionResult EliminarCuenta(Usuario u)
        {
            Repository repos = new Repository(context);
            var usuario = repos.GetUsuarioById(u.Id);
            if (usuario != null)
            {
                HttpContext.SignOutAsync();
                repos.Delete(usuario);
                return RedirectToAction("IniciarSesion");
            }
            else
            {
                return View();
            }
        }

        public IActionResult CambiarContraseña()
        {
            return View();
        }
    }
}
