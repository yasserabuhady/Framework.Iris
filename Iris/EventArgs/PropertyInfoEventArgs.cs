using System;
using System.Reflection;

namespace Framework.Iris.EventArgs
{
    public sealed class PropertyInfoEventArgs
    {
        private FieldInfo _fieldInfo;
        private Type _type;
        private dynamic _storedvalue;
        private dynamic _exposedvalue;

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

        public dynamic StoredValue
        {
            get { return _storedvalue; }
            set { _storedvalue = value; }
        }

        public dynamic ExposedValue
        {
            get { return _exposedvalue; }
            set { _exposedvalue = value; }
        }
    }
}
