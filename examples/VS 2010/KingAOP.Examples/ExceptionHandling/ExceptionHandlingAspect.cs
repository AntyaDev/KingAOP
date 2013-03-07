using System;
using System.Reflection;
using System.Text;
using KingAOP.Aspects;

namespace KingAOP.Examples.ExceptionHandling
{
    class ExceptionHandlingAspect : OnMethodBoundaryAspect
    {
        public override void OnException(MethodExecutionArgs args)
        {
            var str = new StringBuilder();
            str.Append(args.Exception);

            if (args.Instance != null)
            {
                var instType = args.Instance.GetType();
                foreach (var property in instType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    str.AppendFormat("{0} = {1}", property.Name, property.GetValue(args.Instance, null));
                }
            }
            Console.WriteLine(str.ToString());

            args.FlowBehavior = FlowBehavior.Continue;
        }
    }
}
