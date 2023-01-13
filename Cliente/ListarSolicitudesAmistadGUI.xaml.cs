using Cliente.Properties.Langs;
using System.ServiceModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Cliente
{
    public partial class ListarSolicitudesAmistadGUI : Page
    {
        string nombreUsuario;
        private string[] listadoSolicitudesJugador;
        private ServidorBuscaminasServicio.CuentaUsuarioServiceMgtClient cuentaUsuarioServiceMgt;
        private ServidorBuscaminasServicio.AmigosServiceMgtClient amigosServiceMgt;
        public ListarSolicitudesAmistadGUI(string nombreUsuario)
        {
            InitializeComponent();
            this.nombreUsuario = nombreUsuario;
            this.cuentaUsuarioServiceMgt = new ServidorBuscaminasServicio.CuentaUsuarioServiceMgtClient();
            this.amigosServiceMgt = new ServidorBuscaminasServicio.AmigosServiceMgtClient();
            RecargarSolicitudesAmistadDelJugador();
        }

        private void AceptarButton_Click(object sender, RoutedEventArgs e)
        {
            if (SolicitudesListBox.SelectedIndex != -1)
            {
                string nombreUsuarioSolicitante = SolicitudesListBox.SelectedItem.ToString();
                try
                {
                    int idJugadorSolicitante = cuentaUsuarioServiceMgt.ObtenerIdJugador(nombreUsuarioSolicitante);
                    int idJugadorReceptor = cuentaUsuarioServiceMgt.ObtenerIdJugador(nombreUsuario);
                    bool esSolicitudAceptadaExitosamente = amigosServiceMgt.AceptarSolicitud(idJugadorSolicitante, idJugadorReceptor);
                    if (esSolicitudAceptadaExitosamente)
                    {
                        RecargarSolicitudesAmistadDelJugador();
                        MessageBox.Show(nombreUsuarioSolicitante + Lang.AvisoAmistadAgregada_MSJ);
                    }
                    else
                    {
                        MessageBox.Show(Lang.AlertaNoSePudoAgregarAmistad_MSJ);
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
                MessageBox.Show(Lang.AlertaFilaDeSolicitudNoSeleccionada_MSJ);
            }
        }

        private void EliminarButton_Click(object sender, RoutedEventArgs e)
        {
            if (SolicitudesListBox.SelectedIndex != -1)
            {
                MessageBoxResult resucuadroDialogoConfirmacionEliminacionltado = MessageBox.Show(Lang.AvisoConfirmarEliminacionSolicitud_MSJ, Lang.TituloVentanaConfirmarEliminacionSolicitud_MSJ, MessageBoxButton.YesNo);
                switch (resucuadroDialogoConfirmacionEliminacionltado)
                {
                    case MessageBoxResult.Yes:
                        string nombreUsuarioSolicitante = SolicitudesListBox.SelectedItem.ToString();
                        try
                        {
                            bool esExistenteJugadorLogueado = cuentaUsuarioServiceMgt.VerificarExisteciaJugador(nombreUsuario);
                            if (esExistenteJugadorLogueado)
                            {
                                int idJugadorSolicitante = cuentaUsuarioServiceMgt.ObtenerIdJugador(nombreUsuario);
                                int idJugadorReceptor = cuentaUsuarioServiceMgt.ObtenerIdJugador(nombreUsuarioSolicitante);
                                bool eliminarSolicitud = amigosServiceMgt.EliminarAmigo(idJugadorSolicitante, idJugadorReceptor);
                                if (eliminarSolicitud)
                                {
                                    RecargarSolicitudesAmistadDelJugador();
                                    MessageBox.Show(Lang.AvisoSolicitudAmistadBorrada_MSJ);
                                }
                                else
                                {
                                    MessageBox.Show(Lang.AlertaNoSePudoBorrarSolicitud_MSJ);
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
                MessageBox.Show(Lang.AlertaFilaDeSolicitudNoSeleccionada_MSJ);
            }
        }

        private void CerrarButton_Click(object sender, RoutedEventArgs e)
        {
            MenuPrincipalGUI menuPrincipalGUI = new MenuPrincipalGUI(nombreUsuario);
            Application.Current.MainWindow.Content = menuPrincipalGUI;
        }

        private void RecargarSolicitudesAmistadDelJugador()
        {
            SolicitudesListBox.Items.Clear();
            try
            {
                int idJugador = cuentaUsuarioServiceMgt.ObtenerIdJugador(nombreUsuario);
                listadoSolicitudesJugador = amigosServiceMgt.ObtenerSolicitudesUsuario(idJugador);
                foreach (string solicitudJugador in listadoSolicitudesJugador)
                {
                    SolicitudesListBox.Items.Add(solicitudJugador);
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
