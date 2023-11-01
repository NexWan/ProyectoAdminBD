
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
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
            string passedId = FindVisualChild<TextBox>(IdBox as TextBox, "LoginText").Text;
            string passedDesc = FindVisualChild<TextBox>(DescBox as TextBox, "LoginText").Text;
            Button? button = sender as Button;
            if (button != null)
            {
                string? context = button.Content as string;
                if (context != null)
                {
                    if(!holder.CheckForValidText(passedId) || !holder.CheckForValidText(passedDesc))
                    {
                        MessageBox.Show("Caracteres ilegales detectados, por favor intente de nuevo", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    Debug.Write(context);
                    switch (context.ToUpper())
                    {
                        case "BUSCAR":
                            Genero result = new Genero();
                            if (passedId == string.Empty) result = (Genero)listData.Find(g => ((Genero)g).Descripcion == passedDesc);
                            else result = (Genero)listData.Find(g => ((Genero)g).Id == passedId);

                            if (result != null)
                            {
                                List<Genero> resultList = new List<Genero>();
                                resultList.Add(result);
                                myListView.ItemsSource = resultList;
                            }
                            break;
                        case "GUARDAR":
                            try
                            {
                                String q = $"INSERT INTO genero VALUES ('{passedId}', '{passedDesc}')";
                                using (SqlConnection conn = new SqlConn(_configuration).GetConnection())
                                {
                                    using (SqlCommand command = new SqlCommand(q, conn))
                                    {
                                        conn.Open();
                                        int rowsAffected = command.ExecuteNonQuery();
                                        if (rowsAffected > 0)
                                        {
                                            MessageBox.Show("Dato insertado con exito!","Exito!",MessageBoxButton.OK, MessageBoxImage.Information);
                                            UpdateList();
                                        }
                                    }
                                }

                            }
                            catch(Exception ex)
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
                                            MessageBox.Show("Dato actualizado con exito!", "Exito!", MessageBoxButton.OK, MessageBoxImage.Information);
                                            UpdateList();
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
                                String q = $"DELETE FROM genero WHERE id_genero = '{passedId}'";
                                using (SqlConnection conn = new SqlConn(_configuration).GetConnection())
                                {
                                    using (SqlCommand command = new SqlCommand(q, conn))
                                    {
                                        conn.Open();
                                        int rowsAffected = command.ExecuteNonQuery();
                                        if (rowsAffected > 0)
                                        {
                                            MessageBox.Show("Dato eliminado con exito!", "Exito!", MessageBoxButton.OK, MessageBoxImage.Information);
                                            UpdateList();
                                        }
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ha ocurrido un error al eliminar! {ex.Message}", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            break;
                        case "LIMPIAR":
                            IdBox.Text = string.Empty;
                            DescBox.Text = string.Empty;
                            IdBox.IsEnabled = true;
                            DisableButtons();
                            myListView.ItemsSource = ogData;
                            break;
                    }
                }
            }
        }

        private void SearchByTextBox(object sender, TextChangedEventArgs e)
        {
            TextBox IdTxtbox = FindVisualChild<TextBox>(sender as TextBox, "LoginText");
            if (IdTxtbox != null)
            {
                if (IdTxtbox.Text != string.Empty && ((TextBox)sender).Name == "IdBox")
                {
                    EnableButtons();
                }
                else if(((TextBox)sender).Name != "IdBox")
                {
                    Select.IsEnabled = true;
                }
                else
                {
                    DisableButtons();
                }
            }
        }

        public void EnableButtons()
        {
            Select.IsEnabled = true;
            Save.IsEnabled = true;
            Update.IsEnabled = true;
            Delete.IsEnabled = true;
            Clear.IsEnabled = true;
        }

        public void DisableButtons()
        {
            Select.IsEnabled = false;
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
                Debug.WriteLine(((Genero)item).Id + " " + ((Genero)item).Descripcion);
                IdBox.Text = ((Genero)item).Id;
                IdBox.IsEnabled = false; 
                DescBox.Text = ((Genero)item).Descripcion;
            }
        }
    }
}
