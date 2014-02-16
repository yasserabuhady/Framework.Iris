
namespace Framework.Iris.PE
{
    using System.IO;

    public class Image
    {
        public void Read(Stream filestream)
        {
            var fs = filestream as FileStream;
            byte[] array = null;
            var t = fs.Read(array, 0, (int)fs.Length);
        }
    }
}
