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
    /// Interaction logic for Login.xaml
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
            TextBlock watermarkTextBlock = FindVisualChild<TextBlock>(passwordBox, "PlaceholderTextBlock");

***REMOVED***

***REMOVED***
            ***REMOVED***
                if (FindVisualChild<PasswordBox>(passwordBox,"pwdBox").Password.Length == 0)
***REMOVED***
***REMOVED***
***REMOVED***
    ***REMOVED***
***REMOVED***

        private T FindVisualChild<T>(DependencyObject parent, string name) where T : FrameworkElement
        ***REMOVED***
***REMOVED***
            ***REMOVED***
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
***REMOVED***
                ***REMOVED***
***REMOVED***
        ***REMOVED***

                T childOfChild = FindVisualChild<T>(child, name);
***REMOVED***
                ***REMOVED***
***REMOVED***
        ***REMOVED***
    ***REMOVED***
***REMOVED***
***REMOVED***

***REMOVED***
        ***REMOVED***
            Debug.WriteLine("Hola");
            SqlConnection conn = new SqlConn(" "," ", "actas").GetConnection();
***REMOVED***
            String user = LoginText.Text;
            String pwd = FindVisualChild<PasswordBox>(MyPasswordBox, "pwdBox").Password;
            Debug.WriteLine($"***REMOVED***pwd***REMOVED*** ------- ***REMOVED***user***REMOVED***");
***REMOVED***

        String ConvertToUnsecureString(SecureString secureString)
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
