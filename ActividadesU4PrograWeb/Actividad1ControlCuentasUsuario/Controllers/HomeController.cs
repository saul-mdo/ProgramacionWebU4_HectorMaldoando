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
                    controlusuariosContext context = new controlusuariosContext();
                    Repository repos = new Repository(context);
                    var hashContraseña = HashingHelper.GetHash(vm.Usuario.Contrasena);
                    vm.Usuario.Contrasena = hashContraseña;

                    var codigo = GenerarCodigoHelper.GenerarCodigo();
                    
                    //

                    //vm.Usuario.Activo = 1;
                    repos.Insert(vm.Usuario);
                    // return RedirectToAction("IniciarSesion");



                    // MANDO EL CORREO CON UN CODIGO GENERADO RANDOM.
                    MailMessage message = new MailMessage();
                    message.From = new MailAddress("noreply@sistemas171.com", "Cuenta Confirmación de Sistemas171");
                    message.To.Add(vm.Usuario.Correo);
                    message.Subject = "Confirmación de Registro";
                    message.Body = "¡Bienvenido a Sistemas171!<br/> Introduzca el siguiente codigo en la ventana de confirmación para activar su cuenta:<br/>"; //INSERTAR CODIGO
                    message.IsBodyHtml = true;
                    SmtpClient client = new SmtpClient("mail.sistemas171.com", 2525);
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("noreply@sistemas171.com", "##ITESRC2020");
                    client.Send(message);
                    return RedirectToAction("ActivarCuenta", vm);
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

            // LO MANDO A OTRA VISTA, UNA ESPERA MIENTRAS INTRODUCE EL CODIGO QUE LLEGÓ AL CORREO. LA VISTA DE ESPERA TENDRÁ UN INPUT PARA INTRODUCIR EL CODIGO.

            // VALIDA SI ES EL MISMO CODIGO. SI ES EL MISMO, ENTONCES HACE UN UPDATE A ACTIVO=TRUE.
        }
    }
}
