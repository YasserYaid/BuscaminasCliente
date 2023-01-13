using System.Windows;
using System.Windows.Controls;

namespace Cliente
{
    public partial class SeleccionarModoJuegoGUI : Page
    {
        string nombreUsuario;
        public SeleccionarModoJuegoGUI(string nombreUsuario)
        {
            InitializeComponent();
            this.nombreUsuario = nombreUsuario;
        }

        private void UnJugadorButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            JuegoGUI juegoGUI = new JuegoGUI(nombreUsuario, null, false);
            Application.Current.MainWindow.Content = juegoGUI;
        }

        private void DosJugadoresButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CrearSalaGUI crearSalaGUI = new CrearSalaGUI(nombreUsuario, null, true);
            Application.Current.MainWindow.Content = crearSalaGUI;
        }

        private void AtrasButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MenuPrincipalGUI menuPrincipalGUI = new MenuPrincipalGUI(nombreUsuario);
            Application.Current.MainWindow.Content = menuPrincipalGUI;
        }
    }
}
