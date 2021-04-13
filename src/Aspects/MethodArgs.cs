using System.Reflection;

namespace KingAOP.Aspects
{
    public abstract class MethodArgs : AdviceArgs
    {
        protected MethodArgs(object instance, MethodInfo method, Arguments arguments) : base(instance)
        {
            Method = method;
            Arguments = arguments;
        }

        /// <summary>
        /// Gets the method being executed.
        /// </summary>
        public MethodInfo Method { get; private set; }

        /// <summary>
        /// Gets the arguments with which the method has been invoked.
        /// </summary>
        public Arguments Arguments { get; private set; }

        /// <summary>
        /// Gets or sets the method return value.
        /// </summary>
        public object ReturnValue { get; set; }
    }
}
