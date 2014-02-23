
namespace Framework.Iris.Weave
{
    using Framework.Iris.Statics;
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;

    internal class PBuilder : BaseBuilder
    {
        private PropertyDefinition _targetProperty;

        internal PBuilder(PropertyDefinition targetProperty)
        {
            _targetProperty = targetProperty;
        }

        internal override void BuildIL()
        {
            throw new NotImplementedException();
        }
    }
}
