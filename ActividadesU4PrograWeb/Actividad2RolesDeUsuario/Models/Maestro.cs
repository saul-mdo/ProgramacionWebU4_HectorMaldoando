using System;
using System.Collections.Generic;

namespace Actividad2RolesDeUsuario.Models
{
    public partial class Maestro
    {
        public Maestro()
        {
            Alumno = new HashSet<Alumno>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public ulong Activo { get; set; }
        public string Grupo { get; set; }

        public virtual ICollection<Alumno> Alumno { get; set; }
    }
}
