using System;

namespace Cliente
{
    public class BuscaminasExcepcion : ApplicationException
    {
        public BuscaminasExcepcion(string mensaje)
            : base(mensaje)
        {
        }

        public BuscaminasExcepcion(string mensaje, Exception excepcionInterna)
            : base(mensaje, excepcionInterna)
        {
        }
    }
}
