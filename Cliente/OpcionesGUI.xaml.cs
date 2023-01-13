using System.Windows;
using System.Windows.Controls;

namespace Cliente
{
    public partial class OpcionesGUI : Page
    {
        string nombreUsuario;
        public OpcionesGUI(string nombreUsuario)
        {
            InitializeComponent();
            this.nombreUsuario = nombreUsuario;
        }

        private void IdiomaButton_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarIdiomaGUI seleccionarIdiomaGUI = new SeleccionarIdiomaGUI(nombreUsuario);
            seleccionarIdiomaGUI.Owner = Application.Current.MainWindow;
            seleccionarIdiomaGUI.ShowDialog();
        }

        private void CreditosButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void AtrasButton_Click(object sender, RoutedEventArgs e)
        {
            MenuPrincipalGUI menuPrincipalGUI = new MenuPrincipalGUI(nombreUsuario);
            Application.Current.MainWindow.Content = menuPrincipalGUI;
        }
    }
}
