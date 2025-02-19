﻿using System;
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
                        Informacion.Add(new Claim("IdUsuario", user.Id.ToString()));
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
        public IActionResult CrearCuenta(CuentaViewModel vm)
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
                    message.From = new MailAddress("testingcodesistemas@gmail.com", "Sistemas171");
                    message.To.Add(vm.Usuario.Correo);
                    message.Subject = "Confirmación de Registro";
                    message.Body = $"¡Bienvenido a Sistemas171!<br/> Introduzca el siguiente codigo en la ventana de confirmación para activar su cuenta: {codigo}";
                    message.IsBodyHtml = true;
                    SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("testingcodesistemas@gmail.com", "correofake1");
                    client.Send(message);

                    return RedirectToAction("ValidarCodigo", "Home", new { Id = vm.Usuario.Id });
                }
                else
                {
                    ModelState.AddModelError("", "Las contraseñas no coiniciden.");
                    return View(vm);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        } 

        [AllowAnonymous]
        public IActionResult ValidarCodigo(int Id)
        {
            ValidarCodigoViewModel avm = new ValidarCodigoViewModel();
            avm.Id = Id;
            return View(avm);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ValidarCodigo(ValidarCodigoViewModel avm)
        {
            Repository repos = new Repository(context); 

            var original = repos.GetUsuarioById(avm.Id);

            try
            {
                if (original != null)
                {
                    if (original.Codigo == avm.codigoConfirmacion)
                    {
                        if (original.Activo == 1)
                        {
                            return RedirectToAction("CambiarContraseña", "Home", new { id = original.Id });
                        }
                        else
                        {
                            original.Activo = 1;
                            repos.Update(original);
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "El codigo no coincide. No se ha podido realizar la acción.");
                        return View(avm);
                    }
                }
                else
                {
                    return RedirectToAction("IniciarSesion");
                }
            }catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(avm);
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

        public IActionResult CambiarContraseña(int id)
        {
            CuentaViewModel vm = new CuentaViewModel();
            Repository repos = new Repository(context);
            vm.Usuario = repos.GetUsuarioById(id);

            return View(vm);
        }

        [HttpPost]
        public IActionResult CambiarContraseña(CuentaViewModel vm)
        {
            Repository repos = new Repository(context);
            var original = repos.GetUsuarioById(vm.Usuario.Id);
            var contraseñaHash = HashingHelper.GetHash(vm.Usuario.Contrasena);
            if (original != null)
            {
                if (vm.Usuario.Contrasena == vm.ConfirmarContraseña)
                {
                    original.Contrasena = contraseñaHash;
                    repos.Update(original);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Las contraseñas no coiniciden.");
                    return View(vm);
                }
            }
            else
            {
                return RedirectToAction("Index");
            }


        }

        [AllowAnonymous]
        public IActionResult RecuperarContraseña()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult RecuperarContraseña(Usuario u)
        {
            Repository repos = new Repository(context);
            try
            {
                if (u.Correo != null)
                {
                    var original = repos.GetUsuarioByCorreo(u.Correo);
                    var codigo = GenerarCodigoHelper.GenerarCodigo();
                    original.Codigo = codigo;
                    repos.Update(original);
                    // MANDO EL CORREO CON UN CODIGO GENERADO RANDOM.
                    MailMessage message = new MailMessage();
                    message.From = new MailAddress("testingcodesistemas@gmail.com", "Sistemas171");
                    message.To.Add(original.Correo);
                    message.Subject = "Recuperación de Contraseña";
                    message.Body = $"Hemos recibido su solicitud para cambiar su contraseña.<br/> Introduzca el siguiente codigo en la ventana de confirmación para modificar su contraseña: {codigo}";
                    message.IsBodyHtml = true;
                    SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("testingcodesistemas@gmail.com", "correofake1");
                    client.Send(message);
                    return RedirectToAction("ValidarCodigo", "Home", new { id = original.Id });
                }
                else
                {
                    ModelState.AddModelError("", "Introduzca el correo.");
                    return View(u);
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(u);
            }
        }
    }
}
