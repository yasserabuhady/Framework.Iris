
namespace Framework.Iris.Statics
{
    using Mono.Cecil;

    internal static class TypeRefs
    {
        private static readonly AssemblyDefinition asm;
        public static readonly MethodDefinition ctorMethodInfoEventArgs,
                                                OnEntry,
                                                OnSuccess,
                                                OnExit,
                                                OnException,
                                                get_Parameters,
                                                get_Behavior,
                                                get_Results,
                                                set_Results,
                                                set_Exception;

        public static readonly TypeDefinition tdMethodInfoEventArgs,
                                              tdMethodBoundAttribute,
                                              tdHandleExceptionAttribute,
                                              tdFowardBehaviors;

        static TypeRefs()
        {
            asm = AssemblyDefinition.ReadAssembly("Framework.Iris.dll");
            tdMethodInfoEventArgs = asm.MainModule.GetType("Framework.Iris.EventArgs.MethodInfoEventArgs");
            tdMethodBoundAttribute = asm.MainModule.GetType("Framework.Iris.MethodBoundAttribute");
            tdHandleExceptionAttribute = asm.MainModule.GetType("Framework.Iris.HandleExceptionAttribute");
            tdFowardBehaviors = asm.MainModule.GetType("Framework.Iris.FowardBehaviors");

            ctorMethodInfoEventArgs = tdMethodInfoEventArgs.FindMethod(".ctor");
            OnEntry = tdMethodBoundAttribute.FindMethod("OnEntry");
            OnSuccess = tdMethodBoundAttribute.FindMethod("OnSuccess");
            OnExit = tdMethodBoundAttribute.FindMethod("OnExit");
            OnException = tdHandleExceptionAttribute.FindMethod("OnException");

            get_Parameters = tdMethodInfoEventArgs.FindMethod("get_Parameters");
            get_Behavior = tdMethodInfoEventArgs.FindMethod("get_Behavior");
            get_Results = tdMethodInfoEventArgs.FindMethod("get_Results");
            set_Results = tdMethodInfoEventArgs.FindMethod("set_Results");
            set_Exception = tdMethodInfoEventArgs.FindMethod("set_Exception");

        }

    }
}
