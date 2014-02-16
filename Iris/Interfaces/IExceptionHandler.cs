
namespace Framework.Iris
{
    using Framework.Iris.EventArgs;

    public interface IExceptionHandler
    {
        void OnException(MethodInfoEventArgs MethdoInfo);
    }
}
