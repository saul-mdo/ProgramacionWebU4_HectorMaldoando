using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Actividad2RolesDeUsuario.Models;
namespace Actividad2RolesDeUsuario.Repositories
{
    public class AlumnosRepository:Repository<Alumno>
    {
        public AlumnosRepository(rolesusuarioContext context) : base(context) { }

        public override bool Validate(Alumno entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad.Nombre))
            {
                throw new Exception("Introducir el nombre del alumno.");
            }
            if (string.IsNullOrWhiteSpace(entidad.Grupo))
            {
                throw new Exception("Asignar un grupo al alumno.");
            }
            if(Context.Alumno.Any(x=>x.Nombre.ToUpper() == entidad.Nombre.ToUpper() && x.Id != entidad.Id))
            {
                throw new Exception("Este alumno ya está registrado.");
            }
            return true;
        }

    }
}
