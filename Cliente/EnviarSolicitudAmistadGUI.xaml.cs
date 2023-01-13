using Cliente.Properties.Langs;
using System;
using System.ServiceModel;
using System.Windows;

namespace Cliente
{
    public partial class EnviarSolicitudAmistadGUI : Window
    {
        private ServidorBuscaminasServicio.CuentaUsuarioServiceMgtClient cuentaUsuarioServiceMgt;
        private ServidorBuscaminasServicio.AmigosServiceMgtClient amigosServiceMgt;
        private string nombreUsuario;

        public EnviarSolicitudAmistadGUI(string nombreUsuario)
        {
            InitializeComponent();
            this.nombreUsuario = nombreUsuario;
            this.cuentaUsuarioServiceMgt = new ServidorBuscaminasServicio.CuentaUsuarioServiceMgtClient();
            this.amigosServiceMgt = new ServidorBuscaminasServicio.AmigosServiceMgtClient();
        }

        private void CancelarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AceptarButton_Click(object sender, RoutedEventArgs e)
        {
            if (NombreUsuarioTextBox.Text != null)
            {
                if (NombreUsuarioTextBox.Text != nombreUsuario)
                {
                    try
                    {
                        string nombreUsuarioABuscar = NombreUsuarioTextBox.Text;
                        bool esExistenteUsuarioABuscar = cuentaUsuarioServiceMgt.VerificarExisteciaJugador(nombreUsuarioABuscar);
                        if (esExistenteUsuarioABuscar)
                        {
                            int idJugadorSolicitante = cuentaUsuarioServiceMgt.ObtenerIdJugador(nombreUsuario);
                            int idJugadorReceptor = cuentaUsuarioServiceMgt.ObtenerIdJugador(nombreUsuarioABuscar);
                            bool esSolicitudAmistadNoExistente = amigosServiceMgt.ExisteSolicitudAmistad(idJugadorSolicitante, idJugadorReceptor);
                            if (esSolicitudAmistadNoExistente)
                            {
                                bool esSolicitudEnviadaExitosamente = amigosServiceMgt.EnviarSolicitud(idJugadorSolicitante, idJugadorReceptor);
                                if (esSolicitudEnviadaExitosamente)
                                {
                                    MessageBox.Show(Lang.AvisoSolicitudEnviada_MSJ);
                                    NombreUsuarioTextBox.Clear();
                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show(Lang.AlertaNoSePudoEnviarSolicitud_MSJ);
                                }
                            }
                            else
                            {
                                MessageBox.Show(Lang.AlertaYaExisteUnaSolicitudEnviada_MSJ);
                            }
                        }
                        else
                        {
                            MessageBox.Show(Lang.AlertaNoExisteNombreUsuario_MSJ);
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
                    MessageBox.Show(Lang.AlertaMismoNombreUsuarioDestino_MSJ);
                }
            }
            else
            {
                MessageBox.Show(Lang.AlertaNoSeIngresoNombreUsuario_MSJ);
            }
        }
    }
}
