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
***REMOVED***
        ***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
            ***REMOVED***
***REMOVED***
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
            switch (receivedScene)
            ***REMOVED***
                case "LoginScene":
                    LoginWindow.Visibility = Visibility.Visible;
                    SignupWindow.Visibility = Visibility.Hidden;
                    Tittle.Text = "Log in";
                    break;
                case "SignupScene":
                    LoginWindow.Visibility = Visibility.Hidden;
                    SignupWindow.Visibility = Visibility.Visible;
                    Tittle.Text = "Sign up";

                    break;
                case "InvitadoScene":
                    MessageBox.Show("Soy la escena de invitado");
                    break;
    ***REMOVED***
***REMOVED***

***REMOVED***
        ***REMOVED***
***REMOVED***
            if (ellipse.Name == "close") ellipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCFF0000"));
            else if (ellipse.Name == "minimize") ellipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCFFFF00"));
            
***REMOVED***

***REMOVED***
        ***REMOVED***
***REMOVED***
            if (ellipse.Name == "close") ellipse.Fill = new SolidColorBrush(Colors.DarkRed);
            else if (ellipse.Name == "minimize") ellipse.Fill = new SolidColorBrush(Colors.Yellow);
***REMOVED***
***REMOVED***
***REMOVED***
