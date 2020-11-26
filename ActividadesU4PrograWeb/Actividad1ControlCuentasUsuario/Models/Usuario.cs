using System;
using System.Collections.Generic;

namespace Actividad1ControlCuentasUsuario.Models
{
    public partial class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
    }
}
