***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***

***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
    ***REMOVED***

***REMOVED***
        ***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***


***REMOVED***
        ***REMOVED***
***REMOVED***
            ***REMOVED***
***REMOVED***
    ***REMOVED***
***REMOVED***

***REMOVED***
        ***REMOVED***
***REMOVED***
            ***REMOVED***
***REMOVED***
    ***REMOVED***catch (Exception ex)
            ***REMOVED***
***REMOVED***
    ***REMOVED***

***REMOVED***

***REMOVED***
        ***REMOVED***
***REMOVED***
            ***REMOVED***
***REMOVED***
    ***REMOVED***
***REMOVED***
            ***REMOVED***
***REMOVED***
    ***REMOVED***

***REMOVED***

***REMOVED***
        ***REMOVED***
***REMOVED***

***REMOVED***
***REMOVED***

***REMOVED***

***REMOVED***
            ***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
    ***REMOVED***
***REMOVED***

***REMOVED***
        ***REMOVED***
***REMOVED***
            ***REMOVED***
***REMOVED***
***REMOVED***
                ***REMOVED***
***REMOVED***
        ***REMOVED***

***REMOVED***
***REMOVED***
                ***REMOVED***
***REMOVED***
        ***REMOVED***
    ***REMOVED***
***REMOVED***
***REMOVED***

        //Logica detras del boton de login
        private async void ClickLogin(object sender, RoutedEventArgs e)
        ***REMOVED***
            using (HttpClient client = new HttpClient())
            ***REMOVED***
                string response = await client.GetStringAsync("https://api64.ipify.org?format=json");
                Console.WriteLine(response);
    ***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
            ***REMOVED***
                sqlObject = new SqlConn("bd_actas");
***REMOVED***
***REMOVED***
    ***REMOVED***catch(Exception ex)
            ***REMOVED***
***REMOVED***
***REMOVED***
    ***REMOVED***
            
***REMOVED***
***REMOVED***
***REMOVED***
            MatchCollection matchCollection = regex.Matches(pwd);

            if(matchCollection.Count > 0)
            ***REMOVED***
                MessageBox.Show("Contraseña invalida, has puesto caracteres invalidos");
***REMOVED***
    ***REMOVED***
            String query = $"SELECT * FROM empleados WHERE id_empleado=***REMOVED***user***REMOVED*** AND clave= '***REMOVED***pwd***REMOVED***'";
            SqlCommand? cmd = conn?.CreateCommand();
***REMOVED***
            ***REMOVED***
                cmd.CommandText = query;
                SqlDataReader? reader = cmd.ExecuteReader();
                if (reader.Read())  //Si resulta cualquier valor significa que es verdadero
                    MessageBox.Show($"Usuario: ***REMOVED***user***REMOVED*** \n Contraseña: ***REMOVED***pwd***REMOVED*** \n Contraseña valida");
***REMOVED*** MessageBox.Show("Usuario y/o contraseña invalida");
    ***REMOVED***
            catch(Exception ex)
            ***REMOVED***
                MessageBox.Show("Ocurrio un error! Comprueba el usuario o contraseña");
    ***REMOVED***
***REMOVED***

        //Este metodo sirve para poder convertir un string seguro a un string normal
***REMOVED***
        ***REMOVED***
***REMOVED***
***REMOVED***
            ***REMOVED***
***REMOVED***
***REMOVED***
    ***REMOVED***
***REMOVED***
            ***REMOVED***
***REMOVED***
    ***REMOVED***
***REMOVED***

***REMOVED***
        ***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
            ***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***

    ***REMOVED***
    ***REMOVED***
            
            
***REMOVED***

***REMOVED***
        ***REMOVED***
***REMOVED***
***REMOVED***


***REMOVED***

***REMOVED***
        ***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
