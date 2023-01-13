using Cliente.Properties.Langs;
using Cliente.ServidorBuscaminasServicio;
using System;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;

namespace Cliente
{
    public partial class CrearSalaGUI : Page, ISalaServiceDuplexCallback
    {
        private ServidorBuscaminasServicio.CuentaUsuarioServiceMgtClient cuentaUsuarioServiceMgt;
        private ServidorBuscaminasServicio.SalaServiceDuplexClient salaServiceDuplex;
        private ServidorBuscaminasServicio.AmigosServiceMgtClient amigosServiceMgt;
        private ServidorBuscaminasServicio.SalaServiceClient salaServiceClient;
        private ServidorBuscaminasServicio.Sala sala;
        private InstanceContext instanceContext;
        private string[] catalogoAmigos;
        private string codigoAccesoSala;
        private string nombreUsuario;
        private bool esPrimerJugador;
        private int idJugador;

        public CrearSalaGUI(string nombreUsuario, string codigoAccesoSala, bool esPrimerJugador)
        {
            InitializeComponent();
            this.nombreUsuario = nombreUsuario;
            this.codigoAccesoSala = codigoAccesoSala;
            this.esPrimerJugador = esPrimerJugador;
            this.idJugador = 0;
            this.instanceContext = new InstanceContext(this);
            this.cuentaUsuarioServiceMgt = new ServidorBuscaminasServicio.CuentaUsuarioServiceMgtClient();
            this.amigosServiceMgt = new ServidorBuscaminasServicio.AmigosServiceMgtClient();
            this.salaServiceClient = new ServidorBuscaminasServicio.SalaServiceClient();
            this.salaServiceDuplex = new ServidorBuscaminasServicio.SalaServiceDuplexClient(instanceContext);
            
            if(codigoAccesoSala == null || esPrimerJugador == true)
            {
                InvitarButton.IsEnabled = true;
                CrearSalaNueva();
                ObtenerAmigos(nombreUsuario);
            }

            AgregarJugador();
        }

        private void InvitarButton_Click(object sender, RoutedEventArgs e)
        {
            if (AmigosListBox.SelectedIndex != -1)
            {
                try
                {
                    string nombreUsuarioAmigoSeleccionado = AmigosListBox.SelectedItem.ToString();
                    string correoAmigoSeleccionado = cuentaUsuarioServiceMgt.ObtenerCorreoJugador(nombreUsuarioAmigoSeleccionado);
                    try
                    {
                        string mensajeInvitacion = nombreUsuario + Lang.CorreoMensajeCodigoInvitacion_MSJ;
                        bool esEnvioCorreoExitoso = cuentaUsuarioServiceMgt.EnviarCorreo(correoAmigoSeleccionado, Lang.CorreoAsuntoCodigoInvitacion_MSJ, mensajeInvitacion, codigoAccesoSala);
                        if (esEnvioCorreoExitoso)
                        {
                            AmigosListBox.IsEnabled = false;
                            InvitarButton.IsEnabled = false;
                        }
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show(Lang.ErrorFormatoCorreoInvalido_MSJ);
                    }
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
            else
            {
                MessageBox.Show(Lang.AlertaFilaDeAmigoNoSeleccionada_MSJ);
            }
        }

        private void IniciarJuegoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                salaServiceDuplex.ColocarJugadoresEnElJuego(codigoAccesoSala);
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

        private void CancelarJuegoButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult cuadroDialogoConfirmacionCancelarJuego = MessageBox.Show(Lang.AvisoConfirmarCancelacionJuego_MSJ, Lang.TituloVentanaCancelacionJuego_MSJ, MessageBoxButton.YesNo);
            switch (cuadroDialogoConfirmacionCancelarJuego)
            {
                case MessageBoxResult.Yes:
                    SeleccionarModoJuegoGUI seleccionarModoJuegoGUI = new SeleccionarModoJuegoGUI(nombreUsuario);
                    Application.Current.MainWindow.Content = seleccionarModoJuegoGUI;
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void CrearSalaNueva()
        {
            try
            {
                this.sala = salaServiceClient.CrearNuevaSala();
                codigoAccesoSala = sala.codigoSala;
                CodigoSalaLabel.Content = codigoAccesoSala;
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

        private void ObtenerAmigos(string usuario)
        {
            AmigosListBox.Items.Clear();
            try
            {
                idJugador = cuentaUsuarioServiceMgt.ObtenerIdJugador(usuario);
                catalogoAmigos = amigosServiceMgt.ObtenerAmigosUsuario(idJugador);
                if (catalogoAmigos.Any())
                {
                    foreach (var amigo in catalogoAmigos)
                    {
                        AmigosListBox.Items.Add(amigo);
                    }
                }
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

        private void AgregarJugador()
        {
            try
            {
                if (esPrimerJugador && codigoAccesoSala != null)
                {
                    salaServiceDuplex.AgregarJugadorComoPrimerJugador(codigoAccesoSala, nombreUsuario);
                    IniciarJuegoButton.Content = Lang.EsperandoJugador_EG;
                    Jugador1Label.Content = nombreUsuario;
                }
                else if(!esPrimerJugador && codigoAccesoSala != null)
                {
                    salaServiceDuplex.AgregarJugadorComoSegundoJugador(codigoAccesoSala, nombreUsuario);
                    IniciarJuegoButton.Content = Lang.Jugador1DebeComenzar_MSJ;
                }
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

        public void ActualizarNombresUsuario(string nombreUsuarioJugador1, string nombreUsuarioJugador2)
        {
            Jugador1Label.Content = nombreUsuarioJugador1;
            Jugador2Label.Content = nombreUsuarioJugador2;
            if (esPrimerJugador)
            {
                IniciarJuegoButton.IsEnabled = true;
                IniciarJuegoButton.Content = Lang.IniciarJuego_EG;
            }
        }

        public void JugarMultigugador(string codigoSala)
        {
            JuegoGUI juegoGUI = new JuegoGUI(nombreUsuario, codigoSala, true);
            Application.Current.MainWindow.Content = juegoGUI;
        }
    }
}
