using System.Windows;

namespace Cliente
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void IniciarSesionButton_Click(object sender, RoutedEventArgs e)
        {
            IniciarSesionGUI iniciarSesionGUI = new IniciarSesionGUI();
            Application.Current.MainWindow.Content = iniciarSesionGUI;
        }

        private void RegistrarseButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrarCuentaGUI registrarCuentaGUI = new RegistrarCuentaGUI();
            Application.Current.MainWindow.Content = registrarCuentaGUI;
        }
    }
}
