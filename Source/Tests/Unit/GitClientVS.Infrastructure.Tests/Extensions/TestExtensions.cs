using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;

namespace GitClientVS.Infrastructure.Tests.Extensions
{
    public static class TestExtensions
    {
        public static Task<T> FromTaskAsync<T>(this T obj)
        {
            return Task.FromResult(obj);
        }

        public static void WaitToFinish(this ICommand command)
        {
            while (!command.CanExecute(null)) { }
        }
    }
}
