using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GitClientVS.UI.Converters
{
    public class TaskCompletionNotifier<TResult> : INotifyPropertyChanged
    {
        private TResult _result;

        public TResult Result
        {
            get { return _result; }
            set
            {
                _result = value;
                OnPropertyChanged();
            }
        }


        public async Task StartAsync(Task<TResult> task, TResult defaultValue = default(TResult))
        {
            try
            {
                Result = await task;
            }
            catch (Exception)
            {
                Result = default(TResult);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}