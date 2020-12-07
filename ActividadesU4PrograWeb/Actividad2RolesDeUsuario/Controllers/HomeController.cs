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
using Actividad2RolesDeUsuario.Models.ViewModels;
using Actividad2RolesDeUsuario.Repositories;

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
            // PARA PROBAR SIN EL DIRECTOR ALMACENADO EN LA BASE DE DATOS
            if (correo == "CorreoDirector@hotmail.com" && contraseña == "director")
            {
                List<Claim> Informacion = new List<Claim>();
                Informacion.Add(new Claim(ClaimTypes.Name, "Director"));
                Informacion.Add(new Claim(ClaimTypes.Role, "Director"));
                Informacion.Add(new Claim("NombreUsuario", "Saul Maldonado"));
                var claimsIdentity = new ClaimsIdentity(Informacion, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties { IsPersistent = recuerdame });
                return RedirectToAction("Index");
            }

            //if (correo == "CorreoDirector@hotmail.com")
            //{
            //    var usuario = context.Director.FirstOrDefault(x => x.Correo.ToUpper() == correo.ToUpper());
            //    if (usuario != null)
            //    {
            //        if (usuario.Correo.ToUpper() == correo.ToUpper() && usuario.Contrasena == HashingHelper.GetHash(contraseña))
            //        {
            //            List<Claim> Informacion = new List<Claim>();
            //            Informacion.Add(new Claim(ClaimTypes.Name, "Director"));
            //            Informacion.Add(new Claim(ClaimTypes.Role, "Director"));
            //            Informacion.Add(new Claim("NombreUsuario", usuario.Nombre));
            //            Informacion.Add(new Claim("IdUsuario", usuario.Id.ToString()));
            //            var claimsIdentity = new ClaimsIdentity(Informacion, CookieAuthenticationDefaults.AuthenticationScheme);
            //            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            //            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties { IsPersistent = recuerdame });
            //            return RedirectToAction("Index");
            //        }
            //        else
            //        {
            //            ModelState.AddModelError("", "El usuario o la contraseña no coincide.");
            //            return View();
            //        }
            //    }
            //    else
            //    {
            //        ModelState.AddModelError("", "No hay ningun usuario registado con ese correo electronico.");
            //        return View();
            //    }
            //}
            else // ES DOCENTE
            {
                MaestrosRepository repos = new MaestrosRepository(context);
                var usuario = repos.GetMaestroByCorreo(correo);
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

        [AllowAnonymous]
        public IActionResult Denegado()
        {
            return View();
        }

        [Authorize(Roles = "Director")]
        public IActionResult ListaMaestros()
        {
            MaestrosRepository reposMaestro = new MaestrosRepository(context);
            var maestros = reposMaestro.GetAll();
            return View(maestros);
        }

        [Authorize(Roles = "Director")]
        public IActionResult AltaMaestro()
        {
            return View();
        }

        [Authorize(Roles = "Director")]
        [HttpPost]
        public IActionResult AltaMaestro(RegistrarViewModel vm)
        {
            MaestrosRepository reposMaestro = new MaestrosRepository(context);
            try
            {
                if (vm.Maestro.Contrasena.ToString() == vm.ConfirmarContraseña.ToString())
                {
                    var contraHash = HashingHelper.GetHash(vm.Maestro.Contrasena);
                    vm.Maestro.Contrasena = contraHash;
                    reposMaestro.Insert(vm.Maestro);
                    return RedirectToAction("ListaMaestros");
                }
                else
                {
                    ModelState.AddModelError("", "Las contraseñas no coinciden");
                    return View(vm);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }

        [Authorize(Roles = "Director")]
        [HttpPost]
        public IActionResult EditarActivo(Maestro m)
        {
            MaestrosRepository repos = new MaestrosRepository(context);
            var original = repos.Get(m.Id);
            if (original != null)
            {
                if (original.Activo == 1)
                {
                    original.Activo = 0;
                    repos.Update(original);
                }
                else
                {
                    original.Activo = 1;
                    repos.Update(original);
                }

            }
            return RedirectToAction("ListaMaestros");
        }

        [Authorize(Roles = "Director")]
        public IActionResult CambiarContraseña(int id)
        {
            MaestrosRepository repos = new MaestrosRepository(context);
            RegistrarViewModel vm = new RegistrarViewModel();
            vm.Maestro = repos.Get(id);
            return View(vm);
        }
        [Authorize(Roles = "Director")]
        [HttpPost]
        public IActionResult CambiarContraseña(RegistrarViewModel vm)
        {
            MaestrosRepository repos = new MaestrosRepository(context);
            var original = repos.Get(vm.Maestro.Id);
            if (original != null)
            {
                if (vm.Maestro.Contrasena == vm.ConfirmarContraseña)
                {
                    var nuevaContraHash = HashingHelper.GetHash(vm.Maestro.Contrasena);
                    original.Contrasena = nuevaContraHash;
                    repos.Update(original);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Las contraseñas no coiniciden.");
                    return View(vm);
                }
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Director")]
        public IActionResult ModificarMaestro(int id)
        {
            MaestrosRepository repos = new MaestrosRepository(context);
            var maestro = repos.Get(id);
            if (maestro != null)
            {
                return View(maestro);
            }
            else
                return RedirectToAction("ListaMaestros");
        }

        [Authorize(Roles = "Director")]
        [HttpPost]
        public IActionResult ModificarMaestro(Maestro m)
        {
            MaestrosRepository repos = new MaestrosRepository(context);
            try
            {
                var maestro = repos.Get(m.Id);
                if (maestro != null)
                {
                    maestro.Nombre = m.Nombre;
                    maestro.Correo = m.Correo;
                    maestro.Grupo = m.Grupo;
                    repos.Update(maestro);
                    return RedirectToAction("ListaMaestros");
                }
                else
                    return RedirectToAction("ListaMaestros");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(m);
            }
        }

        [Authorize(Roles = "Director,Maestro")]
        public IActionResult AgregarAlumno()
        {
            return View();
        }

        [Authorize(Roles = "Director,Maestro")]
        [HttpPost]
        public IActionResult AgregarAlumno(Alumno a)
        {
            AlumnosRepository repos = new AlumnosRepository(context);
            try
            {
                a.IdMaestro = context.Maestro.FirstOrDefault(x => x.Grupo == a.Grupo).Id;
                repos.Insert(a);
                return RedirectToAction("ListaAlumnos");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(a);
            }
        }

        [Authorize(Roles = "Director,Maestro")]
        [HttpPost]
        public IActionResult EliminarAlumno(Alumno a)
        {
            AlumnosRepository repos = new AlumnosRepository(context);
            var original = repos.Get(a.Id);
            if (original != null)
            {
                repos.Delete(original);
            }
            return RedirectToAction("ListaAlumnos");
        }

        [Authorize(Roles = "Director,Maestro")]
        public IActionResult EditarAlumno(int id)
        {
            AlumnosRepository repos = new AlumnosRepository(context);
            var alumno = repos.Get(id);
            if (alumno != null)
            {
                return View(alumno);
            }
            else
                return RedirectToAction("ListaAlumnos");

        }

        [Authorize(Roles = "Director,Maestro")]
        [HttpPost]
        public IActionResult EditarAlumno(Alumno a)
        {
            AlumnosRepository repos = new AlumnosRepository(context);
            try
            {
                a.IdMaestro = context.Maestro.FirstOrDefault(x => x.Grupo == a.Grupo).Id;
                var original = repos.Get(a.Id);
                if (original != null)
                {
                    original.Nombre = a.Nombre;
                    original.Grupo = a.Grupo;
                    original.IdMaestro = a.IdMaestro;
                    repos.Update(original);
                }
                return RedirectToAction("ListaAlumnos");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(a);
            }
        }

        [Authorize(Roles = "Maestro,Director")]
        public IActionResult ListaAlumnos(int id)
        {
            if (User.IsInRole("Maestro"))
            {
                MaestrosRepository maestroRepos = new MaestrosRepository(context);
                var maestro = maestroRepos.Get(id);
                var alumnos = maestroRepos.GetAlumnosByGrupo(maestro.Grupo);
                return View(alumnos);
            }
            else
            {
                AlumnosRepository reposAlumno = new AlumnosRepository(context);
                var alumnos = reposAlumno.GetAll();
                return View(alumnos);
            }
        }
    }
}
