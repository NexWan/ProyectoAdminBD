using System.Windows;

namespace ProyectoAdminBD.MVVM.Model
{
    internal class ShoInfoMsg
    {
        public const string SUCCESS = "SUCCESS";
        public const string ERROR = "ERROR";
        public const string INFORMATION = "INFORMATION";
        public const string WARNING = "WARNING";


        public ShoInfoMsg(string typeInfo, string message) { 
            switch(typeInfo)
            {
                case "SUCCESS":
                    ShowSuccessMessage(message);
                    break;
                case "ERROR":
                    ShowErrorMessage(message);
                    break;
                case "INFORMATION":
                    ShowInformationMessage(message);
                    break;
                case "WARNING":
                    ShowWarningMessage(message);
                    break;
            }
        }

        public ShoInfoMsg() { }

        public void ShowSuccessMessage(string message)
        {
            MessageBox.Show(message, "EXITO!",MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowInformationMessage(string message) { 
            MessageBox.Show(message, "INFORMACION", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public void ShowWarningMessage(string message) {
            MessageBox.Show(message, "ADVETENCIA!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        public bool ShowConfirmDialog(string message)
        {
            MessageBoxResult res = MessageBox.Show(message,"Confirmacion",MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                return true;
            }
            return false;
        }
    }
}
