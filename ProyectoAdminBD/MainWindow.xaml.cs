using ProyectoAdminBD.Theme;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ProyectoAdminBD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public RadioButton? option1, option2, option3;
        public MainWindow()
        {
            InitializeComponent();
            DataHolder dh = DataHolder.Instance;
            this.Visibility = Visibility.Visible;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            LoggText.Text += $" {dh.UserLoggedIn}";
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

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton? rb = sender as RadioButton;
            if(rb != null)
            {
                if (ListaRegistros.Visibility == Visibility.Visible)
                {
                    ListaRegistros.Visibility = Visibility.Collapsed;
                }
                else if (ListaRegistros.Visibility == Visibility.Collapsed) ListaRegistros.Visibility = Visibility.Visible;
                else
                {
                    if (rb.IsChecked == true) // Use rb.IsChecked to check if RadioButton is checked
                    {
                        ListaRegistros.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        ListaRegistros.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

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
