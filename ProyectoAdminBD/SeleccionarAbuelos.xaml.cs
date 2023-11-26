﻿using ProyectoAdminBD.MVVM.Model;
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
            return new QueryExecutor().ExecuteQuery(
                "SELECT * FROM Abuelos",
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
            this.Close();
        }
    }
}
