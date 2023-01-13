using Cliente.Properties.Langs;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Cliente
{
    public partial class MenuPrincipalGUI : Page
    {
        string nombreUsuario;
        public MenuPrincipalGUI(string nombreUsuario)
        {
            InitializeComponent();
            this.nombreUsuario = nombreUsuario;
        }

        private void JugarButton_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarModoJuegoGUI seleccionarModoJuegoGUI = new SeleccionarModoJuegoGUI(nombreUsuario);
            Application.Current.MainWindow.Content = seleccionarModoJuegoGUI;
        }

        private void UnirseAlJuegoButton_Click(object sender, RoutedEventArgs e)
        {
            IngresarCodigoSalaGUI ingresarCodigoSalaGUI = new IngresarCodigoSalaGUI(nombreUsuario);
            ingresarCodigoSalaGUI.Owner = Application.Current.MainWindow;
            ingresarCodigoSalaGUI.ShowDialog();
        }
        private void OpcionesButton_Click(object sender, RoutedEventArgs e)
        {
            OpcionesGUI opcionesGUI = new OpcionesGUI(nombreUsuario);
            Application.Current.MainWindow.Content = opcionesGUI;
        }

        private void SolicitudesAmistadButton_Click(object sender, RoutedEventArgs e)
        {
            ListarSolicitudesAmistadGUI listarSolicitudesAmistadGUI = new ListarSolicitudesAmistadGUI(nombreUsuario);
            Application.Current.MainWindow.Content = listarSolicitudesAmistadGUI;
        }

        private void ListaAmigosButton_Click(object sender, RoutedEventArgs e)
        {
            ListarAmigosGUI listarAmigosGUI = new ListarAmigosGUI(nombreUsuario);
            Application.Current.MainWindow.Content = listarAmigosGUI;
        }

        private void SalirButton_Click(object sender, RoutedEventArgs e)
        {
                MessageBox.Show(Lang.AvisoSalida_MSJ);
                Environment.Exit(0);
        }

    }

}