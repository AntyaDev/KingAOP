using System.Reflection;
using KingAOP.Aspects;

namespace KingAOP.Examples.ExceptionHandling
{
    class ExceptionHandlingAspect : OnMethodBoundaryAspect
    {
        public override void OnException(MethodExecutionArgs args)
        {
            if (args.Instance != null)
            {
                var instType = args.Instance.GetType();
                foreach (var property in instType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    
                }
            }
        }
    }
}
