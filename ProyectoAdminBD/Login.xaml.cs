using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ProyectoAdminBD.Connection;

namespace ProyectoAdminBD
{
    /// <summary>
    /// Interaction logic for Login.xaml
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
            TextBlock watermarkTextBlock = FindVisualChild<TextBlock>(passwordBox, "PlaceholderTextBlock");

            Debug.WriteLine(passwordBox.Password.Length);

            if (watermarkTextBlock != null)
            {
                if (FindVisualChild<PasswordBox>(passwordBox,"pwdBox").Password.Length == 0)
                    watermarkTextBlock.Visibility = Visibility.Visible;
                else
                    watermarkTextBlock.Visibility = Visibility.Hidden;
            }
        }

        private T FindVisualChild<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T element && element.Name == name)
                {
                    return element;
                }

                T childOfChild = FindVisualChild<T>(child, name);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }

        private void ClickLogin(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Hola");
            SqlConnection conn = new SqlConn(" "," ", "actas").GetConnection();
            conn.Open();
            String user = LoginText.Text;
            String pwd = FindVisualChild<PasswordBox>(MyPasswordBox, "pwdBox").Password;
            Debug.WriteLine($"{pwd} ------- {user}");
        }

        String ConvertToUnsecureString(SecureString secureString)
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
