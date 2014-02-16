
namespace Framework.Iris.PE
{
    using System.IO;

    public class StreamManager
    {
        ImageReader reader;

        public StreamManager(Stream basestream)
        {
            reader = new ImageReader(basestream);
        }

        public ImageReader getImageReader()
        {
            return reader;
        }

        public ImageWriter getImageWriter()
        {
            return new ImageWriter(reader.BaseStream);
        }

    }

    public class ImageReader : BinaryReader
    {
        public ImageReader(Stream basestream) : base(basestream) { }
    }

    public class ImageWriter : BinaryWriter
    {
        public ImageWriter(Stream basestream)
            : base(basestream)
        { }
    }

}
