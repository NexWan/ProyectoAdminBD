using ProyectoAdminBD.MVVM.Model;
using ProyectoAdminBD.Theme;
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
using System.Windows.Shapes;

namespace ProyectoAdminBD
{
    /// <summary>
    /// Interaction logic for SeleccionarAbuelos.xaml
    /// </summary>
    public partial class SeleccionarAbuelos : Window
    {
        string id, pais, genero;
        DataHolder holder;
        public SeleccionarAbuelos()
        {
            InitializeComponent();
            holder = DataHolder.Instance;
            TablaAbuelos.ItemsSource = GetAbuelos();
        }
        
        private List<Abuelos> GetAbuelos()
        {
            string query = "";
            for(int i = 0; i < holder.abuelos.Count; i++)
                Debug.WriteLine(holder.abuelos[i]);
            if (string.IsNullOrEmpty(holder.selectedAbueloId))
                query = $"SELECT * FROM abuelos WHERE id_genero = '{holder.parentescoAbuelo}'";
            else
            {
                query = $"SELECT * FROM abuelos WHERE id_genero = '{holder.parentescoAbuelo}' AND id_abuelo != {holder.abuelos.First()}";
                for(int i = 1; i < holder.abuelos.Count; i++)
                {
                    query += $"AND id_abuelo != '{holder.abuelos[i]}'";
                }
            }

            return new QueryExecutor().ExecuteQuery(
                query,
                row => new Abuelos
                {
                    _id = row["id_abuelo"].ToString(),
                    _pais = row["id_pais"].ToString(),
                    _genero = row["id_genero"].ToString(),
                    _nombres = row["nomAbuelo"].ToString(),
                    _ap_paterno = row["apPaterno"].ToString(),
                    _ap_materno = row["apMaterno"].ToString()
                });
        }

        private void TablaAbuelos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                Save.IsEnabled = true;
                Abuelos abuelos = (Abuelos)item;
                //_clear = false;
                NOMBRE.Text = abuelos._nombres;
                APMATERNO.Text = abuelos._ap_materno;
                APPATERNO.Text = abuelos._ap_paterno;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if(TablaAbuelos.SelectedItems.Count == 0) {
                new ShoInfoMsg(ShoInfoMsg.ERROR, "Seleccione 1 dato!"); 
                return;
            }
            Abuelos? abuelo = TablaAbuelos.SelectedItem as Abuelos;
            id = abuelo._id;
            holder.selectedAbueloId = id;
            holder.abuelos.Add(id);
            Debug.WriteLine(holder.abuelos.Last());
            this.Close();
        }
    }
}
