using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitBucketVs.Contracts.Interfaces;
using ReactiveUI;
using System.Reactive.Linq;

namespace BitbucketVS.Infrastructure.ViewModels
{
    public class MainViewModel : ReactiveObject, IMainViewModel
    {
        public MainViewModel()
        {
            this.WhenAnyValue(x => x.Message).Subscribe(x =>
            {
                MessageB = Message + " Hej";
            });
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
