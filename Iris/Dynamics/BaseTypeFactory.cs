using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Iris.Dynamics
{
    public abstract class BaseTypeFactory
    {
        public abstract dynamic CreateType(string type);
        public abstract T CreateType<T>();
        protected abstract void SetupValues();
        protected System.Reflection.Assembly templates;
    }
}
