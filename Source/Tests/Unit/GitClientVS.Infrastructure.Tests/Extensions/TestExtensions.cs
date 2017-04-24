using System;
using System.Collections.Generic;
using System.Linq;
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

        //public static void With(this IScheduler scheduler,Action<IScheduler> schedulerAction)
        //{
        //    //var tps = RxApp.TaskpoolScheduler;
        //    //var mts = RxApp.MainThreadScheduler;

        //    //RxApp.TaskpoolScheduler = scheduler;
        //    //RxApp.MainThreadScheduler = scheduler;

        //    //schedulerAction(scheduler);

        //    //RxApp.TaskpoolScheduler = tps;
        //    //RxApp.MainThreadScheduler = mts;
        //}
    }
}
