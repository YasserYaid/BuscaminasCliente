namespace Cliente
{
    public class Celda
    {
        public TableroBuscaminas tableroJuego { get; set; }
        public int filaPosicion { get; set; }
        public int columnaPosicion { get; set; }
        public bool esMarcada { get; set; }
        public bool esMinada { get; set; }
        public bool esRevelada { get; set; }

        public Celda(TableroBuscaminas tableroJuego, int filaPosicion, int columnaPosicion)
        {
            this.tableroJuego = tableroJuego;
            this.filaPosicion = filaPosicion;
            this.columnaPosicion = columnaPosicion;
        }

        //metodo para contar las minas alrededor de la celda actual y ponerle un número dependiendo del número de celdas.
        //si no hay minas alrededor se redirige al método tableroJuego.DestaparCelda para comprobar si hay minas alrededor de todas las celdas
        public int ContarMinasCeldasProximas()
        {
            int contadorBombas = 0;

            if (!esRevelada && !esMarcada)
            {
                esRevelada = true;

                for (int iterador = 0; iterador < 9; iterador++) // busca bombas en todas las celdas proximas
                {
                    if (iterador == 4) 
                    {
                        continue; // no se comprueba porque es la propia celda
                    }
                    if (tableroJuego.VerificarSiCeldaEsBomba(filaPosicion + iterador / 3 - 1, columnaPosicion + iterador % 3 - 1)) 
                    {
                        contadorBombas++; // si hay una bomba, la cuenta
                    }
                }

                if (contadorBombas == 0)
                {
                    for (int iterador = 0; iterador < 9; iterador++) // busca bombas en todos las celdas proximas
                    {
                        if (iterador == 4) 
                        {
                            continue; // no se comprueba porque es la propia celda
                        }
                        tableroJuego.DestaparCelda(filaPosicion + iterador / 3 - 1, columnaPosicion + iterador % 3 - 1); // revela todas las celdas
                    }
                }
            }

            return contadorBombas;
        }
    }
}
