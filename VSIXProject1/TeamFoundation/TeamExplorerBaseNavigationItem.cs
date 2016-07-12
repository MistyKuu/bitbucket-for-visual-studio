// <copyright file="TeamExplorerBaseNavigationItem.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>

using System;
using Microsoft.TeamFoundation.Controls;

namespace BitbucketVS.VisualStudio.UI.TeamFoundation
{
    /// <summary>
    /// Team Explorer base navigation item class.
    /// </summary>
    public class TeamExplorerBaseNavigationItem : TeamExplorerBase, ITeamExplorerNavigationItem
    {
        private bool isVisible = true;
        private string text;
        private System.Drawing.Image image;

        public TeamExplorerBaseNavigationItem(IServiceProvider serviceProvider)
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

        public System.Drawing.Image Image
        {
            get
            {
                return this.image;
            }

            set
            {
                this.image = value;
                this.RaisePropertyChanged("Image");
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
