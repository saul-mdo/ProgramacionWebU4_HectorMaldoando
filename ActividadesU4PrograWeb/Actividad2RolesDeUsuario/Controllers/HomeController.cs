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
            if (correo == "CorreoDirector@hotmail.com")
            {
                var usuario = context.Director.FirstOrDefault(x => x.Correo.ToUpper() == correo.ToUpper());
                if (usuario != null)
                {
                    var contraHash = HashingHelper.GetHash(contraseña).ToUpper();
                    if (usuario.Correo.ToUpper() == correo.ToUpper() && usuario.Contrasena == contraHash)
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
            var maestros = reposMaestro.GetAll().OrderBy(x=>x.Nombre);
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
                    if (context.Maestro.Any(x => x.Grupo == m.Grupo && x.Id!=m.Id))
                    {
                        ModelState.AddModelError("", "Ya hay un maestro registrado con ese grupo");
                        return View(m);
                    }
                    else
                    {
                        maestro.Grupo = m.Grupo;
                    }
                    repos.Update(maestro);
                    return RedirectToAction("ListaMaestros");
                }
                else
                    return RedirectToAction("ListaMaestros");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(m);
            }
        }

        [Authorize(Roles = "Director,Maestro")]
        public IActionResult AgregarAlumno(int? id)
        {
            RegistrarViewModel vm = new RegistrarViewModel();
            if (User.IsInRole("Maestro"))
            {
                vm.IdMaestro = (int)id;
                return View(vm);
            }
            return View(vm);
        }

        [Authorize(Roles = "Director,Maestro")]
        [HttpPost]
        public IActionResult AgregarAlumno(RegistrarViewModel vm)
        {
            AlumnosRepository repos = new AlumnosRepository(context);
            try
            {
                MaestrosRepository maestroRepos = new MaestrosRepository(context);

                var maestro = maestroRepos.Get(vm.IdMaestro);

                if (maestro == null)
                {
                    if (User.IsInRole("Director"))
                    {
                        var GrupoMaestro = context.Maestro.FirstOrDefault(x => x.Grupo == vm.Alumno.Grupo);
                        if (GrupoMaestro == null)
                        {
                            ModelState.AddModelError("", "Aún no hay ningun maestro asignado a ese grupo.");
                            return View(vm);
                        }
                        else
                        {
                            vm.Alumno.IdMaestro = GrupoMaestro.Id;
                            repos.Insert(vm.Alumno);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Aún no hay ningun maestro asignado a ese grupo.");
                        return View(vm);
                    }
                }
                else
                {
                    if (context.Maestro.Any(x => x.Grupo == vm.Alumno.Grupo))
                    {

                        if (maestro.Grupo != vm.Alumno.Grupo)
                        {
                            ModelState.AddModelError("", "Usted no tiene permitido agregar alumnos a dicho grupo");
                            return View(vm);
                        }


                        vm.Alumno.IdMaestro = vm.IdMaestro;
                        repos.Insert(vm.Alumno);

                    }
                }

                if (User.IsInRole("Maestro"))
                {
                    return RedirectToAction("ListaAlumnos", new { id = vm.IdMaestro });
                }
                else
                    return RedirectToAction("ListaAlumnos");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
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
            if (User.IsInRole("Maestro"))
            {
                var id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "IdUsuario").Value);
                return RedirectToAction("ListaAlumnos", new { id = id });
            }
            else
            {
                return RedirectToAction("ListaAlumnos");
            }

        }

        [Authorize(Roles = "Director,Maestro")]
        public IActionResult EditarAlumno(int id)
        {
            AlumnosRepository repos = new AlumnosRepository(context);
            RegistrarViewModel vm = new RegistrarViewModel();
            vm.Alumno = repos.Get(id);

            if (User.IsInRole("Maestro"))
            {
                vm.IdMaestro = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "IdUsuario").Value);
            }

            if (vm.Alumno != null)
            {
                return View(vm);
            }
            else
            {
                if (User.IsInRole("Maestro"))
                {
                    return RedirectToAction("ListaAlumnos", new { id = vm.IdMaestro });

                }
                else
                {
                    return RedirectToAction("ListaAlumnos");
                }
            }

        }

        [Authorize(Roles = "Director,Maestro")]
        [HttpPost]
        public IActionResult EditarAlumno(RegistrarViewModel vm)
        {
            AlumnosRepository repos = new AlumnosRepository(context);
            try
            {
                var m = context.Maestro.FirstOrDefault(x => x.Grupo == vm.Alumno.Grupo);
                var original = repos.Get(vm.Alumno.Id);
                if (original != null)
                {
                    original.Nombre = vm.Alumno.Nombre;
                    original.Grupo = vm.Alumno.Grupo;
                    if (m == null)
                    {
                        ModelState.AddModelError("", "Aún no hay ningun maestro asignado a ese grupo.");
                        return View(vm);
                    }
                    else
                    {
                        original.IdMaestro = m.Id;
                    }
                    repos.Update(original);
                }
                if (User.IsInRole("Maestro"))
                {
                    return RedirectToAction("ListaAlumnos", new { id = vm.IdMaestro });
                }
                else
                {
                    return RedirectToAction("ListaAlumnos");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }

        [Authorize(Roles = "Maestro,Director")]
        public IActionResult ListaAlumnos(int? id)
        {
            if (User.IsInRole("Maestro"))
            {
                MaestrosRepository maestroRepos = new MaestrosRepository(context);
                var maestro = maestroRepos.Get(id);
                var alumnos = maestroRepos.GetAlumnosByGrupo(maestro.Grupo).OrderBy(x=>x.Nombre);
                return View(alumnos);
            }
            else
            {
                AlumnosRepository reposAlumno = new AlumnosRepository(context);
                var alumnos = reposAlumno.GetAll().OrderBy(x => x.Grupo);
                return View(alumnos);
            }
        }
    }
}
