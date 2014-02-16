
namespace Framework.Iris
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Linq;

    /// <summary>
    /// Extension Methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Get an exist TypeReference form the moudle.
        /// </summary>
        /// <param name="fullname">Fullname of the target class.</param>
        /// <returns>The TypeReference of the first class.</returns>
        internal static TypeReference GetTypeRef(this ModuleDefinition mod, string fullname)
        {
            return mod.GetTypeReferences().First(t => t.FullName == fullname);
        }

        /// <summary>
        /// Try to get a TypeReference which does not exist in the module. Keep the same socpe with 
        /// the specific module.
        /// </summary>
        /// <param name="type">The target class.</param>
        /// <returns>The TypeReference of target class.</returns>
        internal static TypeReference LookupTypeRef(this ModuleDefinition mod, Type type)
        {
            AssemblyNameReference scope;
            try
            {
                scope = mod.AssemblyReferences.First(ar => ar.FullName == type.Assembly.FullName);
            }
            catch
            {
                try
                {
                    scope = mod.AssemblyReferences.First(ar => ar.Name == type.Assembly.GetName().Name);
                }
                catch
                {
                    throw new Exception("Target typereference " + type.FullName + " not found!");
                }

            }
            return new TypeReference(type.Namespace, type.Name, mod, scope, type.IsValueType);
        }

        /// <summary>
        /// Get an exist MethodReference from the moudle.
        /// </summary>
        /// <param name="fullName">Fullname of the method.</param>
        /// <returns>The first MethodReference which matched the fullname of the method.</returns>
        internal static MethodReference GetMemberRef(this ModuleDefinition mod, string fullName)
        {
            MemberReference member = null;
            try
            {
                member = mod.GetMemberReferences().First(m => m.FullName == fullName);
            }
            catch
            {
                throw new Exception("Target member " + fullName + " not found!");
            }
            return member as MethodReference;
        }

        /// <summary>
        /// Get a TypeDefinition which defined by the moudle.
        /// </summary>
        /// <param name="name">Name of the class.</param>
        /// <returns>The TypeDefinition of the specified class.</returns>
        internal static TypeDefinition FindClass(this ModuleDefinition mod, string name)
        {
            var calss = mod.GetType(name);
            if (calss == null)
            {
                throw new Exception("Target class " + name + " not found!");
            }
            return calss;
        }


        /// <summary>
        /// Get a MethodDefinition which defined by the class.
        /// </summary>
        /// <param name="name">Name of the method.</param>
        /// <returns>The first MethodDefinition which mathes the name specified method.</returns>
        internal static MethodDefinition FindMethod(this TypeDefinition @class, string name)
        {
            MethodDefinition method = null;
            try
            {
                method = @class.Methods.First(m => m.Name == name || m.FullName == name);
            }
            catch
            {
                throw new Exception("Target method " + name + " not found!");
            }
            return method;
        }

        /// <summary>
        /// Get an Instruction from the module.
        /// </summary>
        /// <param name="code">The target OpCode.</param>
        /// <param name="oprend">The target Oprend.</param>
        /// <returns>The first Instruction which contains the specified OpCode and Oprend.</returns>
        internal static Instruction FindInstruction(this MethodDefinition method, OpCode? code, object oprend)
        {
            Instruction ins = null;
            try
            {
                if (code == null)
                    ins = method.Body.Instructions.First(i => i.Operand != null && i.Operand.Equals(oprend));
                else
                    ins = method.Body.Instructions.First(i => i.OpCode.Equals(code) && i.Operand.Equals(oprend));
            }
            catch
            {
                throw new Exception("Target instruction not found!");
            }
            return ins;
        }

        /// <summary>
        /// Get an Instruction which matched the oprend.
        /// </summary>
        /// <param name="oprend">The target Oprend.</param>
        /// <returns>The first Instruction which contained the specified Oprend.</returns>
        internal static Instruction FindInstruction(this MethodDefinition method, object oprend)
        {
            return FindInstruction(method, null, oprend);
        }

        /// <summary>
        /// Get a ParameterDefinition from the method.
        /// </summary>
        /// <param name="name">The target parameter's name.</param>
        /// <returns>The first ParameterDefinition which mathes the name of the parameter.</returns>
        internal static ParameterDefinition FindParameter(this MethodDefinition method, string name)
        {
            ParameterDefinition param = null;
            try
            {
                param = method.Parameters.First(p => p.Name == name);
            }
            catch
            {
                throw new Exception("Target parameter " + name + " not found!");
            }
            return param;
        }

       
    }
}
