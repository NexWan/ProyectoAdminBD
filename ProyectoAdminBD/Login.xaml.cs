using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Http;
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

namespace ProyectoAdminBD
{
    /// <summary>
    /// Clase manejadora de eventos de Login
    /// </summary>
    public partial class Login : Window
    {
        private IConfiguration _configuration;

        public Login()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            var appConfig = new LoadConfig();
            _configuration = appConfig.Configuration;
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
            }catch (Exception ex)
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

            Debug.WriteLine(passwordBox.Password.Length);

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
            SqlConnection conn = getConn();
            conn.Open();
            String? user = FindVisualChild<TextBox>(LoginText, "LoginText").Text;
            String? pwd = FindVisualChild<PasswordBox>(MyPasswordBox, "pwdBox").Password;
            if (CheckForValidText(pwd))
            {
                String query = $"SELECT * FROM empleados WHERE id_empleado={user} AND clave= '{pwd}'";
                SqlCommand? cmd = conn?.CreateCommand();
                try
                {
                    cmd.CommandText = query;
                    SqlDataReader? reader = cmd.ExecuteReader();
                    if (reader.Read())  //Si resulta cualquier valor significa que es verdadero
                        MessageBox.Show($"Usuario: {user} \n Contraseña: {pwd} \n Contraseña valida");
                    else MessageBox.Show("Usuario y/o contraseña invalida");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ocurrio un error! Comprueba el usuario o contraseña", "Error!",MessageBoxButton.OK,MessageBoxImage.Exclamation);
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
            conn.Open();
            String? name = FindVisualChild<TextBox>(SignUpName, "LoginText").Text;
            String? last_name = FindVisualChild<TextBox>(SignUpLastName, "LoginText").Text;
            String? pwd = FindVisualChild<PasswordBox>(SignUpBox, "pwdBox").Password;

            MessageBox.Show(name + "\n" + last_name + "\n" + pwd);
        }
        
        private SqlConnection? getConn() {
            try
            {
                return new SqlConn(_configuration).GetConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ha ocurrido un error! {ex.Message}","ERROR!",MessageBoxButton.OK,MessageBoxImage.Error);
                return null;
            }
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
                MessageBox.Show("Contraseña o usuario con caracteres ilegales, intente de nuevo","ERROR!",MessageBoxButton.OK,MessageBoxImage.Error);
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
    }
}
