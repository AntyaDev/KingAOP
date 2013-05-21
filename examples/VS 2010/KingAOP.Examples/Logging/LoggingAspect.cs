using System;
using System.Text;
using KingAOP.Aspects;

namespace KingAOP.Examples.Logging
{
    internal class LoggingAspect : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            string logData = CreateLogData("Entering", args);
            Console.WriteLine(logData);
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            string logData = CreateLogData("Leaving", args);
            Console.WriteLine(logData);
        }

        private string CreateLogData(string methodStage, MethodExecutionArgs args)
        {
            var str = new StringBuilder();
            str.AppendLine();
            str.AppendLine(string.Format(methodStage + " {0} ", args.Method));
            foreach (var argument in args.Arguments)
            {
                var argType = argument.GetType();

                str.Append(argType.Name + ": ");

                if (argType == typeof(string) || argType.IsPrimitive)
                {
                    str.Append(argument);
                }
                else
                {
                    foreach (var property in argType.GetProperties())
                    {
                        str.AppendFormat("{0} = {1}; ",
                            property.Name, property.GetValue(argument, null));
                    }
                }
            }
            return str.ToString();
        }
    }
}
