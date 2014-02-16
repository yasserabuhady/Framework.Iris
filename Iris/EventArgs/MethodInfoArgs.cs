
namespace Framework.Iris.EventArgs
{
    using System;
    using System.Reflection;

    public sealed class MethodInfoEventArgs
    {
        private MethodInfo _method;
        private object[] _args;
        private object _results;
        private FowardBehaviors _behavior = FowardBehaviors.Continue;
        private object _state;
        private Exception _exception;

        public MethodInfo Method
        {
            get { return _method; }
            set { _method = value; }
        }

        public object[] Parameters
        {
            get { return _args; }
            set { _args = value; }
        }

        public object Results
        {
            get { return _results; }
            set
            {
                if (_method.ReturnType.FullName == "System.Void")
                {
                    throw new Exception("Results cannot be set when the return type is void.");
                }
                _results = value;
            }
        }

        public FowardBehaviors Behavior
        {
            get { return _behavior; }
            set { _behavior = value; }
        }

        public object State
        {
            get { return _state; }
            set { _state = value; }
        }

        public Exception Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }

        public MethodInfoEventArgs(MethodInfo method, object[] args)
        {
            _method = method;
            _args = args;
        }
    }
}
