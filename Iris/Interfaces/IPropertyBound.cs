

namespace Framework.Iris
{
    using Framework.Iris.EventArgs;

    public interface IPropertyBound
    {
        void OnGetValue(PropertyInfoEventArgs PropInfo);
        void OnSetValue(PropertyInfoEventArgs PropInfo);
    }
}
