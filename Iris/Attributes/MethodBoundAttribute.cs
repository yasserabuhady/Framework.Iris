
namespace Framework.Iris
{
    using Framework.Iris.EventArgs;
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public abstract class MethodBoundAttribute : Attribute, IMethodBound
    {
        public abstract void OnEntry(MethodInfoEventArgs MethodInfo);

        public abstract void OnExit(MethodInfoEventArgs MethodInfo);

        public abstract void OnSuccess(MethodInfoEventArgs MethodInfo);

    }
}
