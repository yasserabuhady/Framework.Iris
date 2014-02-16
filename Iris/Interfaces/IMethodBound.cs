
namespace Framework.Iris
{
    using Framework.Iris.EventArgs;

    public interface IMethodBound
    {
        void OnEntry(MethodInfoEventArgs MethdoInfo);
        void OnExit(MethodInfoEventArgs MethdoInfo);
        void OnSuccess(MethodInfoEventArgs MethodInfo);
    }
}
