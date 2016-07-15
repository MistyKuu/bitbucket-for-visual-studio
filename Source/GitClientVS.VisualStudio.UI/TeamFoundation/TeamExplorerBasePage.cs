// <copyright file="TeamExplorerBasePage.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>

using System;
using Microsoft.TeamFoundation.Controls;

namespace GitClientVS.VisualStudio.UI.TeamFoundation
{
    /// <summary>
    /// Team Explorer page base class.
    /// </summary>
    public class TeamExplorerBasePage : TeamExplorerBase, ITeamExplorerPage
    {
        private string title;
        private bool isBusy;
        private object pageContent;

        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.title = value; 
                this.RaisePropertyChanged("Title");
            }
        }

        public object PageContent
        {
            get
            {
                return this.pageContent;
            }

            set
            {
                this.pageContent = value; 
                this.RaisePropertyChanged("PageContent");
            }
        }

        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            set
            {
                this.isBusy = value; 
                this.RaisePropertyChanged("IsBusy");
            }
        }

        public virtual void Initialize(object sender, PageInitializeEventArgs e)
        {
            this.ServiceProvider = e.ServiceProvider;
        }

        public virtual void Loaded(object sender, PageLoadedEventArgs e)
        {
        }

        public virtual void SaveContext(object sender, PageSaveContextEventArgs e)
        {
        }

        public virtual void Refresh()
        {
        }

        public virtual void Cancel()
        {
        }

        public virtual object GetExtensibilityService(Type serviceType)
        {
            return null;
        }
    }
}
