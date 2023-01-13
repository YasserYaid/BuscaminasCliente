using Cliente.Properties.Langs;
using System.ServiceModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Cliente
{
    public partial class ListarAmigosGUI : Page
    {
        private ServidorBuscaminasServicio.CuentaUsuarioServiceMgtClient cuentaUsuarioServiceMgt;
        private ServidorBuscaminasServicio.AmigosServiceMgtClient amigosServiceMgt;
        private Tuple<string, string>[] amigosJugador;
        private int idJugador = 0;
        private string nombreUsuario;
        public ListarAmigosGUI(string nombreUsuario)
        {
            InitializeComponent();
            this.nombreUsuario = nombreUsuario;
            this.cuentaUsuarioServiceMgt = new ServidorBuscaminasServicio.CuentaUsuarioServiceMgtClient();
            this.amigosServiceMgt = new ServidorBuscaminasServicio.AmigosServiceMgtClient();
            RecargarAmigosListView(nombreUsuario);
        }

        private void AgregarButton_Click(object sender, RoutedEventArgs e)
        {
            EnviarSolicitudAmistadGUI enviarSolicitudAmistadGUI = new EnviarSolicitudAmistadGUI(nombreUsuario);
            enviarSolicitudAmistadGUI.Owner = Application.Current.MainWindow;
            enviarSolicitudAmistadGUI.ShowDialog();
        }

        private void EliminarButton_Click(object sender, RoutedEventArgs e)
        {
            int numeroFilaAmigoListViewSeleccionada = AmigosListView.SelectedIndex;
            if (numeroFilaAmigoListViewSeleccionada != -1)
            {
                MessageBoxResult cuadroDialogoConfirmacionEliminacion = MessageBox.Show(Lang.AvisoConfirmarEliminacion_MSJ, Lang.TituloVentanaConfirmarEliminacion_MSJ, MessageBoxButton.YesNo);
                switch (cuadroDialogoConfirmacionEliminacion)
                {
                    case MessageBoxResult.Yes:
                        string nombreAmigoSeleccionado = amigosJugador[numeroFilaAmigoListViewSeleccionada].Item1;
                        try
                        {
                            bool esExistenteJugadorLogueado = cuentaUsuarioServiceMgt.VerificarExisteciaJugador(nombreUsuario);
                            if (esExistenteJugadorLogueado)
                            {
                                int idJugadorSolicitante = cuentaUsuarioServiceMgt.ObtenerIdJugador(nombreUsuario);
                                int idJugadorReceptor = cuentaUsuarioServiceMgt.ObtenerIdJugador(nombreAmigoSeleccionado);
                                bool esSolicitudEliminacionExitosa = amigosServiceMgt.EliminarAmigo(idJugadorSolicitante, idJugadorReceptor);
                                if (esSolicitudEliminacionExitosa)
                                {
                                    MessageBox.Show(Lang.AvisoAmistadBorrada_MSJ);
                                    RecargarAmigosListView(nombreUsuario);
                                }
                                else
                                {
                                    MessageBox.Show(Lang.AlertaNoSePudoBorrarAmistad_MSJ);
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
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            else
            {
                MessageBox.Show(Lang.AlertaFilaDeAmigoNoSeleccionada_MSJ);
            }
        }

        private void CerrarButton_Click(object sender, RoutedEventArgs e)
        {
            MenuPrincipalGUI menuPrincipalGUI = new MenuPrincipalGUI(nombreUsuario);
            Application.Current.MainWindow.Content = menuPrincipalGUI;
        }

        private void RecargarAmigosListView(string usuario)
        {
            AmigosListView.Items.Clear();
            try
            {
                idJugador = cuentaUsuarioServiceMgt.ObtenerIdJugador(usuario);
                amigosJugador = amigosServiceMgt.ObtenerEstadoAmigos(idJugador);
                foreach (Tuple<string, string> amigo in amigosJugador)
                {
                    if (amigo.Item2 == "Baneado")
                    {
                        var jugador = new { Usuario = amigo.Item1, Estado = Lang.AvisoAmigoBaneado_MSJ };
                        AmigosListView.Items.Add(jugador);
                    }
                    else
                    {
                        var jugador = new { Usuario = amigo.Item1, Estado = Lang.AvisoAmigoNormal_MSJ };
                        AmigosListView.Items.Add(jugador);
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

    }
}
