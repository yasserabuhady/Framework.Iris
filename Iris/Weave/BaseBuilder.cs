
namespace Framework.Iris.Weave
{
    using Mono.Cecil;

    internal abstract class BaseBuilder
    {
        private TypeSystem _typeSys;

        internal TypeSystem TypeSys
        {
            get { return _typeSys; }
            set { _typeSys = value; }
        }

        internal abstract void BuildIL();

    }
}
