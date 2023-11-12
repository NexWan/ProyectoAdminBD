using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Lógica de interacción para PresentadoView.xaml
    /// </summary>
    public partial class PresentadoView : UserControl
    {
        public PresentadoView()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            String x = DescBox.Text;
            Debug.WriteLine(x);
        }
    }
}
