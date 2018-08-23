using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReaService
{
    public static class AwaitableExtensions
    {
        public static ConfiguredTaskAwaitable PrintContext(this ConfiguredTaskAwaitable t, [CallerMemberName]string callerName = null, [CallerLineNumber]int line = 0)
        {
            PrintContext(callerName, line);
            return t;
        }

        public static Task PrintContext(this Task t, [CallerMemberName]string callerName = null, [CallerLineNumber]int line = 0)
        {
            PrintContext(callerName, line);
            return t;
        }

        private static void PrintContext([CallerMemberName]string callerName = null, [CallerLineNumber]int line = 0)
        {
            var ctx = SynchronizationContext.Current;
            if (ctx != null)
            {
                Console.WriteLine("{0}:{1:D4} await context will be {2}:", callerName, line, ctx);
                Console.WriteLine("    TSCHED:{0}", TaskScheduler.Current);
            }
            else
            {
                Console.WriteLine("{0}:{1:D4} await context will be <NO CONTEXT>", callerName, line);
                Console.WriteLine("    TSCHED:{0}", TaskScheduler.Current);
            }
        }
    }
}
