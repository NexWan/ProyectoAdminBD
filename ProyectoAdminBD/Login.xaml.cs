using System;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Extensions.Configuration;
using ProyectoAdminBD.Connection;
using ProyectoAdminBD.Theme;

namespace ProyectoAdminBD
{
    /// <summary>
    /// Clase manejadora de eventos de Login
    /// </summary>
    public partial class Login : Window
    {
        private IConfiguration _configuration;
        DataHolder dh;

        public Login()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            var appConfig = new LoadConfig();
            _configuration = appConfig.Configuration;
            dh = DataHolder.Instance;
        }


        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CloseApp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Minimize(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.WindowState = WindowState.Minimized;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void passwordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = (PasswordBox)sender;

            // Find the TextBlock named "PlaceholderTextBlock" within the PasswordBox template
            TextBlock? watermarkTextBlock = FindVisualChild<TextBlock>(passwordBox, "PlaceholderTextBlock");

            if (watermarkTextBlock != null)
            {
                if (FindVisualChild<PasswordBox>(passwordBox, "pwdBox").Password.Length == 0)
                    watermarkTextBlock.Visibility = Visibility.Visible;
                else
                    watermarkTextBlock.Visibility = Visibility.Hidden;
            }
        }

        private T? FindVisualChild<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject? child = VisualTreeHelper.GetChild(parent, i);
                if (child is T element && element.Name == name)
                {
                    return element;
                }

                T? childOfChild = FindVisualChild<T>(child, name);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }

        /// <summary>
        /// Function that handles all the login logic 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickLogin(object sender, RoutedEventArgs e)
        {
            SqlConnection? conn = getConn();
            if (conn == null)
            {
                MessageBox.Show("Ha ocurrido un error con la conexion!, comprueba la conexion", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            conn?.Open();
            String? user = FindVisualChild<TextBox>(LoginText, "LoginText")?.Text;
            String? pwd = FindVisualChild<PasswordBox>(MyPasswordBox, "pwdBox")?.Password;
            if (CheckForValidText(pwd))
            {
                String query = $"SELECT * FROM empleados WHERE id_empleado={user} AND clave= '{pwd}'";
                SqlCommand? cmd = conn?.CreateCommand();
                try
                {
                    cmd.CommandText = query;
                    SqlDataReader? reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string logUser = reader.GetString(1);
                        string logLastFName = reader.GetString(2);
                        string logLastMName = reader.GetString(3);
                        var logId = reader.GetDecimal(0);
                        dh.setUserLastMName(logLastMName);
                        dh.SetUserLastFName(logLastFName);
                        dh.ChangeUser(logUser);
                        dh.setUserId(logId);
                        // Perform UI-related operations on the UI thread using Dispatcher
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"Bienvenido {logUser}!", "Login exitoso!", MessageBoxButton.OK, MessageBoxImage.Information);
                            new MainWindow().Show();
                            Close();
                        });
                    }
                    else
                    {
                        // Perform UI-related operations on the UI thread using Dispatcher
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show("Usuario y/o contraseña invalida", "Datos incorrectos!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        });
                    }
                }
                catch (Exception ex)
                {
                    // Perform UI-related operations on the UI thread using Dispatcher
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("Ocurrio un error! Comprueba el usuario o contraseña", "Error!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    });
                }
            }
        }

        /// <summary>
        /// Function that handles all the sign up logic 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickSignup(object sender, RoutedEventArgs e)
        {
            SqlConnection conn = getConn();
            if (conn == null)
            {
                MessageBox.Show("Ha ocurrido un error con la conexion!, comprueba la conexion", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            conn.Open();

            string? name = FindVisualChild<TextBox>(SignUpName, "LoginText")?.Text;
            string? last_nameF = FindVisualChild<TextBox>(LastNameFather, "LoginText")?.Text;
            string? last_nameM = FindVisualChild<TextBox>(LastNameMother, "LoginText")?.Text;
            string? pwd = FindVisualChild<PasswordBox>(SignUpBox, "pwdBox")?.Password;

            if (pwd == null || last_nameF == null || last_nameM == null || pwd == null)
            {
                MessageBox.Show("Asegurate que los campos no esten vacios!", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (CheckForValidText(name) && CheckForValidText(last_nameM) && CheckForValidText(last_nameF) && CheckForValidText(pwd))
            {
                Random r = new Random();
                long numControl = r.Next(1, 99999999);

                string query = $"INSERT INTO empleados VALUES ({numControl}, '{name}', '{last_nameF}', '{last_nameM}', '{pwd}')";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 1)
                    {
                        MessageBox.Show($"Usuario creado exitosamente!\nSu número de control es: {numControl}", "Usuario creado!", MessageBoxButton.OK, MessageBoxImage.Information);
                        MessageBox.Show($"Se le redireccionará a la pantalla de inicio de sesión", "Usuario creado!", MessageBoxButton.OK, MessageBoxImage.Information);
                        EnableScene(LoginScene, e);
                        LoginScene.IsChecked = true;
                        SignupScene.IsChecked = false;
                    }
                    else
                    {
                        MessageBox.Show("No se pudo crear el usuario. Verifique los datos y vuelva a intentarlo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error al crear el usuario: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Por favor valida que no hayas usado caracteres ilegales.", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private SqlConnection? getConn()
        {
            if (dh.CheckForInternetConnection())
            {
                try
                {
                    return new SqlConn(_configuration).GetConnection();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ha ocurrido un error! {ex.Message}", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Function to turn a SecureString to a normal string
        /// </summary>
        /// <param name="secureString"></param>
        /// <returns>Normal string</returns>
        String? ConvertToUnsecureString(SecureString secureString)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        /// <summary>
        /// Function that checks if a string contains invalid text (sqlinjectio ex)
        /// </summary>
        /// <param name="text"></param>
        /// <returns>bool</returns>
        private bool CheckForValidText(String text)
        {
            Regex regex = new Regex("[@#'\"]");
            MatchCollection matchCollection = regex.Matches(text);
            if (matchCollection.Count > 0)
            {
                MessageBox.Show("Contraseña o usuario con caracteres ilegales, intente de nuevo", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Function to switch between scenes on the login window class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnableScene(object sender, RoutedEventArgs e)
        {
            RadioButton? radioButton = sender as RadioButton;
            if (radioButton == null) return;
            string name = radioButton.Name;
            String receivedScene = name;
            if (receivedScene != null)
            {
                InicioWindow.Visibility = Visibility.Hidden;
                LoginWindow.Visibility = radioButton.Name == "LoginScene" ? Visibility.Visible : Visibility.Hidden;
                SignupWindow.Visibility = radioButton.Name == "SignupScene" ? Visibility.Visible : Visibility.Hidden;
                Tittle.Text = radioButton.Name == "LoginScene" ? "Log in" : "Sign up";

                if (radioButton.Name == "InvitadoScene") MessageBox.Show("Soy la escena de invitado");

            }
        }

        /// <summary>
        /// Function that sets the color when the close or minize button is hovered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ellipse_MouseEnter(object sender, MouseEventArgs e)
        {
            Ellipse? ellipse = sender as Ellipse;
            ellipse.Fill = (ellipse.Name == "close") ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCFF0000")) : (ellipse.Name == "minimize") ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCFFFF00")) : null;
        }

        private void Ellipse_MouseLeave(object sender, MouseEventArgs e)
        {
            Ellipse? ellipse = sender as Ellipse;
            ellipse.Fill = (ellipse.Name == "close") ? new SolidColorBrush(Colors.DarkRed) : (ellipse.Name == "minimize") ? new SolidColorBrush(Colors.Yellow) : null;
        }

        private void KeyManager(object sender, KeyEventArgs e)
        {
            PasswordBox? pb = sender as PasswordBox;
            if (pb != null)
            {
                if ((pb.Name == "MyPasswordBox") && e.Key == Key.Return)
                    ClickLogin((object)MyPasswordBox, e);
                else if (pb.Name == "SignUpBox" && e.Key == Key.Return)
                    ClickSignup((object)SignUpBox, e);
            }
        }
    }
}
