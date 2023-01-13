using System;
using System.Windows.Threading;

namespace Cliente
{
    public class TableroBuscaminas
    {
        //eventos asociados a un EventHandler delegados
        public event EventHandler EventoCambioContadorBanderas;
        public event EventHandler EventoTemporizador;
        public event EventHandler<ArgumentosDeEventosCelda> EventoClicCelda;

        //campos/propiedades
        public int ancho { get; private set; }
        public int alto { get; private set; }
        public int numeroMinas { get; private set; }
        public int tiempoTranscurrido { get; private set; }
        private Celda[,] celdas;
        private int marcasCorrectas;
        private int marcasIncorrectas;
        public int numeroMinasMarcadas { get { return (this.marcasCorrectas + this.marcasIncorrectas); } }
        private DispatcherTimer cronometroJuego;

        public TableroBuscaminas(int ancho, int alto, int numeroMinas)
        {
            this.ancho = ancho;
            this.alto = alto;
            this.numeroMinas = numeroMinas;
        }

        //metodo para comprobar si la posición actual está dentro de la cuadrícula
        public bool ValidarPosicionCeldaDentroCuadricula(int filaPosicion, int columnaPosicion)
        {
            return ((filaPosicion >= 0) && (filaPosicion < this.ancho) && (columnaPosicion >= 0) && (columnaPosicion < this.alto));
        }

        //metodo para comprobar si la posición actual está minada
        public bool VerificarSiCeldaEsBomba(int filaPosicion, int columnaPosicion)
        {
            if (this.ValidarPosicionCeldaDentroCuadricula(filaPosicion, columnaPosicion))
            {
                return this.celdas[filaPosicion, columnaPosicion].esMinada;
            }
            return false;
        }

        //metodo para comprobar si la posición actual está marcada
        public bool VerificarSiCeldaEstaMarcada(int filaPosicion, int columnaPosicion)
        {
            if (this.ValidarPosicionCeldaDentroCuadricula(filaPosicion, columnaPosicion))
            {
                return this.celdas[filaPosicion, columnaPosicion].esMarcada;
            }
            return false;
        }
        //metodo para determinar el estado de la celda actual
        //redirige a Celda.ContarMinasCeldasProximas() para determinar si la celda está minada, o cuántas minas hay a su alrededor
        public int BuscarMinasCercanas(int filaPosicion, int columnaPosicion)
        {
            if (this.ValidarPosicionCeldaDentroCuadricula(filaPosicion, columnaPosicion))
            {
                int numeroMinasEncontradas = this.celdas[filaPosicion, columnaPosicion].ContarMinasCeldasProximas(); // comprueba el número de minas circundantes
                ComprobarFinalizacionTablero(); // comprobaciones de fin de partida
                return numeroMinasEncontradas;
            }
            throw new BuscaminasExcepcion("Llamada de referencia TableroBucaminas no válida [fila, columna] al revelar");
        }

        //método para poner o quitar la bandera si alguna celda está seleccionada
        public void MarcarODesmarcarCelda(int filaPosicion, int columnaPosicion)
        {
            if (!this.ValidarPosicionCeldaDentroCuadricula(filaPosicion, columnaPosicion))
            {
                throw new BuscaminasExcepcion("Llamada de referencia TableroBucaminas no válida [fila, columna] al revelar");
            }

            Celda celdaActual = this.celdas[filaPosicion, columnaPosicion];
            if (!celdaActual.esMarcada)
            {
                if (celdaActual.esMinada)
                {
                    this.marcasCorrectas++;
                }
                else
                {
                    this.marcasIncorrectas++;
                }
            }
            else
            {
                if (celdaActual.esMinada)
                {
                    this.marcasCorrectas--;
                }
                else
                {
                    this.marcasIncorrectas--;
                }
            }
            celdaActual.esMarcada = !celdaActual.esMarcada; // actualiza el valor de la marca
            ComprobarFinalizacionTablero(); // comprobaciones de final de partida
            // Activa el evento CambioContadorBanderas 
            this.EnCambioContadorBanderas(new EventArgs());
        }

        //método para abrir una sola celda
        public void DestaparCelda(int filaPosicion, int columnaPosicion)
        {
            //Comprueba si la celda no está revelada ya
            if (this.ValidarPosicionCeldaDentroCuadricula(filaPosicion, columnaPosicion) && !this.celdas[filaPosicion, columnaPosicion].esRevelada)
            {
                // a continuación, Lanza el evento destaparCeldaEnClic con los datos de posición de la celda  
                this.DestaparCeldaEnClic(new ArgumentosDeEventosCelda(filaPosicion, columnaPosicion));
            }
        }

        //Metodo para comprobar si el tablero está totalmente resuelto
        private void ComprobarFinalizacionTablero()
        {
            bool esJuegoFinalizado = false; // asume que el juego no está terminado
            if (this.marcasIncorrectas == 0 && this.numeroMinasMarcadas == this.numeroMinas) // tenemos cero banderas más para poner
            {
                esJuegoFinalizado = true; // asume que todas las celdas se revelaron
                foreach (Celda elementoCelda in this.celdas)
                {
                    if (!elementoCelda.esRevelada && !elementoCelda.esMinada)
                    {
                        esJuegoFinalizado = false; // si una celda no se ha revelado es que el juego no ha terminado
                        break;
                    }
                }
            }
            if (esJuegoFinalizado) 
            {
                cronometroJuego.Stop(); // cuando el juego haya terminado, el temporizador debe detenerse inmediatamente
            }
        }

        //método para crear un juego
        public void EjecutarJuego()
        {
            this.marcasCorrectas = 0;
            this.marcasIncorrectas = 0;
            this.tiempoTranscurrido = 0;

            this.celdas = new Celda[ancho, alto];

            for (int fila = 0; fila < ancho; fila++)
            {
                for (int columna = 0; columna < alto; columna++)
                {
                    Celda celda = new Celda(this, fila, columna);
                    this.celdas[fila, columna] = celda;
                }
            }

            int contadorMinas = 0;
            Random posicionMinas = new Random();

            while (contadorMinas < numeroMinas)
            {
                int filas = posicionMinas.Next(ancho);
                int columnas = posicionMinas.Next(alto);

                Celda celda = this.celdas[filas, columnas];

                if (!celda.esMinada)
                {
                    celda.esMinada = true;
                    contadorMinas++;
                }
            }

            cronometroJuego = new DispatcherTimer();
            cronometroJuego.Tick += new EventHandler(ActualizarCronometroEnTiempoTranscurrido);
            cronometroJuego.Interval = new TimeSpan(0, 0, 1);
            cronometroJuego.Start();
        }

        // método para detener el cronometro
        public void DetenerConometro()
        {
            cronometroJuego.Stop();
        }

        // "Flag Counter Changed" Event Raiser
        protected virtual void EnCambioContadorBanderas(EventArgs e)
        {
            EventHandler handler = EventoCambioContadorBanderas;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        // "Time Counter Changed" Event Raiser
        protected virtual void ActualizarCronometroEnTiempoTranscurrido(object sender, EventArgs e)
        {
            this.tiempoTranscurrido++;
            EventHandler handler = EventoTemporizador;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        // "Click to Reveal Plate" Event Raiser - se utiliza para abrir automáticamente todos los platos vacíos de una región
        protected virtual void DestaparCeldaEnClic(ArgumentosDeEventosCelda e)
        {
            EventHandler<ArgumentosDeEventosCelda> handler = EventoClicCelda;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
