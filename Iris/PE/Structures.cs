
namespace Framework.Iris.PE
{
    //delegate void Update(uint startindex, byte[] codes);

    public abstract class Range
    {
        private uint _start;
        private uint _length;
        protected static byte[] _data;
        //protected Update update;

        public uint Start
        {
            get { return _start; }
            internal set { _start = value; }
        }

        public uint Length
        {
            get { return (uint)Data.Length; }
            internal set { _length = value; }
        }

        public byte[] Data
        {
            get
            {
                byte[] datarange = new byte[_length];
                for (uint i = _start, j = 0; i < _length; i++, j++)
                {
                    datarange[j] = _data[i];
                }
                return datarange;
            }
            //internal set
            //{
            //    update(_start, value);
            //    _data = value;
            //}
        }

        internal Range(uint start)
        {
            _start = start;
        }

        internal Range(byte[] data, uint start)
        {
            _data = data;
            _start = start;
        }

        internal Range(uint start, uint length)
        {
            this.Start = start;
            this.Length = length;
        }

        public int BytesToInt(byte[] data)
        {
            int x = 0;
            for (int i = 0; i < data.Length; i++)
            {
                x += data[i] << 8;
            }
            return x;
        }
    }

    public class PEImage : Range
    {
        DOSHeader DOSHeader;

        public PEImage(byte[] data, uint start)
            : base(data, 0)
        {
        }
    }

    public class DOSHeader : Range
    {
        IMAGE_DOS_HEADER image_dos_header;
        DOS_STUB dos_stub;

        internal DOSHeader(uint start)
            : base(start)
        {
            image_dos_header = new IMAGE_DOS_HEADER(start);
           // dos_stub = new DOS_STUB(start, image_dos_header.e_lfanew - 64);
        }

        // DOS .EXE header 64bytes
        public class IMAGE_DOS_HEADER : Range
        {
            public byte[] e_magic = new byte[2];                     // Magic number,"MZ"
            public byte[] e_cblp = new byte[2];                      // Bytes on last page of file
            public byte[] e_cp = new byte[2];                        // Pages in file
            public byte[] e_crlc = new byte[2];                      // Relocations
            public byte[] e_cparhdr = new byte[2];                   // Size of header in paragraphs
            public byte[] e_minalloc = new byte[2];                  // Minimum extra paragraphs needed
            public byte[] e_maxalloc = new byte[2];                  // Maximum extra paragraphs needed
            public byte[] e_ss = new byte[2];                        // Initial (relative) SS value
            public byte[] e_sp = new byte[2];                        // Initial SP value
            public byte[] e_csum = new byte[2];                      // Checksum
            public byte[] e_ip = new byte[2];                        // Initial IP value
            public byte[] e_cs = new byte[2];                        // Initial (relative) CS value
            public byte[] e_lfarlc = new byte[2];                    // File address of relocation table
            public byte[] e_ovno = new byte[2];                      // Overlay number
            public byte[] e_res = new byte[8];                       // Reserved words
            public byte[] e_oemid = new byte[2];                     // OEM identifier (for e_oeminfo)
            public byte[] e_oeminfo = new byte[2];                   // OEM information; e_oemid specific
            public byte[] e_res2 = new byte[20];                     // Reserved words
            public byte[] e_lfanew = new byte[4];                    // File address of new exe header

            public IMAGE_DOS_HEADER(uint start)
                : base(start)
            {
                int i = 0, j = 0;
                foreach (var f in this.GetType().GetFields())
                {
                    byte[] bytes = f.GetValue(this) as byte[];
                    for (j = 0; j < bytes.Length; j++, i++)
                        bytes[j] = Data[i];
                }

                this.Length = 64;
            }
        }

        //IMAGE_DOS_HEADER.e_lfanew-64 bytes
        public class DOS_STUB : Range
        {
            public DOS_STUB(uint start, uint length) : base(start, length) { }
        }
    }





    //PE Header 248bytes
    public class IMAGE_NT_HEADERS32 : Range
    {
        public byte[] Signature = new byte[4];                  //"PE\0\0"
        public IMAGE_FILE_HEADER FileHeader;
        public IMAGE_OPTIONAL_HEADER32 OptionalHeader;

        public IMAGE_NT_HEADERS32(uint start) : base(start) { Length = 4 + 20 + 224; }

        //COFF Header 20bytes
        public class IMAGE_FILE_HEADER : Range
        {
            public byte[] Machine = new byte[2];
            public byte[] NumberOfSections = new byte[2];
            public byte[] TimeDateStamp = new byte[4];
            public byte[] PointerToSymbolTable = new byte[4];
            public byte[] NumberOfSymbols = new byte[4];
            public byte[] SizeOfOptionalHeader = new byte[2];
            public byte[] Characteristics = new byte[2];

            public IMAGE_FILE_HEADER(uint start)
                : base(start)
            {
                Length = 20;
            }
        }

        //224bytes
        public class IMAGE_OPTIONAL_HEADER32 : Range
        {
            //
            // Standard fields.
            //
            public byte[] Magic = new byte[2];
            public byte[] MajorLinkerVersion = new byte[1];
            public byte[] MinorLinkerVersion = new byte[1];
            public byte[] SizeOfCode = new byte[4];
            public byte[] SizeOfInitializedData = new byte[4];
            public byte[] SizeOfUninitializedData = new byte[4];
            public byte[] AddressOfEntryPoint = new byte[4];
            public byte[] BaseOfCode = new byte[4];
            public byte[] BaseOfData = new byte[4];
            //
            // NT additional fields.
            //
            public byte[] ImageBase = new byte[4];
            public byte[] SectionAlignment = new byte[4];
            public byte[] FileAlignment = new byte[4];
            public byte[] MajorOperatingSystemVersion = new byte[2];
            public byte[] MinorOperatingSystemVersion = new byte[2];
            public byte[] MajorImageVersion = new byte[2];
            public byte[] MinorImageVersion = new byte[2];
            public byte[] MajorSubsystemVersion = new byte[2];
            public byte[] MinorSubsystemVersion = new byte[2];
            public byte[] Win32VersionValue = new byte[4];
            public byte[] SizeOfImage = new byte[4];
            public byte[] SizeOfHeaders = new byte[4];
            public byte[] CheckSum = new byte[4];
            public byte[] Subsystem = new byte[2];
            public byte[] DllCharacteristics = new byte[2];
            public byte[] SizeOfStackReserve = new byte[4];
            public byte[] SizeOfStackCommit = new byte[4];
            public byte[] SizeOfHeapReserve = new byte[4];
            public byte[] SizeOfHeapCommit = new byte[4];
            public byte[] LoaderFlags = new byte[4];
            public byte[] NumberOfRvaAndSizes = new byte[4];
            public IMAGE_DATA_DIRECTORY[] DataDirectory = new IMAGE_DATA_DIRECTORY[16];

            //8bytes
            public class IMAGE_DATA_DIRECTORY : Range
            {
                public byte[] VirtualAddress = new byte[4];
                public byte[] Size = new byte[4];

                public IMAGE_DATA_DIRECTORY(uint start)
                    : base(start)
                {
                    this.Length = 8;
                }
            }

            public IMAGE_OPTIONAL_HEADER32(uint start)
                : base(start)
            {
                Length = 224;
            }
        }
    }

    //40bytes IMAGE_FILE_HEADER.NumberOfSections
    public class IMAGE_SECTION_HEADER : Range
    {
        byte[] Name = new byte[8];
        byte[] VirtualSize = new byte[4];
        byte[] VirtualAddress = new byte[4];
        byte[] SizeOfRawData = new byte[4];
        byte[] PointerToRawData = new byte[4];
        byte[] PointerToRelocations = new byte[4];
        byte[] PointerToLinenumbers = new byte[4];
        byte[] NumberOfRelocations = new byte[2];
        byte[] NumberOfLinenumbers = new byte[2];
        byte[] Characteristics = new byte[4];

        public IMAGE_SECTION_HEADER(uint start)
            : base(start)
        {
            Length = 40;
        }
    }

    public class IMAGE_IMPORT_DESCRIPTOR
    {
        byte[] OriginalFirstThunk = new byte[4];             // 0 for terminating null import descriptor,RVA to original unbound IAT (PIMAGE_THUNK_DATA)
        byte[] TimeDateStamp = new byte[4];                  // 0 if not bound, -1 if bound, and real date\time stamp in IMAGE_DIRECTORY_ENTRY_BOUND_IMPORT (new BIND) O.W. date/time stamp of DLL bound to (Old BIND)
        byte[] ForwarderChain = new byte[4];                 // -1 if no forwarders
        byte[] Name = new byte[4];
        byte[] FirstThunk = new byte[4];                     // RVA to IAT (if bound this IAT has actual addresses)
    }

    public class IMAGE_IMPORT_BY_NAME
    {
        byte[] Hint = new byte[2];
        byte[] Name = new byte[1];
    }


}
