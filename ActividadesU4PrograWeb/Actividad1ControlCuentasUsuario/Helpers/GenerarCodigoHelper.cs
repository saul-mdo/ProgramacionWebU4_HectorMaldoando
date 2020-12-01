using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Actividad1ControlCuentasUsuario.Helpers
{
    public class GenerarCodigoHelper
    {
        public static string GenerarCodigo()
        {
            Random r = new Random();
            string numerorandom = r.Next().ToString();
            byte[] datos = System.Text.Encoding.UTF8.GetBytes(numerorandom);
            string salida = Convert.ToBase64String(datos);
            return salida;
        }
    }
}
