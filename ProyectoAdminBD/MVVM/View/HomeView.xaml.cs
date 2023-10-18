using Microsoft.Win32;
using ProyectoAdminBD.Theme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProyectoAdminBD.MVVM.View
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        DataHolder dh;
        public HomeView()
        {
            InitializeComponent();
            dh = DataHolder.Instance;
            WelcomeText.Text += " " + dh.UserLoggedIn;
            UserInfoBox.Text += $"\n Nombre: {dh.UserLoggedIn}" +
                $"\n Apellido Paterno: {dh.userLastFName}" +
                $"\n Apellido Materno: {dh.userLastMName}" +
                $"\n Numero de control: {dh.userId}";
        }

        private void ChangePfp(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = @"C:\";
            saveFileDialog.Title = "Save an Image";
            saveFileDialog.DefaultExt = "png";

            // Set the filter to allow only image files (e.g., PNG, JPG, BMP, GIF)
            saveFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All Files|*.*";

            saveFileDialog.ShowDialog();
        }
    }
}
