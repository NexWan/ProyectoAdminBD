using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProyectoAdminBD.MVVM.Model
{
    internal class ShoInfoMsg
    {
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
    }
}
