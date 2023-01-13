using Cliente.Properties.Langs;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Cliente
{
    public partial class SeleccionarIdiomaGUI : Window
    {
        string nombreUsuario;
        private int indexIdiomaSeleccionadoComboBox;
        public SeleccionarIdiomaGUI(string nombreUsuario)
        {
            InitializeComponent();
            this.nombreUsuario = nombreUsuario;
        }

        private void IdiomaCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.indexIdiomaSeleccionadoComboBox = IdiomaCombobox.SelectedIndex;
        }

        private void AceptarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            if (indexIdiomaSeleccionadoComboBox == 0)
            {
                Properties.Settings.Default.languajeCode = "es-MX";
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("es-MX");
                InitializeComponent();
                RecargarVentana();
                MessageBox.Show(Lang.AvisoIdiomaCambiado_MSJ);
            }
            else if (indexIdiomaSeleccionadoComboBox == 1)
            {
                Properties.Settings.Default.languajeCode = "en-US";
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                InitializeComponent();
                RecargarVentana();
                MessageBox.Show(Lang.AvisoIdiomaCambiado_MSJ);
            }
            Properties.Settings.Default.Save();
        }

        private void CancelarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RecargarVentana()
        {
            Title = Lang.SeleccionarIdioma_EG;
            SeleccionarIdiomaLabel.Content = Lang.SeleccionarIdioma_EG;
            CancelarButton.Content = Lang.Cancelar_EG;
            AceptarButton.Content = Lang.Aceptar_EG;
        }
    }
}
