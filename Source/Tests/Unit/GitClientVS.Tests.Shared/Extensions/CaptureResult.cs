using System.Collections.Generic;

namespace GitClientVS.Tests.Shared.Extensions
{
    public class CaptureResult<TResult>
    {
        public IList<TResult> Args { get; private set; }

        public int CallCount => Args.Count;

        private CaptureResult() { }

        public static CaptureResult<TResult> Create(IList<TResult> args)
        {
            return new CaptureResult<TResult> {Args = args};
        }
    }
}