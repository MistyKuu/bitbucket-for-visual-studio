using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Infrastructure.Extensions
{
    public static class ChainingExtensions
    {
        [DebuggerStepThrough]
        public static TInput Then<TInput>(this TInput obj, Action<TInput> action)
        {
            action(obj);
            return obj;
        }
        [DebuggerStepThrough]
        public static TOutput Then<TInput, TOutput>(this TInput obj, Func<TInput, TOutput> action)
        {
            return action(obj);
        }
    }
}