using System;
using System.Collections.Generic;

namespace Actividad1ControlCuentasUsuario.Models
{
    public partial class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public ulong Activo { get; set; }
        public string Codigo { get; set; }
    }
}
