using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Actividad2RolesDeUsuario.Models.ViewModels
{
    public class RegistrarViewModel
    {
        public Maestro Maestro { get; set; }
        public Alumno Alumno { get; set; }
        public string ConfirmarContraseña { get; set; }
        public int IdMaestro { get; set; }
    }
}
