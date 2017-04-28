using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GitClientVS.Contracts.Interfaces.Views;
using ReactiveUI;

namespace GitClientVS.Infrastructure.Tests.Extensions
{
    public static class TestExtensions
    {
        public static Task<T> FromTaskAsync<T>(this T obj)
        {
            return Task.FromResult(obj);
        }

        public static ExportFactory<TObject> CreateFactoryMock<TObject>(this TObject obj)
        {
            return new ExportFactory<TObject>(() => new Tuple<TObject, Action>(obj, () => Console.WriteLine("Releasing...")));
        }
    }
}
