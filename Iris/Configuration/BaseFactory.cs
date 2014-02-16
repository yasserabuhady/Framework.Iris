
namespace Framework.Iris.Configuration
{
    public class BaseFactory
    {
        private BaseFactory _instance = new BaseFactory();
        private BaseXMLParser _Parser;

        private BaseFactory() { }

        public BaseFactory Instance()
        {
            return _instance;
        }

        public BaseFactory Instance(BaseXMLParser Parser)
        {
            _Parser = Parser;
            return _instance;
        }

        public void ReadConfiguration()
        {
            _Parser.Read();
        }
    }
}
