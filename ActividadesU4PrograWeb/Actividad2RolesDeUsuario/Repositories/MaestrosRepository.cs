using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Actividad2RolesDeUsuario.Models;

namespace Actividad2RolesDeUsuario.Repositories
{
    public class MaestrosRepository:Repository<Maestro>
    {
        public MaestrosRepository(rolesusuarioContext context) : base(context) { }

        public Maestro GetMaestroByCorreo(string correo)
        {
            return Context.Maestro.FirstOrDefault(x => x.Correo.ToUpper() == correo.ToUpper());
        }

        public override bool Validate(Maestro entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad.Nombre))
            {
                throw new Exception("Introducir el nombre del maesto.");
            }
            if(string.IsNullOrWhiteSpace(entidad.Correo))
            {
                throw new Exception("Asigne un correo electronico al maestro.");
            }
            if (string.IsNullOrWhiteSpace(entidad.Contrasena))
            {
                throw new Exception("Debe asignarle una contraseña al docente.");
            }
            if (string.IsNullOrWhiteSpace(entidad.Grupo))
            {
                throw new Exception("El maestro debe tener un grupo asignado.");
            }
            return true;
        }
    }
}
