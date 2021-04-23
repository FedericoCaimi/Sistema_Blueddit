using System;

namespace SistemaBlueddit.Domain
{
    public class BluedditFile
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }

        public int FileSize { get; set; }

        public DateTime CreationDate { get; set; }

        public string PrintFile(bool addTabBeforeField)
        {
            var tab = addTabBeforeField ? "\t" : "";
            return $"{tab}FileName: {FileName}\n{tab}FilePath: {FilePath}\n{tab}FileSize: {FileSize}\n{tab}CreationDate: {CreationDate:yyyy-MM-dd HH:mm:ss}";
        }
    }
}