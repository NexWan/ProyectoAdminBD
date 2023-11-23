using ProyectoAdminBD.MVVM.Model;
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
    /// Lógica de interacción para ElRegistro.xaml
    /// </summary>
    public partial class ElRegistro : UserControl
    {
        List<ElementosRegistro> listData;
        DataHolder holder;
        public ElRegistro()
        {
            InitializeComponent();
            holder = DataHolder.Instance;
        }

        private void UpdateList()
        {
            List<ElementosRegistro> temp = new QueryExecutor().ExecuteQuery(
                holder.currentQuery,
                row => new ElementosRegistro
                {
                    _no_oficialia = (int)row["NO_OFICIALIA"],
                    _municipio = row["id_Municipio"].ToString(),
                    _nombreOficial = row["nombreOficialMayor"].ToString()
                });
        }

        private void ExecOperation(object sender, RoutedEventArgs e)
        {

        }

        private void SelectManager(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
