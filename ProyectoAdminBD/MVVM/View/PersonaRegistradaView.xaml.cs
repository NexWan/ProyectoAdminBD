using Microsoft.Extensions.Configuration;
using ProyectoAdminBD.Connection;
using ProyectoAdminBD.MVVM.Model;
using ProyectoAdminBD.Theme;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace ProyectoAdminBD.MVVM.View
{
    /// <summary>
    /// Interaction logic for PersonaRegistradaView.xaml
    /// </summary>
    public partial class PersonaRegistradaView : UserControl
    {
        private IConfiguration _configuration;
        private SqlDataReader reader;
        private SqlCommand cmd;
        private bool _clear = true, wasTriggered = false, triggerSearch = true;
        private TextBox[] TextBoxes;
        private TextBox currentAwelo;

        // Lists
        private List<ElementosRegistro> elementosRegistros;
        private List<Municipio> municipios;
        private List<Presentado> presentados;
        private List<PersonaRegistrada> listData;
        private List<Padres> padres;
        private List<Genero> generos;

        // Other variables
        private DataHolder holder;
        private string? curp, municipio, genero, presentado, oficialia, nombre, apPaterno, apMaterno,
            curpPadre, curpMadre, abueloPaterno, abuelaPaterna, abueloMaterno, abuelaMaterna,
            crip, numActa, numLibro;
        private DateTime? fechaNac, fechaReg;
        private TimeSpan? horaNac;


        public PersonaRegistradaView()
        {
            InitializeComponent();
            var appConfig = new LoadConfig();
            _configuration = appConfig.Configuration;
            holder = DataHolder.Instance;
            MUNICIPIO.ItemsSource = GetMunicipios();
            GENERO.ItemsSource = GetGeneros();
            PRESENTADO.ItemsSource = GetPresentados();
            OFICIALIA.ItemsSource = GetElementosRegistros();
            FECHANAC.DisplayDate = DateTime.Now;
            FECHAREG.DisplayDate = DateTime.Now;
            UpdateList();
            DisableButtons();
            TextBoxes = new TextBox[] { CURP, NOMBRE, APPATERNO, APMATERNO, CURPPADRE, CURPMADRE, NUMLIBRO, NUMACTA };
            for (int i = 0; i < TextBoxes.Length; i++)
                TextBoxes[i].TextChanged += SearchByTextBox;
            ABUELOP.TextChanged += SelectAbuelos;
            ABUELOM.TextChanged += SelectAbuelos;
            ABUELAP.TextChanged += SelectAbuelos;
            ABUELAM.TextChanged += SelectAbuelos;
        }

        private void UpdateList()
        {
            List<PersonaRegistrada> list = new QueryExecutor().ExecuteQuery(
                "SELECT PR.*, CONCAT(P.nombres,' ', P.ap_paterno,' ', p.ap_materno, ' Edad: ', p.edad) as PAPA, " +
                "CONCAT(M.nombres,' ', M.ap_paterno,' ', M.ap_materno, ' Edad: ', M.edad) as MAMA " +
                "FROM PERSONA_REGISTRADA pr, padre P, madre M " +
                "WHERE pr.madre_Curp = m.Curp AND pr.padre_curp = p.curp",
                row => new PersonaRegistrada
                {
                    Curp = row["curp"].ToString(),
                    Municipio = row["idMunicipio"].ToString(),
                    Genero = row["id_Genero"].ToString(),
                    Presentado = row["idPRESENTADO"].ToString(),
                    No_oficialia = Convert.ToInt32(row["NO_OFICIALIA"]),
                    Nombre = row["NOMBRES"].ToString(),
                    ApPaterno = row["APELLIDO_PATERNO"].ToString(),
                    ApMaterno = row["APELLIDO_MATERNO"].ToString(),
                    FechaNac = (DateTime)row["FECHA_NACIMIENTO"],
                    HoraNac = (TimeSpan)row["HORA_NAC"],
                    Crip = row["CRIP"].ToString(),
                    No_acta = Convert.ToInt32(row["NO_ACTA"]),
                    No_libro = Convert.ToInt32(row["NO_LIBRO"]),
                    FechaReg = (DateTime)row["FECHA_REGISTRO"],
                    CurpPadre = row["padre_curp"].ToString(),
                    CurpMadre = row["madre_curp"].ToString(),
                    Padre = row["PAPA"].ToString(),
                    Madre = row["MAMA"].ToString()
                });
            List<PersonaRegistrada> temp = list;
            List<AbueloPersona> x = new QueryExecutor().ExecuteQuery(
                "SELECT PA.*, A.id_genero, CONCAT(A.nomAbuelo, ' ', A.apPaterno, ' ', A.apMaterno) as Nombre FROM personaAbuelos PA, Abuelos A WHERE PA.id_abuelo = A.id_Abuelo",
                row => new AbueloPersona
                {
                    Curp = row["curp"].ToString(),
                    Id_abuelo = Convert.ToInt32(row["id_abuelo"]),
                    Parentesco = row["parentesco"].ToString(),
                    Genero = row["id_genero"].ToString(),
                    Nombre = row["Nombre"].ToString()
                });
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < x.Count; j++)
                    if (temp[i].Curp == x[j].Curp)
                    {
                        if (x[j].Parentesco.Contains("Pate"))
                        {
                            if (x[j].Genero == "F") temp[i].AbuelaPaternaId = Convert.ToInt32(x[j].Id_abuelo);
                            else temp[i].AbueloPaternoId = Convert.ToInt32(x[j].Id_abuelo);
                            temp[i].AbueloPaterno += x[j].Nombre + " || ";
                        }
                        else
                        {
                            if (x[j].Genero == "F") temp[i].AbuelaMaternaId = Convert.ToInt32(x[j].Id_abuelo);
                            else temp[i].AbueloMaternoId = Convert.ToInt32(x[j].Id_abuelo);
                            temp[i].AbueloMaterno += x[j].Nombre + " || ";
                        }
                    }
                temp[i].AbueloPaterno = temp[i].AbueloPaterno.Substring(0, temp[i].AbueloPaterno.Length - 3);
                temp[i].AbueloMaterno = temp[i].AbueloMaterno.Substring(0, temp[i].AbueloMaterno.Length - 3);
            }
            ActaTable.ItemsSource = temp;
            listData = list;
        }

        private List<Municipio> GetMunicipios()
        {
            List<Municipio> temp = new QueryExecutor().ExecuteQuery(
                "SELECT * FROM municipio",
                row => new Municipio
                {
                    _id = row["id_municipio"].ToString(),
                    _entidad = row["id_entidad"].ToString(),
                    _name = row["nombre"].ToString()
                }
                );
            municipios = temp;
            return municipios;
        }

        private List<Genero> GetGeneros()
        {
            List<Genero> temp =  new QueryExecutor().ExecuteQuery(
                "SELECT * FROM genero",
                row => new Genero
                {
                    Id = row["Id_GENERO"].ToString(),
                    Descripcion = row["Descripcion"].ToString()
                });
            generos = temp;
            return generos;
        }

        private async void SelectAbuelos(object sender, TextChangedEventArgs e)
        {
            Debug.WriteLine("ola");
            TextBox curr = (TextBox)sender;
            string txt = curr.Text;
            string type = curr.Name;
            Debug.WriteLine(txt);
            if (txt != "Click para seleccionar" && _clear && AreAllTextBoxesEmptyExceptOne(curr, TextBoxes))
            {
                ActaTable.ItemsSource = await GetPersonaRegistradasAsync(txt, type);
                UpdateList();
            }
            if (ActaTable.Items.Count == 1)
                ActaTable.SelectedIndex = 0;
        }

        private List<Presentado> GetPresentados()
        {
            List<Presentado> temp = new QueryExecutor().ExecuteQuery(
                "SELECT * FROM presentado",
                row => new Presentado
                {
                    _id = row["id_PRESENTADO"].ToString(),
                    _descripcion = row["Descripcion"].ToString()
                });
            presentados = temp;
            return presentados;
        }

        private List<ElementosRegistro> GetElementosRegistros()
        {
            List<ElementosRegistro> temp =  new QueryExecutor().ExecuteQuery(
                "SELECT * FROM elementos_registro",
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
            elementosRegistros = temp;
            return elementosRegistros;
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

        private void ExecOperation(object sender, RoutedEventArgs e)
        {
            curp = CURP.Text;
            municipio = MUNICIPIO?.SelectedValue?.ToString();
            genero = GENERO?.SelectedValue?.ToString();
            presentado = PRESENTADO?.SelectedValue?.ToString();
            oficialia = OFICIALIA?.SelectedValue?.ToString();
            nombre = NOMBRE.Text;
            apPaterno = APPATERNO.Text;
            apMaterno = APMATERNO.Text;
            curpPadre = CURPPADRE.Text;
            curpMadre = CURPMADRE.Text;
            abueloPaterno = ABUELOP.Text;
            abuelaPaterna = ABUELAP.Text;
            abueloMaterno = ABUELOM.Text;
            abuelaMaterna = ABUELAM.Text;
            fechaNac = FECHANAC.SelectedDate;
            horaNac = TimeSpan.Parse(HORANAC.Text);
            crip = CRIP.Text;
            numActa = NUMACTA.Text;
            numLibro = NUMLIBRO.Text;
            fechaReg = FECHAREG.SelectedDate;
            string context = (sender as Button).Content.ToString().ToUpper();
            using (SqlConnection conn = new SqlConn(_configuration).GetConnection())
            {
                conn.Open();
                string q;
                Debug.WriteLine(context);
                if (context == "LIMPIAR")
                {
                    Clean();
                    return;
                }
                string[] checkParentales = {$"SELECT * FROM padre WHERE curp = '{curpPadre}'", $"SELECT * FROM madre WHERE curp = '{curpMadre}'",
                                $"SELECT * FROM abuelos WHERE id_abuelo = {abueloMaterno}",$"SELECT * FROM abuelos WHERE id_abuelo = {abuelaMaterna}",
                                $"SELECT * FROM abuelos WHERE id_abuelo = {abueloPaterno}", $"SELECT * FROM abuelos WHERE id_abuelo = {abuelaPaterna}"};
                for (int i = 0; i < checkParentales.Length; i++)
                {
                    if (!VerifyExistingValue(conn, checkParentales[i]))
                    {
                        new ShoInfoMsg(ShoInfoMsg.ERROR, "Verifique el valor de curp padre, madre o los abuelos, no se ha podido encontrar en la base de datos alguno!");
                        return;
                    }
                }
                switch (context)
                {
                    case "GUARDAR":
                        GuardarQuery(conn);
                        UpdateList();
                        Clean();
                        break;
                    case "MODIFICAR":
                        if (string.IsNullOrEmpty(curp) || string.IsNullOrEmpty(municipio) ||
                            string.IsNullOrEmpty(genero) || string.IsNullOrEmpty(presentado) ||
                            string.IsNullOrEmpty(oficialia))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "El campo de curp, municipio, genero, presentado y oficialia no puede estar vacios!");
                            return;
                        }
                        if (string.IsNullOrEmpty(curpPadre) || string.IsNullOrEmpty(curpMadre) || string.IsNullOrEmpty(abuelaMaterna) || string.IsNullOrEmpty(abuelaPaterna)
                            ||string.IsNullOrEmpty(abueloMaterno) || string.IsNullOrEmpty(abueloPaterno))
                            new ShoInfoMsg(ShoInfoMsg.WARNING, "Tome a consideracion que los campos padre, madre o abuelos no pueden ser modificados. Pero" +
                                " los campos necesitan estar marcados para hacer las modificaciones");

                        if (string.IsNullOrEmpty(nombre) ||
                            string.IsNullOrEmpty(apPaterno) || string.IsNullOrEmpty(apMaterno) ||
                            fechaNac == null || horaNac == null ||
                            string.IsNullOrEmpty(crip) || string.IsNullOrEmpty(numActa) ||
                            string.IsNullOrEmpty(numLibro) || fechaReg == null)
                        {
                            bool conf = new ShoInfoMsg().ShowConfirmDialog("1 o mas valores estan vacios, estos seran sobreescritos a nada, deseas continuar?");
                            if (!conf)
                                return;
                        }
                        q = $"SELECT * FROM persona_registrada WHERE curp = '{curp}'";
                        if(!VerifyExistingValue(conn, q))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "Dato no encontrado en la BD!, intente de nuevo");
                            return;
                        }
                        q = $"UPDATE persona_registrada SET nombres = '{nombre}', apellido_materno = '{apMaterno}', apellido_paterno = '{apPaterno}'" +
                            $", id_genero = '{genero}'";
                        if (ExecQuery(conn, q))                    
                            new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato actualizado con exito!");
                        UpdateList();
                        Clean();
                        break;
                    case "ELIMINAR":
                        new ShoInfoMsg(ShoInfoMsg.WARNING, "WIP");
                        /*q = $"SELECT * FROM persona_registrada WHERE curp = {curp}";
                        if (!VerifyExistingValue(conn, q))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "Dato no encontrado en la BD!, intente de nuevo");
                            return;
                        }
                        q = $"DELETE FROM persona_registrada WHERE curp = '{curp}'";
                        if (ExecQuery(conn, q))
                            new ShoInfoMsg(ShoInfoMsg.SUCCESS, "dato eliminado con exito!");*/
                        break;
                }
            }
        }
        private void GuardarQuery(SqlConnection conn)
        {
            string q;
            if (!string.IsNullOrEmpty(curp) || string.IsNullOrEmpty(crip))
                new ShoInfoMsg(ShoInfoMsg.WARNING, "Se ha detectado que el campo curp o crip contiene texto, este sera sobreescrito al generado automaticamente");

            if (string.IsNullOrEmpty(municipio) ||
                string.IsNullOrEmpty(genero) || string.IsNullOrEmpty(presentado) ||
                string.IsNullOrEmpty(oficialia) || string.IsNullOrEmpty(nombre) ||
                string.IsNullOrEmpty(apPaterno) || string.IsNullOrEmpty(apMaterno) ||
                string.IsNullOrEmpty(curpPadre) || string.IsNullOrEmpty(curpMadre) ||
                fechaNac == null || horaNac == null || string.IsNullOrEmpty(numActa) ||
                string.IsNullOrEmpty(numLibro) || fechaReg == null)
            {
                new ShoInfoMsg(ShoInfoMsg.ERROR, "1 o mas campos vacios, por favor intente de nuevo!");
                return;
            }
            string nuCurp = GenerarCurp(apPaterno, apMaterno, nombre, fechaNac, genero, municipio);
            Debug.WriteLine(nuCurp);
            q = $"SELECT * FROM persona_registrada where curp = '{nuCurp}'";
            if (VerifyExistingValue(conn, q))
            {
                new ShoInfoMsg(ShoInfoMsg.ERROR, "Dato ya existe en la BD! intente de nuevo");
                return;
            }
            Random r = new Random();
            crip = r.NextInt64(1, 10).ToString();
            q = $"INSERT INTO persona_registrada VALUES (" +
                    $"'{nombre}', " +
                    $"'{apMaterno}', " +
                    $"'{apPaterno}', " +
                    $"'{fechaNac?.ToString("yyyy-MM-dd")}', " + // Format the date for SQL
                    $"'{horaNac?.ToString()}', " + // Use the default TimeSpan.ToString() format
                    $"'{crip}', " +
                    $"'{nuCurp}', " +
                    $"'{curpMadre}', " +
                    $"'{curpPadre}', " +
                    $"'{municipio}', " +
                    $"'{genero}', " +
                    $"'{presentado}', " +
                    $"{oficialia}," +
                    $"{numActa}, " +
                    $"{numLibro}," +
                    $"'{fechaReg?.ToString("yyyy-MM-dd")}')";
            if (ExecQuery(conn, q))
            {
                string[] inserts = { $"INSERT INTO personaAbuelos VALUES('{nuCurp}','{abueloPaterno}','Paterno')", $"INSERT INTO personaAbuelos VALUES('{nuCurp}','{abuelaPaterna}','Paterno')",
                            $"INSERT INTO personaAbuelos VALUES('{nuCurp}','{abueloMaterno}','Materno')", $"INSERT INTO personaAbuelos VALUES('{nuCurp}','{abuelaMaterna}','Materna')"};
                for (int i = 0; i < inserts.Length; i++)
                {
                    if (ExecQuery(conn, inserts[i]))
                    {
                        if (i == inserts.Length - 1)
                            new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato insertado con exito!");
                    }
                    else
                    {
                        ManageDuplicatesError(nuCurp, conn);
                        return;
                    }
                }
            }
        }
        private string GenerarCurp(string apPaterno, string apMaterno, string nombre, DateTime? fechaNac, string genero, string municipio)
        {
            Random rnd = new Random();
            string iniciales = $"{apPaterno[0]}";

            for (int i = 1; i < apPaterno.Length; i++)
            {
                if ("AEIOU".Contains(apPaterno[i], StringComparison.OrdinalIgnoreCase))
                {
                    iniciales += char.ToUpper(apPaterno[i]);
                    break;
                }
            }

            string estado = municipios.FirstOrDefault(m => m._id == municipio)._entidad;

            iniciales += $"{apMaterno[0]}{nombre[0]}".ToUpper();

            // Obtener los dígitos del año de nacimiento
            string anioNac = (fechaNac?.Year % 100)?.ToString("D2");

            // Obtener el carácter del mes de nacimiento (en el caso de enero, se utiliza "0")
            string mesNac = fechaNac?.Month == 1 ? "0" : fechaNac?.Month.ToString("D2");

            // Obtener los dígitos del día de nacimiento
            string diaNac = fechaNac?.Day.ToString("D2");
            string consonantes = $"{ObtenerConsonante(apPaterno.Substring(1))}{ObtenerConsonante(apMaterno)}{ObtenerConsonante(nombre)}";

            // Construir el string de la CURP
            return $"{iniciales}{anioNac}{mesNac}{diaNac}{genero[0]}{estado.Substring(0, 2)}{consonantes}{iniciales[0]}{rnd.NextInt64(10, 99)}";
        }

        private char ObtenerConsonante(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (!EsVocal(s[i]))
                {
                    return char.ToUpper(s[i]);
                }
            }
            return '\0';
        }

        bool EsVocal(char letra)
        {
            return "AEIOUaeiou".Contains(letra);
        }

        private void Clean()
        {
            triggerSearch = false;
            CURP.IsEnabled = true;
            CURP.Text = string.Empty;
            MUNICIPIO.SelectedItem = null;
            MUNICIPIO.SelectedValue = null;
            GENERO.SelectedValue = null;
            GENERO.SelectedItem = null;
            PRESENTADO.SelectedItem = null;
            PRESENTADO.SelectedValue = null;
            OFICIALIA.SelectedValue = null;
            OFICIALIA.SelectedItem = null;
            NOMBRE.Text  = string.Empty;
            APPATERNO.Text  = string.Empty;
            APMATERNO.Text  = string.Empty;
            CURPPADRE.Text  = string.Empty;
            CURPMADRE.Text  = string.Empty;
            ABUELOM.Text = "Click para seleccionar";
            ABUELAM.Text = "Click para seleccionar";
            ABUELOP.Text = "Click para seleccionar";
            ABUELAP.Text = "Click para seleccionar";
            FECHANAC.SelectedDate = null;
            HORANAC.Text = "00:00:00";
            CRIP.Text  = string.Empty;
            NUMACTA.Text  = string.Empty;
            NUMLIBRO.Text  = string.Empty;
            FECHAREG.SelectedDate = null;
            holder.selectedAbueloId = string.Empty;
            holder.abuelos.Clear();
            wasTriggered = false;
            _clear = true;
            UpdateList();
            DisableButtons();
            triggerSearch = true;
        }

        private async void SearchByTextBox(object sender, TextChangedEventArgs e)
        {
            TextBox curr = sender as TextBox;
            string type = curr.Name;
            string txt = curr.Text;
            if (triggerSearch)
            {
                Debug.WriteLine(type);
                if (string.IsNullOrEmpty(txt) && _clear && AreAllTextBoxesEmptyExceptOne(curr,TextBoxes))
                    UpdateList();
                else if (txt.Length > 0 && _clear && AreAllTextBoxesEmptyExceptOne(curr,TextBoxes))
                {
                    EnableButtons();
                    ActaTable.ItemsSource = await GetPersonaRegistradasAsync(txt, type);
                }
                if (ActaTable.Items.Count == 1)
                {
                    ActaTable.SelectedIndex = 0;
                    wasTriggered = true;
                }
            }
        }

        private bool AreAllTextBoxesEmptyExceptOne(TextBox exceptTextBox, params TextBox[] textBoxes)
        {
            return textBoxes.Except(new[] { exceptTextBox }).All(tb => string.IsNullOrEmpty(tb.Text));
        }

        private async Task<List<PersonaRegistrada>> GetPersonaRegistradasAsync(string value, string type)
        {
            value = value.ToUpper();
            List<PersonaRegistrada> temp = new List<PersonaRegistrada>();
            foreach(PersonaRegistrada obj in listData)
            {
                if(type == "CURP" && obj.Curp.ToUpper().Contains(value))
                    temp.Add(obj);
                if (type == "NOMBRE" && obj.Nombre.ToUpper().Contains(value))
                    temp.Add(obj);
                if(type == "APPATERNO" && obj.ApPaterno.ToUpper().Contains(value))
                    temp.Add(obj);
                if(type == "APMATERNO" && obj.ApMaterno.ToUpper().Contains(value))
                    temp.Add(obj);
                if(type == "ABUELOP" && obj.AbueloPaternoId == Convert.ToInt32(value))
                    temp.Add(obj);
                if(type == "ABUELOM" && obj.AbueloMaternoId == Convert.ToInt32(value))
                    temp.Add(obj);
                if (type == "ABUELAP" && obj.AbuelaPaternaId == Convert.ToInt32(value))
                    temp.Add(obj);
                if (type == "ABUELAM" && obj.AbuelaMaternaId == Convert.ToInt32(value))
                    temp.Add(obj);
                if(type == "CURPPADRE" && obj.CurpPadre.ToUpper().Contains(value))
                    temp.Add(obj);
                if(type == "CURPMADRE" && obj.CurpMadre.ToUpper().Contains(value))
                    temp.Add(obj);
            }
            return temp;

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

        private void ManageDuplicatesError(string curp, SqlConnection conn)
        {
            string q = $"DELETE FROM personaAbuelos where curp = '{curp}'";
            string q2 = $"DELETE FROM persona_registrada WHERE curp = '{curp}'";
            if (ExecQuery(conn, q))
                if (ExecQuery(conn, q2))
                    new ShoInfoMsg(ShoInfoMsg.ERROR, "Ocurrio un error a la hora de insertar, por favor intente de nuevo");
        }

        private void SelectManager(object sender, SelectionChangedEventArgs e)
        {
            PersonaRegistrada item = ((PersonaRegistrada)(sender as ListView).SelectedItem);
            if(item != null)
            {
                CURP.Text = item.Curp;
                MUNICIPIO.SelectedItem = municipios.FirstOrDefault(m => m._id == item.Municipio);
                GENERO.SelectedItem = generos.FirstOrDefault(g => g.Id == item.Genero);
                PRESENTADO.SelectedItem = presentados.FirstOrDefault(p => p._id == item.Presentado);
                OFICIALIA.SelectedItem = elementosRegistros.FirstOrDefault(e => e._no_oficialia == item.No_oficialia);
                NOMBRE.Text = item.Nombre;
                APPATERNO.Text = item.ApPaterno;
                APMATERNO.Text = item.ApMaterno;
                CURPPADRE.Text = item.CurpPadre;
                CURPMADRE.Text = item.CurpMadre;
                ABUELOP.Text = item.AbueloPaternoId.ToString();
                ABUELAP.Text = item.AbuelaPaternaId.ToString();
                ABUELOM.Text = item.AbueloMaternoId.ToString();
                ABUELAM.Text = item.AbuelaMaternaId.ToString();
                HORANAC.Text = item.HoraNac.ToString();
                CRIP.Text = item.Crip;
                NUMACTA.Text = item.No_acta.ToString();
                NUMLIBRO.Text = item.No_libro.ToString();
                if(!wasTriggered)
                    CURP.IsEnabled = false;
            }
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

        private void TextBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            currentAwelo = sender as TextBox;
            SeleccionarAbuelos seleccionarAbuelos = new SeleccionarAbuelos();
            seleccionarAbuelos.Closed += SeleccionarAbuelos_Closed;
            seleccionarAbuelos.Show();
        }

        private void SeleccionarAbuelos_Closed(object? sender, EventArgs e)
        {
            currentAwelo.Text = holder.selectedAbueloId;
        }
        private void timeMaskedTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string currentText = HORANAC.Text;
            if (int.TryParse(e.Text, out int enteredNumber))
            {
                if (IsValidTime(currentText + e.Text))
                {
                    e.Handled = true; 
                }
            }
            else
            {
                e.Handled = true; 
            }
        }

        private bool IsValidTime(string time)
        {
            // You may need to implement your own logic based on your specific requirements
            // Here's a simple example assuming the time is in the format "hh:mm:ss"
            if (TimeSpan.TryParseExact(time, "hh\\:mm\\:ss", CultureInfo.InvariantCulture, out TimeSpan result))
            {
                return result.Minutes <= 59;
            }
            return false;
        }

        private void VerifyNumeric(object sender, TextCompositionEventArgs e)
        {
            if (!IsNumeric(e.Text))
            {
                e.Handled = true; // Prevent non-numeric characters from being added
            }
        }
        private bool IsNumeric(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsDigit(c))
                {
                    return false; // If any character is not a digit, return false
                }
            }
            return true; // If all characters are digits, return true
        }

    }
}
