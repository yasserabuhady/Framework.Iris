
namespace Framework.Iris
{
    using Framework.Iris.EventArgs;
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public abstract class HandleExceptionAttribute : Attribute, IExceptionHandler
    {
        public abstract void OnException(MethodInfoEventArgs MethdoInfo);
    }
}
