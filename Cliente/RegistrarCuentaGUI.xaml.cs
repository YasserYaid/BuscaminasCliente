using Cliente.Properties.Langs;
using System.ServiceModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace Cliente
{
    public partial class RegistrarCuentaGUI : Page
    {
        private ServidorBuscaminasServicio.CuentaUsuarioServiceMgtClient cuentaUsuarioServiceMgt;
        private string codigoConfirmacionAEnviarAlCorreo;
        private int indexGeneroComboBox;
        public RegistrarCuentaGUI()
        {
            InitializeComponent();
            this.cuentaUsuarioServiceMgt = new ServidorBuscaminasServicio.CuentaUsuarioServiceMgtClient();
        }

        private void EnviarCodigoButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(CorreoTextBox.Text))
            {
                string correoDestino = CorreoTextBox.Text;
                try
                {
                    codigoConfirmacionAEnviarAlCorreo = cuentaUsuarioServiceMgt.GenerarCodigo();
                    bool esCorrectoEnvioCorreo = cuentaUsuarioServiceMgt.EnviarCorreo(correoDestino, Lang.CorreoAsuntoCodigoRegistroCuenta_MSJ, Lang.CorreoMensajeCodigoRegistroCuenta_MSJ, codigoConfirmacionAEnviarAlCorreo);
                    if (esCorrectoEnvioCorreo)
                    {
                        CorreoTextBox.IsEnabled = false;
                        CodigoConfirmacionTextBox.IsEnabled = true;
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show(Lang.ErrorFormatoCorreoInvalido_MSJ);
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
                MessageBox.Show(Lang.AlertaCorreoEsRequerido_MSJ);
            }

        }

        private void AceptarButton_Click(object sender, RoutedEventArgs e)
        {
            bool esCorrectoLLenadoFormulario = RevisarLlenadoFormulario();
            if (esCorrectoLLenadoFormulario)
            {
                bool esCorrectoFormatoContrasenia = RevisarformatoContrasenia();
                if (esCorrectoFormatoContrasenia)
                {
                    string nombreUsuario = NombreUsuarioTextBox.Text;
                    string contrasenia = ContraseniaPasswordBox.ToString();
                    string correo = CorreoTextBox.Text;
                    bool esGeneroHombre = true;
                    if(indexGeneroComboBox != 0)
                    {
                        esGeneroHombre = false;
                    }
                    try
                    {
                        bool esRegistroExitoso = false;
                        bool esCuentaDisponible = cuentaUsuarioServiceMgt.VerificarDisponibilidadCuenta(nombreUsuario, correo);

                        if (esCuentaDisponible)
                        {
                            esRegistroExitoso = cuentaUsuarioServiceMgt.VerificarRegistroCuenta(nombreUsuario, contrasenia, correo, esGeneroHombre);
                        }
                        else
                        {
                            MessageBox.Show(Lang.AlertaLaCuentaYaExiste_MSJ);
                            CodigoConfirmacionTextBox.IsEnabled = false;
                            CodigoConfirmacionTextBox.Clear();
                            CorreoTextBox.IsEnabled = true;
                            codigoConfirmacionAEnviarAlCorreo = "";
                        }

                        if (esRegistroExitoso)
                        {
                            MessageBox.Show(Lang.AvisoCuentaRegistradaExitosamente_MSJ);
                            NombreUsuarioTextBox.Clear();
                            CorreoTextBox.Clear();
                            ContraseniaPasswordBox.Clear();
                            ConfirmarContraseniaPasswordBox.Clear();
                            CodigoConfirmacionTextBox.Clear();
                            MainWindow mainWindow = new MainWindow();
                            mainWindow.WindowState = Application.Current.MainWindow.WindowState;
                            mainWindow.WindowStyle = Application.Current.MainWindow.WindowStyle;
                            Application.Current.MainWindow.Content = mainWindow.Content;
                        }
                        else
                        {
                            MessageBox.Show(Lang.ErrorCuentaNoRegistrada_MSJ);
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
                    MessageBox.Show(Lang.AlertaFormatoContraseniaNoValido_MSJ);
                }
            }
        }

        private void CancelarButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.WindowState = Application.Current.MainWindow.WindowState;
            mainWindow.WindowStyle = Application.Current.MainWindow.WindowStyle;
            Application.Current.MainWindow.Content = mainWindow.Content;
        }

        private bool RevisarLlenadoFormulario()
        {
            string nombreUsuario = NombreUsuarioTextBox.Text;
            string correo = CorreoTextBox.Text;
            string contrasenia = ContraseniaPasswordBox.ToString();
            string confirmacionContrasenia = ConfirmarContraseniaPasswordBox.ToString();
            string codigoConfirmacionIntroducido = CodigoConfirmacionTextBox.Text; 

            if (!string.IsNullOrEmpty(nombreUsuario) && !string.IsNullOrEmpty(correo) && !string.IsNullOrEmpty(contrasenia) && !string.IsNullOrEmpty(confirmacionContrasenia) && !string.IsNullOrEmpty(codigoConfirmacionIntroducido))
            {
                if (!nombreUsuario.Contains(" ") && !correo.Contains(" ") && !contrasenia.Contains(" ") && !confirmacionContrasenia.Contains(" "))
                {
                    if (contrasenia == confirmacionContrasenia)
                    {
                        try
                        {
                            bool esMismoCodigoConfirmacion = cuentaUsuarioServiceMgt.ComprobarCodigos(codigoConfirmacionAEnviarAlCorreo, codigoConfirmacionIntroducido);

                            if (esMismoCodigoConfirmacion)
                            {
                                return true;
                            }
                            else
                            {
                                MessageBox.Show(Lang.ErrorCodigosConfirmacionDiferentes_MSJ);
                            }
                        }
                        catch (EndpointNotFoundException)
                        {
                            MessageBox.Show(Lang.ErrorNoSeEncontroServidor_MSJ);
                            Environment.Exit(0); ;
                        }
                        catch (CommunicationObjectFaultedException)
                        {
                            MessageBox.Show(Lang.ErrorObjetoComunicacionConServidor_MSJ);
                            Environment.Exit(0);
                        }
                    }
                    else
                    {
                        MessageBox.Show(Lang.ErrorContraseniaNoCoincide_MSJ);
                    }
                }
                else
                {
                    MessageBox.Show(Lang.ErrorCamposConCaracterEspacio_MSJ);
                }
            }
            return false;
        }

        public bool RevisarformatoContrasenia()
        {
            string contrasenia = ContraseniaPasswordBox.Password.ToString();
            int longitudContrasenia = contrasenia.Length;
            bool esContraseniaValida = true;

            Regex expresionRegularLetras = new Regex(@"[a-zA-z]");
            Regex expresionRegularNumeros = new Regex(@"[0-9]");
            Regex caracteresEspeciales = new Regex("[!\"#\\$%&'()*+,-./:;=?@\\[\\]^_`{|}~]");

            if (longitudContrasenia < 6)
            {
                esContraseniaValida = false;
            }
            if (!expresionRegularLetras.IsMatch(contrasenia))
            {
                esContraseniaValida = false;
            }
            if (!expresionRegularNumeros.IsMatch(contrasenia))
            {
                esContraseniaValida = false;
            }
            if (!caracteresEspeciales.IsMatch(contrasenia))
            {
                esContraseniaValida = false;
            }

            return esContraseniaValida;
        }

        private void GeneroComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            indexGeneroComboBox = GeneroComboBox.SelectedIndex;
        }
    }

}
