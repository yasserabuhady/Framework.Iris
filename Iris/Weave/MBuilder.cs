
namespace Framework.Iris.Weave
{
    using Framework.Iris.Statics;
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;

    internal class MBuilder : BaseBuilder
    {
        private MethodDefinition ctorMethodHandleAttribute, ctorExceptionHandleAttribute, _targetMethod;

        internal MBuilder(MethodDefinition targetMethod)
        {
            _targetMethod = targetMethod;
        }

        internal override void BuildIL()
        {
            CallOriginal();
        }

        private void CallOriginal()
        {
            MethodDefinition newmethod = new MethodDefinition(_targetMethod.Name, _targetMethod.Attributes, _targetMethod.ReturnType);
            _targetMethod.Name += "_origin";

            #region Variables
            var methodinfo = new VariableDefinition(_targetMethod.Module.LookupTypeRef(typeof(System.Reflection.MethodInfo)));
            newmethod.Body.Variables.Add(methodinfo);

            var args = new VariableDefinition(_targetMethod.Module.LookupTypeRef(typeof(object[])));
            newmethod.Body.Variables.Add(args);

            var methodArgs = new VariableDefinition(TypeRefs.tdMethodInfoEventArgs);
            newmethod.Body.Variables.Add(methodArgs);

            var methodBoundAttr = new VariableDefinition(TypeRefs.tdMethodBoundAttribute);
            newmethod.Body.Variables.Add(methodBoundAttr);

            var exceptionHandlerAttr = new VariableDefinition(TypeRefs.tdHandleExceptionAttribute);
            newmethod.Body.Variables.Add(exceptionHandlerAttr);

            var trException = _targetMethod.Module.LookupTypeRef(typeof(Exception));
            var ex = new VariableDefinition(trException);
            newmethod.Body.Variables.Add(ex);

            var bf = new VariableDefinition(_targetMethod.Module.LookupTypeRef(typeof(System.Reflection.BindingFlags)));
            newmethod.Body.Variables.Add(bf);
            #endregion

            var processor = newmethod.Body.GetILProcessor();

            //Build up parameter array
            processor.Emit(OpCodes.Nop);
            if (_targetMethod.Parameters.Count > 0)
            {
                processor.Emit(OpCodes.Ldc_I4, _targetMethod.Parameters.Count);
                processor.Emit(OpCodes.Newarr, TypeSys.Object);
                processor.Emit(OpCodes.Stloc, args);

                for (int i = 0; i < _targetMethod.Parameters.Count; i++)
                {
                    processor.Emit(OpCodes.Ldloc_S, args);
                    processor.Emit(OpCodes.Ldc_I4, i);
                    processor.Emit(OpCodes.Ldarg, i + (_targetMethod.IsStatic ? 0 : 1));

                    var paramType = _targetMethod.Parameters[i].ParameterType;

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
            processor.Emit(OpCodes.Ldtoken, _targetMethod.DeclaringType);
            var mtdGetTypeFromHandle = _targetMethod.Module.LookupTypeRef(typeof(Type)).Resolve().FindMethod("GetTypeFromHandle");
            processor.Emit(OpCodes.Call, mtdGetTypeFromHandle);
            processor.Emit(OpCodes.Ldstr, _targetMethod.Name);
            processor.Emit(OpCodes.Ldloc_S, bf);
            var mtdGetMethod = _targetMethod.Module.LookupTypeRef(typeof(Type)).Resolve().FindMethod("GetMethod");
            processor.Emit(OpCodes.Call, mtdGetMethod);
            processor.Emit(OpCodes.Ldloc_S, args);

            processor.Emit(OpCodes.Newobj, _targetMethod.Module.Import(TypeRefs.ctorMethodInfoEventArgs));
            processor.Emit(OpCodes.Stloc, methodArgs);

            //Import Target MethodHandleAttribute
            processor.Emit(OpCodes.Newobj, ctorMethodHandleAttribute);
            processor.Emit(OpCodes.Stloc, methodBoundAttr);

            //Import Target ExceptionHandleAttribute
            processor.Emit(OpCodes.Newobj, ctorExceptionHandleAttribute);
            processor.Emit(OpCodes.Stloc, exceptionHandlerAttr);

            #region Try
            var catchHandler = new ExceptionHandler(ExceptionHandlerType.Catch);
            catchHandler.CatchType = trException;
            catchHandler.TryStart = Instruction.Create(OpCodes.Nop);
            catchHandler.TryEnd = Instruction.Create(OpCodes.Nop);
            catchHandler.HandlerStart = catchHandler.TryEnd;
            catchHandler.HandlerEnd = Instruction.Create(OpCodes.Nop);
            processor.Body.ExceptionHandlers.Add(catchHandler);

            var finallyHandler = new ExceptionHandler(ExceptionHandlerType.Finally);
            finallyHandler.TryStart = catchHandler.TryStart;
            finallyHandler.TryEnd = Instruction.Create(OpCodes.Nop);
            finallyHandler.HandlerStart = finallyHandler.TryEnd;
            finallyHandler.HandlerEnd = Instruction.Create(OpCodes.Nop);
            processor.Body.ExceptionHandlers.Add(finallyHandler);

            //Try Handle Start
            processor.Append(catchHandler.TryStart);
            processor.Emit(OpCodes.Ldloc_S, methodBoundAttr);
            processor.Emit(OpCodes.Ldloc_S, methodArgs);
            processor.Emit(OpCodes.Callvirt, TypeRefs.OnEntry);
            processor.Emit(OpCodes.Nop);
            processor.Emit(OpCodes.Ldloc_S, methodArgs);
            processor.Emit(OpCodes.Ldarg_0);

            if (_targetMethod.Parameters.Count > 0)
            {
                for (int i = 0; i < _targetMethod.Parameters.Count; i++)
                {
                    processor.Emit(OpCodes.Ldloc_S, methodArgs);
                    processor.Emit(OpCodes.Callvirt, TypeRefs.get_Parameters);
                    processor.Emit(OpCodes.Ldc_I4, i);
                    processor.Emit(OpCodes.Ldelem_Ref);

                    var paramType = _targetMethod.Parameters[i].ParameterType;
                    if (paramType.IsValueType || paramType is GenericParameter)
                    {
                        processor.Emit(OpCodes.Unbox_Any, paramType);
                    }
                }
            }

            processor.Emit(OpCodes.Call, _targetMethod);
            var returnType = _targetMethod.ReturnType;
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

            //Goto Exception End
            processor.Emit(OpCodes.Leave_S, catchHandler.HandlerEnd);
            #endregion

            //Exception Handler Start
            processor.Append(catchHandler.HandlerStart);

            #region Exception Handle
            processor.Emit(OpCodes.Stloc_S, ex);
            processor.Emit(OpCodes.Nop);

            processor.Emit(OpCodes.Ldloc_S, methodArgs);
            processor.Emit(OpCodes.Ldloc_S, ex);
            processor.Emit(OpCodes.Callvirt, TypeRefs.set_Exception);
            processor.Emit(OpCodes.Nop);

            processor.Emit(OpCodes.Ldloc_S, exceptionHandlerAttr);
            processor.Emit(OpCodes.Ldloc_S, methodArgs);
            processor.Emit(OpCodes.Callvirt, TypeRefs.OnException);
            processor.Emit(OpCodes.Nop);

            processor.Emit(OpCodes.Ldloc_S, methodArgs);
            processor.Emit(OpCodes.Callvirt, TypeRefs.get_Behavior);

            processor.Emit(OpCodes.Ldc_I4_2);
            processor.Emit(OpCodes.Ceq);
            var endc = Instruction.Create(OpCodes.Nop);
            processor.Emit(OpCodes.Brfalse_S, endc);
            processor.Emit(OpCodes.Rethrow);
            processor.Append(endc);

            //Goto Exception End
            processor.Emit(OpCodes.Leave_S, catchHandler.HandlerEnd);

            //Exception End
            processor.Append(catchHandler.HandlerEnd);
            #endregion

            //Goto Finally End
            processor.Emit(OpCodes.Leave_S, finallyHandler.HandlerEnd);

            //Finally Start
            processor.Append(finallyHandler.HandlerStart);

            processor.Emit(OpCodes.Ldloc_S, methodBoundAttr);
            processor.Emit(OpCodes.Ldloc_S, methodArgs);
            processor.Emit(OpCodes.Callvirt, TypeRefs.OnExit);
            processor.Emit(OpCodes.Nop);
            processor.Emit(OpCodes.Endfinally);

            //Finally End 
            processor.Append(finallyHandler.HandlerEnd);

            //Return Result
            if (_targetMethod.ReturnType != TypeSys.Void)
            {
                processor.Emit(OpCodes.Ldloc_S, methodArgs);
                processor.Emit(OpCodes.Callvirt, TypeRefs.get_Results);

                if (returnType.IsValueType || returnType is GenericParameter)
                {
                    processor.Emit(OpCodes.Unbox_Any, returnType);
                }
                processor.Emit(OpCodes.Ret);
            }



        }
    }
}
