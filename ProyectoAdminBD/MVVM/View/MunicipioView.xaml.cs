using Microsoft.Extensions.Configuration;
using ProyectoAdminBD.Connection;
using ProyectoAdminBD.MVVM.Model;
using ProyectoAdminBD.Theme;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ProyectoAdminBD.MVVM.View
{
    /// <summary>
    /// Interaction logic for Municipio.xaml
    /// </summary>
    public partial class MunicipioView : UserControl
    {
        bool wasTriggered = false;
        private IConfiguration _configuration;
        DataHolder holder;
        SqlCommand cmd;
        SqlDataReader reader;
        bool _clear = true;
        List<Municipio> listData;
        List<Pais> paises;
        List<Entidad> entidadList;
        public MunicipioView()
        {
            InitializeComponent();
            holder = DataHolder.Instance;
            var appConfig = new LoadConfig();
            _configuration = appConfig.Configuration;
            UpdateList();
            CountryCB.ItemsSource = GetCountries();
        }

        private void UpdateList()
        {
            List<Municipio> data = new QueryExecutor().ExecuteQuery(
               holder.currentQuery,
               row => new Municipio
               {
                   _id = row["id_municipio"].ToString(),
                   _entidad = row["id_entidad"].ToString(),
                   _name = row["nombre"].ToString()
               }
               );
            MunicipioListView.ItemsSource = data;
            listData = data;
        }

        private List<Pais> GetCountries()
        {
            string query;
            List<Pais> data = new QueryExecutor().ExecuteQuery(
                "SELECT * FROM pais",
                row => new Pais
                {
                    _id = row["id_pais"].ToString(),
                    _nombre = row["nombre"].ToString(),
                });
            paises = data;
            return data;
        }

        private List<Entidad> GetEntidads(string pais)
        {
            string query;
            if (pais == "no")
                query = "SELECT * FROM entidad";
            else
                query = $"SELECT * FROM entidad WHERE id_pais = '{pais}'";
            Debug.WriteLine(query);
            List<Entidad> data = new QueryExecutor().ExecuteQuery(
                query,
                row => new Entidad
                {
                    _id = row["id_entidad"].ToString(),
                    _nombre = row["nombre"].ToString(),
                    _pais = row["id_pais"].ToString()
                });
            entidadList = data;
            return data;
        }

        private void ExecOperation(object sender, RoutedEventArgs e)
        {
            string id = IdBox.Text;
            string name = NombreBox.Text;
            using(SqlConnection conn = new SqlConn(_configuration).GetConnection())
            {
                conn.Open();
                string context = (sender as Button).Content.ToString();
                string searchQ, insertQ, q;
                string? entity = EntityCB?.SelectedValue?.ToString();
                switch (context.ToUpper())
                {
                    case "GUARDAR":
                        if (CountryCB.SelectedItem == null || EntityCB?.SelectedItem == null || id == string.Empty)
                        {
                            new ShoInfoMsg(ShoInfoMsg.WARNING, "Los campos Id, Pais o Estado no pueden estar vacios, por favor verifique sus datos");
                            return;
                        }
                        searchQ = $"SELECT * FROM municipio WHERE id_municipio = '{id}'";
                        if(VerifyExistingValue(conn, searchQ))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "El dato ya existe en la BD, intente de nuevo");
                            return;
                        }
                        insertQ = $"INSERT INTO municipio VALUES ('{id}','{entity}','{name}')";
                        if(ExecQuery(conn, insertQ))
                            new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato insertado con exito!");
                        UpdateList();
                        goto LIMPIAR;
                        break;
                    case "MODIFICAR":
                        if (CountryCB.SelectedItem == null || EntityCB?.SelectedItem == null || id == string.Empty)
                        {
                            new ShoInfoMsg(ShoInfoMsg.WARNING, "Los campos Id, Pais o Estado no pueden estar vacios, por favor verifique sus datos");
                            return;
                        }
                        searchQ = $"SELECT * FROM municipio WHERE id_municipio = '{id}'";
                        if (!VerifyExistingValue(conn, searchQ))
                        {
                            new ShoInfoMsg(ShoInfoMsg.WARNING, "El dato no existe en la BD, intente de nuevo");
                            return;
                        }
                        q = $"UPDATE municipio SET nombre = '{name}' WHERE id_municipio = '{id}'";
                        if(ExecQuery(conn,q))
                        {
                            new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato actualizado con exito!");
                            UpdateList();
                            goto LIMPIAR;
                        }
                        break;
                    case "ELIMINAR":
                        searchQ = $"SELECT * FROM municipio WHERE id_municipio = '{id}'";
                        if (!VerifyExistingValue(conn, searchQ))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "ERROR!, dato no encontrado en la BD, intente de nuevo");
                            return;
                        }
                        searchQ = $"SELECT * FROM persona_registrada WHERE idmunicipio = '{id}'";
                        if (VerifyExistingValue(conn, searchQ))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "Este dato tiene hijos!, no se puede eliminar");
                            return;
                        }
                        q = $"DELETE FROM municipio WHERE id_municipio = '{id}'";
                        if (ExecQuery(conn, q))
                            new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato eliminado con exito!");
                        UpdateList();
                        goto LIMPIAR;
                    case "LIMPIAR":
                        goto LIMPIAR;
                }
            }
        LIMPIAR:
            IdBox.Text = string.Empty;
            CountryCB.SelectedItem = null;
            CountryCB.SelectedValue = null;
            NombreBox.Text = string.Empty;
            IdBox.IsEnabled = true;
            DisableButtons();
            UpdateList();
            wasTriggered = false;
            _clear = true;
        }

        private bool ExecQuery(SqlConnection conn, string query)
        {
            Debug.WriteLine(query);
            if (reader != null) reader.Close();
            cmd = conn.CreateCommand();
            cmd.CommandText = query;
            if (cmd.ExecuteNonQuery() > 0)
            {
                return true;
            }
            return false;
        }

        private bool VerifyExistingValue(SqlConnection conn, string query)
        {
            cmd = conn.CreateCommand();
            cmd.CommandText = query;
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                reader.Close();
                return true;
            }
            return false;
        }

        private void CountryCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            object selectedItem = comboBox.SelectedItem;
            object selectedValue = comboBox.SelectedValue;
            if (selectedValue == null)
            {
                EntityCB.SelectedValue = null;
                EntityCB.SelectedItem = null;
            }
            else
                EntityCB.ItemsSource = GetEntidads(selectedValue.ToString());

        }

        private async void SearchByTextBox(object sender, TextChangedEventArgs e)
        {
            TextBox currTxt = (TextBox)sender;
            string txt = currTxt.Text;
            if (txt == string.Empty)
                UpdateList();
            else if (txt != string.Empty)
            {
                if (txt != string.Empty && currTxt == IdBox && _clear)
                {
                    EnableButtons();
                    MunicipioListView.ItemsSource = await GetMunicipiosAsync(txt, "ID");
                }
                else if (txt != string.Empty && currTxt != IdBox && _clear)
                {
                    EnableButtons();
                    MunicipioListView.ItemsSource = await GetMunicipiosAsync(txt, "NOMBRE");
                }
            }
            if (MunicipioListView.Items.Count == 1)
            {
                MunicipioListView.SelectedIndex = 0;
                wasTriggered = true;
            }
        }

        private async Task<List<Municipio>> GetMunicipiosAsync(string id, string type)
        {
            List<Municipio> list = new List<Municipio>();
            foreach (Municipio obj in listData)
            {
                if (obj._id.ToUpper().Contains(id.ToUpper()) && type == "ID")
                    list.Add(obj);
                if (obj._name.ToUpper().Contains(id.ToUpper()) && type == "NOMBRE")
                    list.Add(obj);
            }
            return list;
        }

        private void SelectManager(object sender, SelectionChangedEventArgs e)
        {
            EntityCB.ItemsSource = GetEntidads("no");
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                _clear = false;
                IdBox.Text = ((Municipio)item)._id;
                NombreBox.Text = ((Municipio)item)._name;
                EntityCB.SelectedItem = entidadList.FirstOrDefault(m => m._id == ((Municipio)item)._entidad);
                CountryCB.SelectedItem = paises.FirstOrDefault(p => p._id == ((Entidad)EntityCB.SelectedItem)._pais);
                EntityCB.SelectedItem = entidadList.FirstOrDefault(m => m._id == ((Municipio)item)._entidad);
                EnableButtons();
            }
            if (!wasTriggered)
            {
                IdBox.IsEnabled = false;
            }
            Debug.WriteLine(wasTriggered);
        }

        private void DisableButtons()
        {
            Save.IsEnabled = false;
            Update.IsEnabled = false;
            Delete.IsEnabled = false;
            Clear.IsEnabled = false;
        }

        public void EnableButtons()
        {
            Save.IsEnabled = true;
            Update.IsEnabled = true;
            Delete.IsEnabled = true;
            Clear.IsEnabled = true;
        }
    }
}
