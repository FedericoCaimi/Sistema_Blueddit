using System;

namespace SistemaBlueddit.Domain
{
    public class Header
    {
        public short Command { get; set; }
        public int DataLength { get; set; }
        public string HeaderMethod { get; set; }
        public int FileNameLength { get; set; }

    }
}
