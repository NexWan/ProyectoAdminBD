
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Extensions.Configuration;
using ProyectoAdminBD.Connection;
using ProyectoAdminBD.MVVM.Model;
using ProyectoAdminBD.Theme;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ProyectoAdminBD.MVVM.View
{
    /// <summary>
    /// Lógica de interacción para RegistersWindow.xaml
    /// </summary>
    public partial class RegistersWindow : UserControl
    {
        private IConfiguration _configuration;
        DataHolder holder;
        SqlCommand cmd;
        SqlDataReader reader;
        bool _clear = true;

        public List<object> listData = new List<object>();
        public List<object> ogData = new List<object>();
        public RegistersWindow()
        {
            InitializeComponent();
            var appconfig = new LoadConfig();
            _configuration = appconfig.Configuration;
            Debug.WriteLine(DataHolder.Instance.currentQuery);
            holder = DataHolder.Instance;
            UpdateList();
            DisableButtons();
            IdBox.MaxLength = 2;
            DescBox.MaxLength = 30;
            ogData = listData;
        }

        private void UpdateList()
        {
            List<Genero> data = new QueryExecutor().ExecuteQuery(
                holder.currentQuery,
                row => new Genero
                {
                    Id = row["Id_GENERO"].ToString(),
                    Descripcion = row["Descripcion"].ToString()
                }
                );
            myListView.ItemsSource = data;
            listData = data.Cast<object>().ToList();
        }

        private void ExecOperation(object sender, RoutedEventArgs e)
        {
            string? passedId = FindVisualChild<TextBox>(IdBox as TextBox, "LoginText").Text;
            string? passedDesc = FindVisualChild<TextBox>(DescBox as TextBox, "LoginText").Text;
            Button? button = sender as Button;
            if (button != null)
            {
                string? context = button.Content as string;
                if (context != null)
                {
                    if (!holder.CheckForValidText(passedId) || !holder.CheckForValidText(passedDesc))
                    {
                        new ShoInfoMsg("ERROR", "Caracteres ilegales detectados, intente de nuevo");
                        return;
                    }
                    Debug.Write(context);
                    switch (context.ToUpper())
                    {
                        case "GUARDAR":
                            try
                            {
                                String insertQ = $"INSERT INTO genero VALUES ('{passedId}', '{passedDesc}')";
                                string searchQ = $"SELECT * FROM genero WHERE id_genero = '{passedId}'";
                                using (SqlConnection conn = new SqlConn(_configuration).GetConnection())
                                {
                                    conn.Open();
                                    cmd = conn.CreateCommand();
                                    cmd.CommandText = searchQ;
                                    reader = cmd.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        new ShoInfoMsg("ERROR", "Dato ya existente en la BD, intente de nuevo");
                                        break;
                                    }
                                    reader.Close();
                                    cmd.CommandText = insertQ;
                                    if (cmd.ExecuteNonQuery() > 0)
                                    {
                                        new ShoInfoMsg("SUCCESS", "Dato insertado con exito!");
                                        UpdateList();
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ha ocurrido un error al insertar! {ex.Message}", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            break;
                        case "MODIFICAR":
                            Debug.WriteLine("Entro a modificar");
                            try
                            {
                                String q = $"UPDATE genero SET DESCRIPCION = '{passedDesc}' WHERE id_genero = '{passedId.ToUpper()}'";
                                using (SqlConnection conn = new SqlConn(_configuration).GetConnection())
                                {
                                    using (SqlCommand command = new SqlCommand(q, conn))
                                    {
                                        conn.Open();
                                        int rowsAffected = command.ExecuteNonQuery();
                                        Debug.WriteLine(rowsAffected);
                                        if (rowsAffected > 0)
                                        {
                                            new ShoInfoMsg("SUCCESS", "Dato actualizado con exito!");
                                            UpdateList();
                                            goto LIMPIAR;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ha ocurrido un error al actualizar! {ex.Message}", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            break;

                        case "ELIMINAR":
                            try
                            {
                                if (passedId == string.Empty)
                                {
                                    new ShoInfoMsg("WARNING", "No se recomienda eliminar elementos sin un ID expecifico, intente de nuevo!");
                                    return;
                                }
                                String q = $"DELETE FROM genero WHERE id_genero = '{passedId}'";
                                string verify = $"SELECT * FROM PERSONA_REGISTRADA WHERE id_genero = '{passedId}'";
                                using (SqlConnection conn = new SqlConn(_configuration).GetConnection())
                                {
                                    conn.Open();
                                    cmd = conn.CreateCommand();
                                    cmd.CommandText = verify;
                                    reader = cmd.ExecuteReader();
                                    if(reader.Read())
                                    {
                                        new ShoInfoMsg("ERROR", "No se pueden eliminar valores que ya estan en uso!");
                                        return;
                                    }
                                    reader.Close();
                                    cmd.CommandText = q;
                                    if (cmd.ExecuteNonQuery() > 0)
                                        new ShoInfoMsg("SUCCESS", "Dato eliminado con exito!");
                                }

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ha ocurrido un error al eliminar! {ex.Message}", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            break;
                        case "LIMPIAR":
                            goto LIMPIAR;
                            break;
                    }
                LIMPIAR:
                    FindVisualChild<TextBox>(IdBox as TextBox, "LoginText").Text = string.Empty;
                    FindVisualChild<TextBox>(DescBox as TextBox, "LoginText").Text = string.Empty;
                    IdBox.IsEnabled = true;
                    DisableButtons();
                    UpdateList();
                    _clear = true;
                }
            }
        }

        private async void SearchByTextBox(object sender, TextChangedEventArgs e)
        {
            TextBox IdTxtbox = FindVisualChild<TextBox>(sender as TextBox, "LoginText");
            if (IdTxtbox != null)
            {
                if (IdTxtbox.Text == string.Empty)
                    UpdateList();
                if (IdTxtbox.Text != string.Empty && ((TextBox)sender).Name == "IdBox" && _clear)
                {
                    EnableButtons();
                    myListView.ItemsSource = await ExecQueryAsync(IdTxtbox.Text, "ID");
                }
                else if (((TextBox)sender).Name != "IdBox" && _clear)
                {
                    myListView.ItemsSource = await ExecQueryAsync(IdTxtbox.Text, "DESC");
                }
                else if(FindVisualChild<TextBox>(DescBox as TextBox, "LoginText").Text == string.Empty && FindVisualChild<TextBox>(IdBox as TextBox, "LoginText").Text == string.Empty)
                {
                    DisableButtons();
                }
            }
        }

        private async Task<List<Genero>> ExecQueryAsync(string id, string type)
        {
            List<Genero> tempData = new List<Genero>();
            foreach (Genero obj in listData)
            {
                if (obj.Id.ToUpper().Contains(id.ToUpper()) && type == "ID")
                    tempData.Add(obj);
                else if(obj.Descripcion.ToUpper().Contains(id.ToUpper()) && type == "DESC")
                    tempData.Add(obj);
            }
            if(tempData.Count == 1)
            {
                FindVisualChild<TextBox>(IdBox as TextBox, "LoginText").Text = ((Genero)tempData[0]).Id;
                FindVisualChild<TextBox>(DescBox as TextBox, "LoginText").Text = ((Genero)tempData[0]).Descripcion;
                _clear = false;
            }
            else
            {
                FindVisualChild<TextBox>(DescBox as TextBox, "LoginText").Text = string.Empty;
            }
            return tempData;
        }

        public void EnableButtons()
        {
            Save.IsEnabled = true;
            Update.IsEnabled = true;
            Delete.IsEnabled = true;
            Clear.IsEnabled = true;
        }

        public void ClearTextBoxes()
        {
            FindVisualChild<TextBox>(IdBox as TextBox, "LoginText").Text = string.Empty;
        }

        public void DisableButtons()
        {
            Save.IsEnabled = false;
            Update.IsEnabled = false;
            Delete.IsEnabled = false;
            Clear.IsEnabled = false;
        }

        private T? FindVisualChild<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject? child = VisualTreeHelper.GetChild(parent, i);
                if (child is T element && element.Name == name)
                {
                    return element;
                }

                T? childOfChild = FindVisualChild<T>(child, name);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }

        private void myListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;

            if (item != null)
            {
                _clear = false;
                Debug.WriteLine(((Genero)item).Id + " " + ((Genero)item).Descripcion);
                FindVisualChild<TextBox>(IdBox as TextBox, "LoginText").Text = ((Genero)item).Id;
                IdBox.IsEnabled = false;
                FindVisualChild<TextBox>(DescBox as TextBox, "LoginText").Text = ((Genero)item).Descripcion;
                EnableButtons();
            }
        }
    }
}
