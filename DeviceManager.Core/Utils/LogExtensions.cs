using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceManager.Core.Utils
{
    public class LogExtensions
    {
        public class CallerEnricher : ILogEventEnricher
        {
            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                var skip = 3;
                while (true)
                {
                    var stack = new StackFrame(skip);
                    if (stack.GetMethod().Name == null)
                    {
                        logEvent.AddPropertyIfAbsent(new LogEventProperty("Caller", new ScalarValue("<unknown method>")));
                        return;
                    }
                    var method = stack.GetMethod();
                    if (method.DeclaringType != null)
                    {
                        if (method.DeclaringType.Assembly != typeof(Log).Assembly)
                        {
                            var caller = @"" + method.DeclaringType.FullName + "." + method.Name + "(" + string.Join(", ", method.GetParameters().Select(pi => pi.ParameterType.FullName)) + ")";
                            logEvent.AddPropertyIfAbsent(new LogEventProperty("Caller", new ScalarValue(caller)));
                            return;
                        }
                    }
                    skip++;
                }
            }
        }
        public class ThreadIdEnricher : ILogEventEnricher
        {
            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ThreadId", Thread.CurrentThread.ManagedThreadId));
            }
        }
    }

}
