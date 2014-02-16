
namespace Framework.Iris
{
    using Framework.Iris.EventArgs;
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class FieldVisitAttribute : Attribute, IPropertyBound
    {
        public abstract void OnGetValue(PropertyInfoEventArgs PropInfo); 

        public abstract void OnSetValue(PropertyInfoEventArgs PropInfo);
    }
}
