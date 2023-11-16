using ProyectoAdminBD.MVVM.Model;
using ProyectoAdminBD.Theme;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;


namespace ProyectoAdminBD.MVVM.View
{
    /// <summary>
    /// Lógica de interacción para PaternalesView.xaml
    /// </summary>
    public partial class PaternalesView : UserControl
    {
        DataHolder holder;
        public PaternalesView()
        {
            InitializeComponent();
            holder = DataHolder.Instance;
            CountryCB.ItemsSource = GetPaises();
            CountryCB2.ItemsSource = CountryCB.ItemsSource;
            GeneroBox.ItemsSource = GetGeneros();
        }

        private List<Pais> GetPaises()
        {
            List<Pais> data = new QueryExecutor().ExecuteQuery(
                "SELECT id_pais, nombre, nacionalidad FROM pais",
                row => new Pais
                {
                    _id = row["id_pais"].ToString(),
                    _nombre = row["nombre"].ToString(),
                    _nacionalidad = row["nacionalidad"].ToString()
                }
                );
            return data;
        }

        private List<Genero> GetGeneros()
        {
            List<Genero> data = new QueryExecutor().ExecuteQuery(
                "SELECT id_genero, DESCRIPCION FROM genero",
                row => new Genero
                {
                    Id = row["Id_GENERO"].ToString(),
                    Descripcion = row["Descripcion"].ToString()
                }
                );
            return data;
        }

        private void CountryCB_Selected(object sender, System.Windows.RoutedEventArgs e)
        {
            
        }

        private void CountryCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            // You can access the selected item or selected value from the ComboBox
            object selectedItem = comboBox.SelectedItem;
            object selectedValue = comboBox.SelectedValue; // Assu

            Debug.WriteLine(selectedValue);
        }

        private void ValidateNumeric(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
