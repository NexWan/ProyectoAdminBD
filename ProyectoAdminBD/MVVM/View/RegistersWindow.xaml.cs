
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
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

        public List<object> listData = new List<object>();
        public RegistersWindow()
        {
            InitializeComponent();
            var appconfig = new LoadConfig();
            _configuration = appconfig.Configuration;
            Debug.WriteLine(DataHolder.Instance.currentQuery);
            holder = DataHolder.Instance;
            UpdateList();
            DisableButtons();
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
                    }
                }
            }
        }

        private void SearchByTextBox(object sender, TextChangedEventArgs e)
        {
            TextBox actualTb = FindVisualChild<TextBox>(sender as TextBox, "LoginText");
            if (actualTb != null)
            {
                if(actualTb.Text != string.Empty)
                {
                    EnableButtons();
                }
                else
                {
                    DisableButtons();
                }
            }
        }

        public void EnableButtons()
        {
            Save.IsEnabled = true;
            Update.IsEnabled = true;
            Delete.IsEnabled = true;
            Select.IsEnabled = true;
        }

        public void DisableButtons()
        {
            Save.IsEnabled = false;
            Update.IsEnabled = false;
            Delete.IsEnabled = false;
            Select.IsEnabled = false;
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
    }
}
