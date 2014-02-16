

namespace Framework.Iris.Weave
{
    using EventArgs;
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Statics;
    using System;
    using System.Linq;

    public class Weaver
    {
        private AssemblyDefinition _sourceasm;
        private AssemblyDefinition _targetasm;
        private TypeSystem _typesys;

        public Weaver(string SourceAsm, string TargetAsm)
        {
            _sourceasm = AssemblyDefinition.ReadAssembly(SourceAsm);
            _targetasm = _sourceasm;
            if (!string.IsNullOrEmpty(TargetAsm) && !SourceAsm.Equals(TargetAsm))
            {
                _targetasm = AssemblyDefinition.ReadAssembly(TargetAsm);
                if (_sourceasm.MainModule.Runtime < _targetasm.MainModule.Runtime)
                {
                    throw new Exception("Target assmebly runtime is higher than source assmebly runtime!");
                }
                //var asmNameRef = new AssemblyNameReference(asm.Name.Name, asm.Name.Version);
                if (!_sourceasm.MainModule.AssemblyReferences.Contains(_targetasm.Name))
                {
                    _sourceasm.MainModule.AssemblyReferences.Add(_targetasm.Name);

                }
            }
            _typesys = _targetasm.MainModule.TypeSystem;
        }

        public MethodReference Import(TypeDefinition soureclass, string methodref)
        {
            var method = soureclass.Methods.First(m => m.Name == methodref);
            return _sourceasm.MainModule.Import(method);
        }

        public void BuildIL(MethodDefinition targetmethod)
        {
            var processor = targetmethod.Body.GetILProcessor();
            processor.InsertBefore(targetmethod.Body.Instructions[0], Instruction.Create(OpCodes.Call, targetmethod));

        }

        private void CallOriginal(MethodDefinition targetMethod)
        {
            //var openUnbox = ImportMethod<StubHelper>("Unbox");

            //var callOriginal = new MethodDefinition("CallOriginal",
            //                                      MethodAttributes.Private | MethodAttributes.HideBySig |
            //                                      MethodAttributes.Static, _objType);
            MethodDefinition newmethod = new MethodDefinition(targetMethod.Name, targetMethod.Attributes, targetMethod.ReturnType);
            targetMethod.Name += "_origin";

            #region Variables
            var methodinfo = new VariableDefinition(targetMethod.Module.LookupTypeRef(typeof(System.Reflection.MethodInfo)));
            newmethod.Body.Variables.Add(methodinfo);

            var args = new VariableDefinition(targetMethod.Module.LookupTypeRef(typeof(object[])));
            newmethod.Body.Variables.Add(args);

            var methodArgs = new VariableDefinition(TypeRefs.tdMethodInfoEventArgs);
            newmethod.Body.Variables.Add(methodArgs);

            var methodBoundAttr = new VariableDefinition(TypeRefs.tdMethodBoundAttribute);
            newmethod.Body.Variables.Add(methodBoundAttr);

            var exceptionHandlerAttr = new VariableDefinition(TypeRefs.tdHandleExceptionAttribute);
            newmethod.Body.Variables.Add(exceptionHandlerAttr);

            var trException = targetMethod.Module.LookupTypeRef(typeof(Exception));
            var ex = new VariableDefinition(trException);
            newmethod.Body.Variables.Add(ex);

            var bf = new VariableDefinition(targetMethod.Module.LookupTypeRef(typeof(System.Reflection.BindingFlags)));
            newmethod.Body.Variables.Add(bf);
            #endregion


            var processor = newmethod.Body.GetILProcessor();

            //var args = new VariableDefinition(_typesys.Object);

            //Build up parameter array
            processor.Emit(OpCodes.Nop);
            if (targetMethod.Parameters.Count > 0)
            {
                processor.Emit(OpCodes.Ldc_I4, targetMethod.Parameters.Count);
                processor.Emit(OpCodes.Newarr, _typesys.Object);
                processor.Emit(OpCodes.Stloc, args);

                for (int i = 0; i < targetMethod.Parameters.Count; i++)
                {
                    processor.Emit(OpCodes.Ldloc_S, args);
                    processor.Emit(OpCodes.Ldc_I4, i);
                    processor.Emit(OpCodes.Ldarg, i + (targetMethod.IsStatic ? 0 : 1));

                    var paramType = targetMethod.Parameters[i].ParameterType;

                    if (paramType.IsValueType || paramType is GenericParameter)
                    {
                        processor.Emit(OpCodes.Box, paramType);
                    }

                    processor.Emit(OpCodes.Stelem_Ref);
                }
            }
            processor.Emit(OpCodes.Nop);

            processor.Emit(OpCodes.Ldc_I4, 0x124);
            processor.Emit(OpCodes.Stloc, bf);

            //set new instance of MethodInfoEventArgs
            processor.Emit(OpCodes.Ldtoken, targetMethod.DeclaringType);
            var mtdGetTypeFromHandle = targetMethod.Module.LookupTypeRef(typeof(Type)).Resolve().FindMethod("GetTypeFromHandle");
            processor.Emit(OpCodes.Call, mtdGetTypeFromHandle);
            processor.Emit(OpCodes.Ldstr, targetMethod.Name);
            processor.Emit(OpCodes.Ldloc_S, bf);
            var mtdGetMethod = targetMethod.Module.LookupTypeRef(typeof(Type)).Resolve().FindMethod("GetMethod");
            processor.Emit(OpCodes.Call, mtdGetMethod);
            processor.Emit(OpCodes.Ldloc_S, args);
            processor.Emit(OpCodes.Newobj, targetMethod.Module.Import(TypeRefs.ctorMethodInfoEventArgs));
            processor.Emit(OpCodes.Stloc, methodArgs);
            //Import Target MethodHandleAttribute
            //processor.Emit(OpCodes.Newobj, targetMethod.Module.Import(""));
            processor.Emit(OpCodes.Stloc, methodBoundAttr);
            //Import Target ExceptionHandleAttribute
            //processor.Emit(OpCodes.Newobj, targetMethod.Module.Import(""));
            processor.Emit(OpCodes.Stloc, exceptionHandlerAttr);
            processor.Emit(OpCodes.Nop);

            processor.Emit(OpCodes.Ldloc_S, methodBoundAttr);
            processor.Emit(OpCodes.Ldloc_S, methodArgs);
            processor.Emit(OpCodes.Callvirt, TypeRefs.OnEntry);
            processor.Emit(OpCodes.Nop);
            processor.Emit(OpCodes.Ldloc_S, methodArgs);
            processor.Emit(OpCodes.Ldarg_0);

            if (targetMethod.Parameters.Count > 0)
            {
                for (int i = 0; i < targetMethod.Parameters.Count; i++)
                {
                    processor.Emit(OpCodes.Ldloc_S, methodArgs);
                    processor.Emit(OpCodes.Callvirt, TypeRefs.get_Parameters);
                    processor.Emit(OpCodes.Ldc_I4, i);
                    processor.Emit(OpCodes.Ldelem_Ref);

                    var paramType = targetMethod.Parameters[i].ParameterType;
                    if (paramType.IsValueType || paramType is GenericParameter)
                    {
                        processor.Emit(OpCodes.Unbox_Any, paramType);
                    }
                }
            }

            processor.Emit(OpCodes.Call, targetMethod);
            var returnType = targetMethod.ReturnType;
            if (returnType.IsValueType || returnType is GenericParameter)
            {
                processor.Emit(OpCodes.Box, returnType);
            }
            processor.Emit(OpCodes.Callvirt, TypeRefs.set_Results);
            processor.Emit(OpCodes.Nop);

            processor.Emit(OpCodes.Ldloc_S, methodBoundAttr);
            processor.Emit(OpCodes.Ldloc_S, methodArgs);
            processor.Emit(OpCodes.Callvirt, TypeRefs.OnSuccess);
            processor.Emit(OpCodes.Nop);

            //Goto finally
            //processor.Emit(OpCodes.Leave_S, Instruction);

            #region Exception Handle
            processor.Emit(OpCodes.Stloc_S, ex);
            processor.Emit(OpCodes.Nop);

            processor.Emit(OpCodes.Ldloc_S, methodArgs);
            processor.Emit(OpCodes.Ldloc_S, ex);
            processor.Emit(OpCodes.Callvirt, TypeRefs.set_Exception);
            processor.Emit(OpCodes.Nop);

            processor.Emit(OpCodes.Ldloc_S, methodBoundAttr);
            processor.Emit(OpCodes.Ldloc_S, methodArgs);
            processor.Emit(OpCodes.Callvirt, TypeRefs.OnException);
            processor.Emit(OpCodes.Nop);

            processor.Emit(OpCodes.Ldloc_S, methodArgs);
            processor.Emit(OpCodes.Callvirt, TypeRefs.get_Behavior);

            processor.Emit(OpCodes.Ldc_I4_2);
            processor.Emit(OpCodes.Ceq);
            processor.Emit(OpCodes.Brfalse_S, Instruction);
            processor.Emit(OpCodes.Rethrow);
            processor.Emit(OpCodes.Nop);
            processor.Emit(OpCodes.Leave_S, Instruction);
            processor.Emit(OpCodes.Nop);
            processor.Emit(OpCodes.Leave_S, Instruction);

            processor.Emit(OpCodes.Nop);
            processor.Emit(OpCodes.Ldloc_S, methodBoundAttr);
            processor.Emit(OpCodes.Ldloc_S, methodArgs);
            processor.Emit(OpCodes.Callvirt, TypeRefs.OnExit);
            processor.Emit(OpCodes.Nop);
            processor.Emit(OpCodes.Endfinally);
            #endregion

            processor.Emit(OpCodes.Nop);
            processor.Emit(OpCodes.Ldloc_S, methodArgs);
            processor.Emit(OpCodes.Callvirt, TypeRefs.get_Results);


            if (returnType.IsValueType || returnType is GenericParameter)
            {
                processor.Emit(OpCodes.Unbox_Any, returnType);
            }
            processor.Emit(OpCodes.Ret);

            var catchHandler = new ExceptionHandler(ExceptionHandlerType.Catch);
            catchHandler.CatchType = trException;
            catchHandler.TryStart = Instruction;
            catchHandler.TryEnd = Instruction;
            catchHandler.HandlerStart = Instruction;
            catchHandler.HandlerEnd = Instruction;
            processor.Body.ExceptionHandlers.Add(catchHandler);

            var finallyHandler = new ExceptionHandler(ExceptionHandlerType.Finally);
            finallyHandler.TryStart = Instruction;
            finallyHandler.TryEnd = Instruction;
            finallyHandler.HandlerStart = Instruction;
            finallyHandler.HandlerEnd = Instruction;
            processor.Body.ExceptionHandlers.Add(finallyHandler);
         
        }
    }
}
