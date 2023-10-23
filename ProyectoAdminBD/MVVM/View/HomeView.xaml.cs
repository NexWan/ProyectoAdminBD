using Microsoft.Win32;
using ProyectoAdminBD.Theme;
using System;
using System.Windows;
using System.Windows.Controls;

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
            dateTextBlock.Text += $" {DateTime.Now.ToString("dd MM yyyy")}";
        }


        /// <summary>
        /// Function where the user will be able to upload a profile picture and store it on the database 
        /// (It's still on the making, but the file dialog is working at the moment)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangePfp(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            saveFileDialog.Title = "Save an Image";
            saveFileDialog.DefaultExt = "png";

            // Set the filter to allow only image files (e.g., PNG, JPG, BMP, GIF)
            saveFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif";

            saveFileDialog.ShowDialog();
        }
    }
}
