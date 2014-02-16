
namespace Framework.Iris.Dynamics
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    public class TypeFactory : BaseTypeFactory
    {
        protected dynamic _type;

        public TypeFactory(string asmname)
        {
            try
            {
                templates = Assembly.Load(asmname);
            }
            catch
            {
                Debug.Print("Fail to load assembly!");
            }
        }

        public override dynamic CreateType(string type)
        {
            if (templates != null)
            {
                foreach (Type t in templates.GetTypes())
                {
                    if (t.FullName == type || t.Name == type)
                    {
                        _type = templates.CreateInstance(t.FullName);
                    }
                }
            }
            return _type;
        }

        public override T CreateType<T>()
        {
            if (templates != null)
            {
                foreach (Type t in templates.GetTypes())
                {
                    if (t == typeof(T))
                    {
                        _type = templates.CreateInstance(t.FullName);
                    }
                }
            }
            return _type;
        }

        protected override void SetupValues()
        {
            throw new NotImplementedException();
            //System.ComponentModel.TypeDescriptor.GetConverter(typeof(Console)).
            //    dynamic _dict = null;
            //    PropertyInfo pi;
            //    if (_dict != null)
            //    {
            //        for (int i = 0; i <= _dict.Count - 1; i++)
            //        {
            //            pi = _type.GetType().GetProperty(_dict(i).Key.ToString());
            //            if (pi != null)
            //            {
            //                try
            //                {
            //                    pi.SetValue(_type, Convert.ChangeType(_dict(i).Value, pi.PropertyType), null);

            //                }
            //                catch
            //                {
            //                }
            //            }
            //        }
            //    }
        }
    }
}
