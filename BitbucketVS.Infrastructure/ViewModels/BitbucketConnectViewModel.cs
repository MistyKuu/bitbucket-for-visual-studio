using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitBucketVs.Contracts.Interfaces;
using ReactiveUI;
using System.Reactive.Linq;
using BitBucketVs.Contracts.Interfaces.ViewModels;
using BitBucketVs.Contracts.Interfaces.Views;

namespace BitbucketVS.Infrastructure.ViewModels
{
    [Export(typeof(IBitbucketConnectViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class BitbucketConnectViewModel : ViewModelBase, IBitbucketConnectViewModel
    {
        public BitbucketConnectViewModel()
        {
            this.WhenAnyValue(x => x.Message).Subscribe(x =>
            {
                MessageB = Message + " Hej";
            });
            Message = "Bucket";
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set { this.RaiseAndSetIfChanged(ref _message, value); }
        }

        public string MessageB { get; set; }
    }
}
