// <copyright file="TeamExplorerBase.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Controls;

namespace GitClientVS.VisualStudio.UI.TeamFoundation
{
    /// <summary>
    /// Team Explorer extension common base class.
    /// </summary>
    public class TeamExplorerBase : IDisposable, INotifyPropertyChanged
    {
        private bool contextSubscribed;
        private IServiceProvider serviceProvider;

        public static readonly Guid TeamExplorerConnectionsSectionId = new Guid("ef6a7a99-f01f-4c91-ad31-183c1354dd97");

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Get/set the service provider.
        /// </summary>
        public IServiceProvider ServiceProvider
        {
            get
            {
                return this.serviceProvider;
            }

            set
            {
                // Unsubscribe from Team Foundation context changes
                if (this.serviceProvider != null)
                {
                    this.UnsubscribeContextChanges();
                }

                this.serviceProvider = value;

                // Subscribe to Team Foundation context changes
                if (this.serviceProvider != null)
                {
                    this.SubscribeContextChanges();
                }
            }
        }

        protected ITeamFoundationContext CurrentContext
        {
            get
            {
                ITeamFoundationContextManager tfcontextManager = this.GetService<ITeamFoundationContextManager>();
                return tfcontextManager != null ? tfcontextManager.CurrentContext : null;
            }
        }

        public T GetService<T>()
        {
            if (this.ServiceProvider != null)
            {
                return (T)this.ServiceProvider.GetService(typeof(T));
            }

            return default(T);
        }

        public virtual void Dispose()
        {
            this.UnsubscribeContextChanges();
        }

        public Guid ShowNotification(string message, NotificationType type)
        {
            ITeamExplorer teamExplorer = this.GetService<ITeamExplorer>();
            if (teamExplorer != null)
            {
                Guid guid = Guid.NewGuid();
                teamExplorer.ShowNotification(message, type, NotificationFlags.None, null, guid);
                return guid;
            }

            return Guid.Empty;
        }

        protected void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void SubscribeContextChanges()
        {
            if (this.ServiceProvider == null || this.contextSubscribed)
            {
                return;
            }

            ITeamFoundationContextManager tfcontextManager = this.GetService<ITeamFoundationContextManager>();
            if (tfcontextManager != null)
            {
                tfcontextManager.ContextChanged += this.ContextChanged;
                this.contextSubscribed = true;
            }
        }

        protected void UnsubscribeContextChanges()
        {
            if (this.ServiceProvider == null || !this.contextSubscribed)
            {
                return;
            }

            ITeamFoundationContextManager tfcontextManager = this.GetService<ITeamFoundationContextManager>();
            if (tfcontextManager != null)
            {
                tfcontextManager.ContextChanged -= this.ContextChanged;
            }
        }

        protected virtual void ContextChanged(object sender, ContextChangedEventArgs e)
        {
            
        }
    }
}
