using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace GitClientVS.Tests.Shared.Extensions
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
