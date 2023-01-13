using Cliente.Properties.Langs;
using System.ServiceModel;
using System;
using System.Windows;
using Cliente.ServidorBuscaminasServicio;

namespace Cliente
{
    public partial class IngresarCodigoSalaGUI : Window
    {
        private ServidorBuscaminasServicio.SalaServiceClient salaServiceClient;
        private ServidorBuscaminasServicio.Sala sala;
        string nombreUsuario;
        public IngresarCodigoSalaGUI(string nombreUsuario)
        {
            InitializeComponent();
            this.nombreUsuario = nombreUsuario;
            this.salaServiceClient = new ServidorBuscaminasServicio.SalaServiceClient();
        }

        private void CancelarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AceptarButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(CodigoSalaTextBox.Text))
            {
                try
                {
                    this.sala = salaServiceClient.BuscarSala(CodigoSalaTextBox.Text);
                    if (!(sala == null))
                    {
                        CrearSalaGUI crearSalaGUI = new CrearSalaGUI(nombreUsuario, sala.codigoSala, false);
                        Application.Current.MainWindow.Content = crearSalaGUI;
                        this.Close();
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
                MessageBox.Show(Lang.AlertaCodigoSalaNoIntroducido_MSJ);
            }
        }
    }
}
