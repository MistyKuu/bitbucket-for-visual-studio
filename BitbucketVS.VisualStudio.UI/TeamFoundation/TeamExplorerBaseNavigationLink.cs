// <copyright file="TeamExplorerBaseNavigationLink.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>

using System;
using Microsoft.TeamFoundation.Controls;

namespace BitbucketVS.VisualStudio.UI.TeamFoundation
{
    /// <summary>
    /// Team Explorer base navigation link class.
    /// </summary>
    public class TeamExplorerBaseNavigationLink : TeamExplorerBase, ITeamExplorerNavigationLink
    {
        private bool isEnabled = true;
        private string text;
        private bool isVisible = true;

        public TeamExplorerBaseNavigationLink(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.text = value; 
                this.RaisePropertyChanged("Text");
            }
        }

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }

            set
            {
                this.isEnabled = value; 
                this.RaisePropertyChanged("IsEnabled");
            }
        }

        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }

            set
            {
                this.isVisible = value; 
                this.RaisePropertyChanged("IsVisible");
            }
        }

        public virtual void Invalidate()
        {
        }

        public virtual void Execute()
        {
        }
    }
}
