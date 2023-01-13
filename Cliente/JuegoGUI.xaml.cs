using Cliente.Properties.Langs;
using Cliente.ServidorBuscaminasServicio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cliente
{
    public partial class JuegoGUI : Page, IChatServiceDuplexCallback , IJuegoServiceDuplexCallback
    {
        private ServidorBuscaminasServicio.ChatServiceDuplexClient chatServiceDuplex;
        private ServidorBuscaminasServicio.JuegoServiceDuplexClient juegoServiceDuplex;
        private InstanceContext instanceContext;
        private string nombreUsuario;
        private string codigoAccesoSala;
        private string nombreUsuarioTurnoAnterior;
        private bool esTurnoEfectuado = false;
        private bool esMultiJugador;
        private bool esJuegoTerminado = false;


        private int numeroMinas;
        public TableroBuscaminas tableroBuscaMinas { get; private set; }
        private bool esJuegoIniciado;
        private Color[] colorTextoMina;

        public JuegoGUI(string nombreUsuario, string codigoAccesoSala, bool esMultiJugador)
        {
            InitializeComponent();
            this.nombreUsuario = nombreUsuario;
            this.codigoAccesoSala = codigoAccesoSala;
            this.esMultiJugador = esMultiJugador;
            this.instanceContext = new InstanceContext(this);
            this.chatServiceDuplex = new ServidorBuscaminasServicio.ChatServiceDuplexClient(instanceContext);
            this.chatServiceDuplex.AgregarJugadorChatCallback(codigoAccesoSala, nombreUsuario);
            this.juegoServiceDuplex = new ServidorBuscaminasServicio.JuegoServiceDuplexClient(instanceContext);
            this.juegoServiceDuplex.AgregarJugadorJuegoCallback(codigoAccesoSala, nombreUsuario);
            this.nombreUsuarioTurnoAnterior = nombreUsuario;

            this.esJuegoIniciado = false;
            this.numeroMinas = 15;
            colorTextoMina = new Color[] { Colors.White, Colors.Blue, Colors.DarkGreen, Colors.Red, Colors.DarkBlue, Colors.DarkViolet, Colors.DarkCyan, Colors.Brown, Colors.Black };
            ConfigurarJuego();
        }

        public void RecibirMensaje(string nombreUsuarioEmisor, string mensaje)
        {
            ListBoxItem mensajeListBoxItem = new ListBoxItem();
            mensajeListBoxItem.Content = nombreUsuarioEmisor + ": " + mensaje;
            MensajesChatListBox.Items.Add(mensajeListBoxItem);
        }

        public void RecibirTurno(string nombreUsuarioAnterior)
        {
            this.nombreUsuarioTurnoAnterior = nombreUsuarioAnterior;
        }

        private void SalirButton_Click(object sender, RoutedEventArgs e)
        {
            MenuPrincipalGUI menuPrincipalGUI = new MenuPrincipalGUI(nombreUsuario);
            Application.Current.MainWindow.Content = menuPrincipalGUI;
        }

        private void EnviarMensajeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                chatServiceDuplex.EnviarMensajeChat(codigoAccesoSala, nombreUsuario, MensajeChatTextbox.Text);
            }
            catch (EndpointNotFoundException)
            {
                MessageBox.Show(Lang.ErrorNoSeEncontroServidor_MSJ);
                Environment.Exit(0);
            }
            catch (CommunicationObjectFaultedException)
            {
                MessageBox.Show(Lang.ErrorObjetoComunicacionConServidor_MSJ);
                Environment.Exit(0);
            }
        }

        public void RecibirFinalizacionDelJuego(string cause)
        {
            throw new NotImplementedException();
        }



        /*
        private void MenuItem_Click_New(object sender, RoutedEventArgs e)
        {
            GameSetup();
        }
        */

        private void ConfigurarJuego()
        {
            tableroBuscaMinas = new TableroBuscaminas(10, 10, numeroMinas);
            foreach (Button boton in TableroGrid.Children)
            {
                boton.Content = ""; // borra la bandera o la imagen de la bomba (si existe)
                boton.IsEnabled = true; // se puede hacer clic en el botón
            }
            // Adjunta el evento indicador de minas
            tableroBuscaMinas.EventoCambioContadorBanderas += ActualizarContadorBanderasGUI;
            BanderasTextBox.Text = numeroMinas.ToString();

            // Adjunta el Click del boton , invocado por una celda
            tableroBuscaMinas.EventoClicCelda += AbrirCeldaEnBotonClic;

            // Adjunta el evento de tiempo transcurrido
            tableroBuscaMinas.EventoTemporizador += ActualizarCronometroGUI;
            TiempoTextBox.Text = "0";

            tableroBuscaMinas.EjecutarJuego();
            esJuegoIniciado = true;
        }
        /*
        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // closes application
        }
        */
        private void Boton_Izquierdo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("NombreUsuario Anterior" + nombreUsuarioTurnoAnterior);
            if (nombreUsuarioTurnoAnterior != nombreUsuario)
            {
                MessageBox.Show("INTER AUN NO ES TU TURNO, ESPERA TU TURNO PORFAVOR");
            }
            else
            {
                Button boton = (Button)sender; // obtiene la referencia del botón pulsado
                int fila = AnalizarFilaBoton(boton);
                int columna = AnalizarColumnaBoton(boton);

                if (!tableroBuscaMinas.ValidarPosicionCeldaDentroCuadricula(fila, columna))
                {
                    throw new BuscaminasExcepcion(Lang.ErrorBMExcepcionReferenciaInvalidaBoton_MSJ); // el botón señala una celda no válida
                }

                if (tableroBuscaMinas.VerificarSiCeldaEstaMarcada(fila, columna))
                {
                    return; // la celda marcada no puede revelarse
                }

                boton.IsEnabled = false; // desactiva el boton
                if (tableroBuscaMinas.VerificarSiCeldaEsBomba(fila, columna)) //se revelo una bomba
                {
                    // adjunta la imagen de la bomba al botón
                    StackPanel stackPanel = new StackPanel();
                    stackPanel.Orientation = Orientation.Horizontal;
                    Image imagenBomba = new Image();
                    BitmapImage mapaBitsImagenBomba = new BitmapImage();
                    mapaBitsImagenBomba.BeginInit();
                    mapaBitsImagenBomba.UriSource = new Uri(@"/Imagenes/JuegoGUI/Bomba.png", UriKind.Relative);
                    mapaBitsImagenBomba.EndInit();
                    imagenBomba.Source = mapaBitsImagenBomba;
                    stackPanel.Children.Add(imagenBomba);
                    boton.Content = stackPanel;
                    tableroBuscaMinas.DetenerConometro();

                    //termina el juego y abre todas las celdas
                    if (esJuegoIniciado)
                    {
                        esJuegoIniciado = false;
                        foreach (Button button in TableroGrid.Children)
                        {
                            if (button.IsEnabled) this.Boton_Izquierdo_Click(button, e); // llama al resto de botones no revelados
                        }
                    }
                }
                else // se reveló un espacio vacío
                {
                    int numeroMinasProximas = tableroBuscaMinas.BuscarMinasCercanas(fila, columna); // abre la celda y comprueba si hay bombas alrededor
                    if (numeroMinasProximas > 0) // pone la etiqueta correspondiente en el botón actual
                    {
                        boton.Foreground = new SolidColorBrush(colorTextoMina[numeroMinasProximas]);
                        boton.FontWeight = FontWeights.Bold;
                        boton.Content = numeroMinasProximas.ToString();
                    }
                }
                esTurnoEfectuado = true;
            }
            if (esTurnoEfectuado)
            {
                juegoServiceDuplex.PasarTurno(codigoAccesoSala, nombreUsuario);
            }
        }

        private void Boton_Derecho_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("NombreUsuario Anterior" + nombreUsuarioTurnoAnterior);
            if (nombreUsuarioTurnoAnterior != nombreUsuario)
            {
                MessageBox.Show("INTER AUN NO ES TU TURNO, ESPERA TU TURNO PORFAVOR");
            }
            else
            {
                Button boton = (Button)sender; // obtiene la referencia del botón pulsado
                int fila = AnalizarFilaBoton(boton);
                int columna = AnalizarColumnaBoton(boton);
                if (!tableroBuscaMinas.ValidarPosicionCeldaDentroCuadricula(fila, columna))
                {
                    throw new BuscaminasExcepcion(Lang.ErrorBMExcepcionReferenciaInvalidaBoton_MSJ); // el botón señala una celda no válida
                }

                if (tableroBuscaMinas.VerificarSiCeldaEstaMarcada(fila, columna)) // si el botón tiene imagen de bandera
                {
                    boton.Content = ""; // limpia imagen
                }
                else
                {
                    //adjunta la imagen de la bandera al botón
                    StackPanel stackPanel = new StackPanel();
                    stackPanel.Orientation = Orientation.Horizontal;
                    Image imagenBandera = new Image();
                    BitmapImage mapaBitsImagenBandera = new BitmapImage();
                    mapaBitsImagenBandera.BeginInit();
                    mapaBitsImagenBandera.UriSource = new Uri(@"/Imagenes/JuegoGUI/Bandera.png", UriKind.Relative);
                    mapaBitsImagenBandera.EndInit();
                    imagenBandera.Source = mapaBitsImagenBandera;
                    stackPanel.Children.Add(imagenBandera);
                    boton.Content = stackPanel;
                }
                tableroBuscaMinas.MarcarODesmarcarCelda(fila, columna);
                esTurnoEfectuado = true;
            }
            if (esTurnoEfectuado)
            {
                juegoServiceDuplex.PasarTurno(codigoAccesoSala, nombreUsuario);
            }
        }

        private int AnalizarFilaBoton(Button button)
        {
            // El formato del nombre del botón debe ser "ButtonXY" o "ButtonXXYY", donde X e Y son los índices numéricos de la celda de la mina.
            if (button.Name.IndexOf("CasillaButton") != 0)
            {
                throw new BuscaminasExcepcion(Lang.ErrorBMExcepcionNombreIncorrectoBoton_MSJ); // el botón está mal
            }
            return int.Parse(button.Name.Substring(13, (button.Name.Length - 13) / 2));
        }

        private int AnalizarColumnaBoton(Button button)
        {
            // Button Name format must be "ButtonXY" or "ButtonXXYY", where X and Y are numberical indices of the mine cell
            if (button.Name.IndexOf("CasillaButton") != 0)
            {
                throw new BuscaminasExcepcion(Lang.ErrorBMExcepcionNombreIncorrectoBoton_MSJ); // el botón está mal
            }
            return int.Parse(button.Name.Substring(13 + (button.Name.Length - 13) / 2, (button.Name.Length - 13) / 2));
        }

        private void ActualizarContadorBanderasGUI(object sender, EventArgs e)
        {
            BanderasTextBox.Text = (this.numeroMinas - tableroBuscaMinas.numeroMinasMarcadas).ToString();
        }

        private void ActualizarCronometroGUI(object sender, EventArgs e)
        {
            TiempoTextBox.Text = tableroBuscaMinas.tiempoTranscurrido.ToString();
        }

        private void AbrirCeldaEnBotonClic(object sender, ArgumentosDeEventosCelda e)
        {
            // Abre la celda solicitada simulando el clic de un botón
            string nombreBoton = "CasillaButton";
            if (tableroBuscaMinas.ancho <= 10 && tableroBuscaMinas.alto <= 10)
            {
                nombreBoton += String.Format("{0:D1}{1:D1}", e.CeldaFila, e.CeldaColumna); // coordenadas de un dígito
            }
            else
            {
                nombreBoton += String.Format("{0:D2}{1:D2}", e.CeldaFila, e.CeldaColumna); // coordenadas de dos dígitos
            }

            Button botonEmisor = (TableroGrid.FindName(nombreBoton) as Button);
            if (botonEmisor == null)
            {
                throw new BuscaminasExcepcion(Lang.ErrorBMExcepcionReferenciaInvalidaBoton_MSJ); // la celda se refiere a un botón no válido
            }
            // llama al manejador del evento "Click del botón" correspondiente 
            this.Boton_Izquierdo_Click(botonEmisor, new RoutedEventArgs());
        }

        public void RecibirPorcentages(int percentage)
        {
            throw new NotImplementedException();
        }
    }
}
