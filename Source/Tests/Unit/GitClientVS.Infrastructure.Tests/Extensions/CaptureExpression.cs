using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rhino.Mocks;

namespace GitClientVS.Infrastructure.Tests.Extensions
{
    public class CaptureExpression<TMock>
        where TMock : class
    {
        private readonly TMock _stub;

        public CaptureExpression(TMock stub)
        {
            _stub = stub;
        }

        public CaptureResult<TArgument> Args<TArgument, TResult>(Func<TMock, TArgument, Task<TResult>> methodExpression, TResult result = default(TResult))
        {
            return Args(methodExpression, Task.FromResult(result));
        }

        public CaptureResult<TArgument> Args<TArgument>(Func<TMock, TArgument, Task> methodExpression)
        {
            return Args(methodExpression, Task.CompletedTask);
        }

        public CaptureResult<TArgument> Args<TArgument, TResult>(Func<TMock, TArgument, TResult> methodExpression, TResult result = default(TResult))
        {
            var argsCaptured = new List<TArgument>();

            TResult CaptureArg(TArgument x)
            {
                argsCaptured.Add(x);
                return result;
            }

            void StubArg(TMock stub) => methodExpression(stub, default(TArgument));

            _stub.Stub(StubArg).IgnoreArguments().Do((Func<TArgument, TResult>) CaptureArg);

            return CaptureResult<TArgument>.Create(argsCaptured);
        }

        public CaptureResult<(TArgument1 arg1, TArgument2 arg2)> Args<TArgument1, TArgument2, TResult>(Func<TMock, TArgument1, TArgument2, TResult> methodExpression, TResult result = default(TResult))
        {
            var argsCaptured = new List<(TArgument1 arg1, TArgument2 arg2)>();

            Func<TArgument1, TArgument2, TResult> captureArg = (arg1, arg2) =>
            {
                argsCaptured.Add((arg1, arg2));
                return result;
            };

            Action<TMock> stubArg = stub => methodExpression(stub, default(TArgument1), default(TArgument2));

            _stub.Stub(stubArg).IgnoreArguments().Do(captureArg);

            return CaptureResult<(TArgument1 arg1, TArgument2 arg2)>.Create(argsCaptured);
        }
    }
}