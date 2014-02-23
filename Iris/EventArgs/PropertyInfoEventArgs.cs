using System;
using System.Reflection;

namespace Framework.Iris.EventArgs
{
    public sealed class PropertyInfoEventArgs
    {
        private FieldInfo _fieldInfo;
        private Type _type;
        private object _storedValue;
        private object _exposedValue;

        public FieldInfo FieldInfo
        {
            get { return _fieldInfo; }
            set { _fieldInfo = value; }
        }

        public Type Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public object StoredValue
        {
            get { return _storedValue; }
            set { _storedValue = value; }
        }

        public object ExposedValue
        {
            get { return _exposedValue; }
            set { _exposedValue = value; }
        }
    }
}
