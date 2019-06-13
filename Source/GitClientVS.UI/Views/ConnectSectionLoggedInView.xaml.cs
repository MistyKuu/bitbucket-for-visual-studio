﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;

namespace GitClientVS.UI.Views
{
    public partial class ConnectSectionLoggedInView : UserControl
    {
        public ConnectSectionLoggedInView()
        {
            InitializeComponent();
        }



        private void ChangeUserBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ChangeUserBtn.ContextMenu.PlacementTarget = ChangeUserBtn;
            ChangeUserBtn.ContextMenu.IsOpen = true;
        }
    }
}
