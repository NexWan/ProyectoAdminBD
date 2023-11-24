using Microsoft.Extensions.Configuration;
using ProyectoAdminBD.Connection;
using ProyectoAdminBD.MVVM.Model;
using ProyectoAdminBD.Theme;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ProyectoAdminBD.MVVM.View
{
    /// <summary>
    /// Lógica de interacción para ElRegistro.xaml
    /// </summary>
    public partial class ElRegistro : UserControl
    {
        private IConfiguration _configuration;
        List<ElementosRegistro> listData;
        List<Municipio> municipios;
        DataHolder holder;
        SqlDataReader reader;
        SqlCommand cmd;
        bool _clear;
        TextBox[] TextBoxes;
        public ElRegistro()
        {
            InitializeComponent();
            holder = DataHolder.Instance;
            MunicipioCB.ItemsSource = GetMunicipios();
            var appConfig = new LoadConfig();
            _configuration = appConfig.Configuration;
            UpdateList();
        }

        private void UpdateList()
        {
            List<ElementosRegistro> temp = new QueryExecutor().ExecuteQuery(
                holder.currentQuery,
                row => new ElementosRegistro
                {
                    _no_oficialia = Convert.ToInt32(row["NO_OFICIALIA"]),
                    _municipio = row["id_Municipio"].ToString(),
                    _nombreOficial = row["nombreOficialMayor"].ToString(),
                    _apPaternoOficial = row["apPaternoOficialMayor"].ToString(),
                    _apMaternoOficial = row["apMaternoOficialMayor"].ToString(),
                    _nombreAsistente = row["nombreAsienta"].ToString(),
                    _apMaternoAsistente = row["apMaternoAsienta"].ToString(),
                    _apPaternoAsistente = row["apPaternoAsienta"].ToString()
                });
            RegistroTable.ItemsSource = temp;
            listData = temp;
            TextBoxes = new TextBox[] {NumOficialia,NombreOficial,ApMaternoAs,ApMaternoOfi,ApPaternoAs,ApPaternoOfi,NomAsistente};
            DisableButtons();
        }

        private List<Municipio> GetMunicipios()
        {
            List<Municipio> temp = new QueryExecutor().ExecuteQuery(
                "SELECT * FROM municipio",
                row => new Municipio
                {
                    _id = row["id_municipio"].ToString(),
                    _name = row["nombre"].ToString(),
                    _entidad = row["id_entidad"].ToString()
                }
                );
            municipios = temp;
            return temp;
        }

        private void ExecOperation(object sender, RoutedEventArgs e)
        {
            var numOficialia = NumOficialia.Text;
            string? municipio = MunicipioCB?.SelectedValue?.ToString();
            string nomOficial = NombreOficial.Text;
            string apPatOficial = ApPaternoOfi.Text;
            string apMatOficial = ApMaternoOfi.Text;
            string nomAsistente = NomAsistente.Text;
            string apPatAsistente = ApPaternoAs.Text;
            string apMatAsistente = ApMaternoAs.Text;
            Debug.WriteLine(numOficialia, nomOficial, apPatOficial, apMatOficial, nomAsistente);
            if ((sender as Button).Content.ToString().ToUpper() == "LIMPIAR")
            {
                Clean();
                return;
            }
            if(string.IsNullOrEmpty(municipio) || string.IsNullOrEmpty(numOficialia))
            {
                new ShoInfoMsg(ShoInfoMsg.ERROR, "Los campos oficialia y municipio no pueden estar vacios! intente de nuevo");
                return;
            }
            if(string.IsNullOrEmpty(nomOficial) || string.IsNullOrEmpty(apPatOficial) || string.IsNullOrEmpty(apMatOficial) || string.IsNullOrEmpty(nomAsistente) || string.IsNullOrEmpty(apMatAsistente) || string.IsNullOrEmpty (apPatAsistente)) {
                bool conf = new ShoInfoMsg().ShowConfirmDialog("1 o mas campos se encuentran vacios, estos se reescribiran, desea continuar?");
                if(!conf) return;
            }
            using (SqlConnection conn = new SqlConn(_configuration).GetConnection())
            {
                conn.Open();
                string q, insertQ, searchQ;
                string context = (sender as Button).Content.ToString();
                switch (context.ToUpper())
                {
                    case "GUARDAR":
                        searchQ = $"SELECT * FROM elementos_registro WHERE no_oficialia = {numOficialia}";
                        if (VerifyExistingValue(conn, searchQ))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "Este dato ya existe en la BD!, intente de nuevo");
                            return;
                        }
                        insertQ = $"INSERT INTO elementos_registro VALUES({numOficialia},'{municipio}','{nomOficial}','{apPatOficial}','{apMatOficial}','{nomAsistente}','{apPatAsistente}','{apMatAsistente}')";
                        if(ExecQuery(conn, insertQ))
                            new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato insertado con exito!");
                        UpdateList();
                        Clean();
                        break;
                    case "MODIFICAR":
                        searchQ = $"SELECT * FROM elementos_registro WHERE no_oficialia = {numOficialia}";
                        if (!VerifyExistingValue(conn, searchQ))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "Este dato no existe en la BD!, intente de nuevo");
                            return;
                        }
                        q = $"UPDATE elementos_registro SET id_municipio = '{municipio}', nombreOficialMayor = '{nomOficial}', " +
                            $"apPaternoOficialMayor = '{apPatOficial}', apMaternoOficialMayor = '{apPatOficial}', nombreAsienta = '{nomAsistente}'" +
                            $", apPaternoAsienta = '{apPatAsistente}', apMaternoAsienta = '{apMatAsistente}'";
                        if (ExecQuery(conn, q))
                            new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato actualizado con exito!");
                        UpdateList();
                        Clean();
                        break;
                    case "ELIMINAR":
                        string[] verify = new string[] {$"SELECT * FROM persona_registrada WHERE no_oficialia = {numOficialia}",
                            $"SELECT * FROM empleados WHERE no_oficialia = {numOficialia}"};
                        for (int i = 0; i < verify.Length; i++)
                        {
                            if (VerifyExistingValue(conn, verify[i]))
                            {
                                new ShoInfoMsg(ShoInfoMsg.ERROR, "Este dato es hijo de alguna tabla!, intente de nuevo");
                                return;
                            }
                        }
                        q = $"DELETE FROM elementos_registro WHERE no_oficialia = '{numOficialia}'";
                        if (ExecQuery(conn, q))
                            new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato eliminado con exito!");
                        UpdateList();
                        Clean();
                        break;
                }
            }
        }
        
        private void Clean()
        {
            NomAsistente.Text = string.Empty;
            MunicipioCB.SelectedItem = null;
            MunicipioCB.SelectedValue = null;
            NumOficialia.Text = string.Empty;
            ApMaternoAs.Text = string.Empty;
            ApPaternoAs.Text = string.Empty;
            NombreOficial.Text = string.Empty;
            ApMaternoOfi.Text = string.Empty;
            ApPaternoOfi.Text = string.Empty;
            _clear = true;
            DisableButtons();
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
            if (reader != null) reader.Close();
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
        private bool AreAllTextBoxesEmptyExceptOne(TextBox exceptTextBox, params TextBox[] textBoxes)
        {
            return textBoxes.Except(new[] { exceptTextBox }).All(tb => string.IsNullOrEmpty(tb.Text));
        }


        private async void SearchByTextBox(object sender, TextChangedEventArgs e)
        {
            TextBox currTxt = sender as TextBox;
            string txt = currTxt.Text;
            if (string.IsNullOrEmpty(txt) && AreAllTextBoxesEmptyExceptOne(currTxt, TextBoxes))
                UpdateList();
            else if(txt != string.Empty && AreAllTextBoxesEmptyExceptOne(currTxt, TextBoxes) && _clear)
            {
                EnableButtons();
                RegistroTable.ItemsSource = await GetElementosRegistroAsync(txt, currTxt.Name);
            }
            if (RegistroTable.Items.Count == 1)
                RegistroTable.SelectedIndex = 0;
        }

        private async Task<List<ElementosRegistro>> GetElementosRegistroAsync(string value, string dataType)
        {
            List<ElementosRegistro> list = new List<ElementosRegistro> ();
            value = value.ToUpper();
            foreach(ElementosRegistro obj in listData)
            {
                if(dataType == "NumOficialia" && obj._no_oficialia == Convert.ToInt32(value))
                    list.Add(obj);
            }
            return list;
        }

        private void SelectManager(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                EnableButtons();
                ElementosRegistro sel = (ElementosRegistro)item;
                _clear = false;
                NumOficialia.Text = sel._no_oficialia.ToString();
                NombreOficial.Text = sel._nombreOficial;
                ApPaternoOfi.Text = sel._apPaternoOficial;
                ApMaternoOfi.Text = sel._apMaternoOficial;
                NomAsistente.Text = sel._nombreAsistente;
                ApPaternoAs.Text = sel._apPaternoAsistente;
                ApMaternoAs.Text = sel._apMaternoAsistente;
                MunicipioCB.SelectedItem = municipios.FirstOrDefault(m => m._id == sel._municipio);
            }
        }

        private void ValidateNumeric(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
