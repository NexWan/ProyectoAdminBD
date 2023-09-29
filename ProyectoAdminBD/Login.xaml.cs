using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ProyectoAdminBD.Connection;

namespace ProyectoAdminBD
{
    /// <summary>
    /// Clase manejadora de eventos de Login
    /// </summary>
    public partial class Login : Window
    {

        public Login()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
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

        //Logica detras del boton de login
        private void ClickLogin(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Hola");
            SqlConnection conn = new SqlConn("actas").GetConnection();
            conn.Open();
            String? user = FindVisualChild<TextBox>(LoginText, "LoginText").Text;
            String? pwd = FindVisualChild<PasswordBox>(MyPasswordBox, "pwdBox").Password;
            Regex regex = new Regex("[@#'\"]");
            MatchCollection matchCollection = regex.Matches(pwd);

            if(matchCollection.Count > 0)
            {
                MessageBox.Show("Contraseña invalida, has puesto caracteres invalidos");
                return;
            }
            String query = $"SELECT * FROM empleados WHERE id_empleado={user} AND clave= '{pwd}'";
            SqlCommand? cmd = conn.CreateCommand();
            try
            {
                cmd.CommandText = query;
                SqlDataReader? reader = cmd.ExecuteReader();
                if (reader.Read())  //Si resulta cualquier valor significa que es verdadero
                    MessageBox.Show($"Usuario: {user} \n Contraseña: {pwd} \n Contraseña valida");
                else MessageBox.Show("Usuario y/o contraseña invalida");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ocurrio un error! Comprueba el usuario o contraseña");
            }
        }

        //Este metodo sirve para poder convertir un string seguro a un string normal
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
    }
}
