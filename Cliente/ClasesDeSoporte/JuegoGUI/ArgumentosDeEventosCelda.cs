using System;

namespace Cliente
{
    public class ArgumentosDeEventosCelda : EventArgs
    {
        public int CeldaFila { get; set; }
        public int CeldaColumna { get; set; }

        public ArgumentosDeEventosCelda(int fila, int columna)
        {
            this.CeldaFila = fila;
            this.CeldaColumna = columna;
        }
    }
}
