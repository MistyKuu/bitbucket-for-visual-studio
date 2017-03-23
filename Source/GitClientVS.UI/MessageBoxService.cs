using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GitClientVS.Contracts.Interfaces;
using GitClientVS.Contracts.Interfaces.Services;

namespace GitClientVS.UI
{
    [Export(typeof(IMessageBoxService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MessageBoxService : IMessageBoxService
    {
        public bool ShowDialogYesNo(string title, string message)
        {
            var btnMessageBox = MessageBoxButton.YesNo;
            var icnMessageBox = MessageBoxImage.Warning;

            var result = MessageBox.Show(message, title, btnMessageBox, icnMessageBox);
            return result == MessageBoxResult.Yes;
        }
    }
}
