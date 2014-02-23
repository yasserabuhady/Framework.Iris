
namespace Framework.Iris.Weave
{
    using Mono.Cecil;
    using System;

    public class Weaver
    {
        private AssemblyDefinition _sourceAsm;
        private AssemblyDefinition _targetAsm;
        private BaseBuilder _builder;

        public Weaver(string SourceAsm, string TargetAsm)
        {
            _sourceAsm = AssemblyDefinition.ReadAssembly(SourceAsm);
            _targetAsm = _sourceAsm;
            if (!string.IsNullOrEmpty(TargetAsm) && !SourceAsm.Equals(TargetAsm))
            {
                _targetAsm = AssemblyDefinition.ReadAssembly(TargetAsm);
                if (_sourceAsm.MainModule.Runtime < _targetAsm.MainModule.Runtime)
                {
                    throw new Exception("Target assmebly runtime is higher than source assmebly runtime!");
                }
                if (!_sourceAsm.MainModule.AssemblyReferences.Contains(_targetAsm.Name))
                {
                    _sourceAsm.MainModule.AssemblyReferences.Add(_targetAsm.Name);
                }
            }
        }

        internal void Weave(BaseBuilder builder)
        {
            if (_sourceAsm == null || _targetAsm == null)
                throw new Exception("Assembly initialize not finished!");
            builder.TypeSys = _targetAsm.MainModule.TypeSystem;
            builder.BuildIL();
        }

    }
}
