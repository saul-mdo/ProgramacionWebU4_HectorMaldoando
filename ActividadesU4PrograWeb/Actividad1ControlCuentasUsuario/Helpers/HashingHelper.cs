using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Actividad1ControlCuentasUsuario.Helpers
{
    public static class HashingHelper
    {
        public static string GetHash(string cadena)
        {
            var alg = SHA256.Create();
            byte[] codificar = Encoding.UTF8.GetBytes(cadena);
            byte[] hash = alg.ComputeHash(codificar);

            string cadenaHash = "";
            foreach (var item in hash)
            {
                cadenaHash += item.ToString("x2");
            }
            return cadenaHash;
        }
    }
}
