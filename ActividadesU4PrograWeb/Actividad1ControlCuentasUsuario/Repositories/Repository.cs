using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Actividad1ControlCuentasUsuario.Models;

namespace Actividad1ControlCuentasUsuario.Repositories
{
    public class Repository
    {
        public controlusuariosContext Context { get; set; }
        public Repository(controlusuariosContext ctx)
        {
            Context = ctx;
        }

        public Usuario GetUsuario(Usuario id)
        {
            return Context.Find<Usuario>(id);
        }

        public virtual void Insert(Usuario entidad)
        {
            if (Validar(entidad))
            {
                Context.Add(entidad);
                Context.SaveChanges();
            }
        }
        public virtual void Update(Usuario entidad)
        {
            if (Validar(entidad))
            {
                Context.Update(entidad);
                Context.SaveChanges();
            }
        }

        public bool Validar(Usuario entidad)
        {
            if (string.IsNullOrEmpty(entidad.NombreUsuario))
            {
                throw new Exception("Introduzca su nombre de usuario.");
            }
            if (string.IsNullOrEmpty(entidad.Correo))
            {
                throw new Exception("Introduzca su correo electronico de usuario.");
            }
            if (string.IsNullOrEmpty(entidad.Contrasena))
            {
                throw new Exception("Introduzca su contraseña de usuario.");
            }
            if ((Context.Usuario.Any(x => x.Correo.ToUpper() == entidad.Correo.ToUpper()&&x.Id!=entidad.Id)))
            {
                throw new Exception("Ya hay un usuario registrado con este correo electronico.");
            }
            return true;
        }
    }
}
