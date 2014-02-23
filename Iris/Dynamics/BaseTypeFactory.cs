
namespace Framework.Iris.Dynamics
{
    using System.Reflection;

    public abstract class BaseTypeFactory
    {
        public abstract dynamic CreateType(string type);
        public abstract T CreateType<T>();
        protected abstract void SetupValues();
        protected Assembly templates;
    }
}
