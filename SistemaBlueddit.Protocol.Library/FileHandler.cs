using System;
using System.IO;

namespace SistemaBlueddit.Protocol.Library
{
    public class FileHandler
    {
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public string GetFileName(string path)
        {
            if (FileExists(path))
            {
                return new FileInfo(path).Name;
            }

            throw new Exception("File no existe!");
        }

        public long GetFileSize(string path)
        {
            if (FileExists(path))
            {
                return new FileInfo(path).Length;
            }

            throw new Exception("File no existe!");
        }
    
        public byte[] ReadFile(string path)
        {
            return File.ReadAllBytes(path);
        }

        public void WriteFile(string path, byte[] data)
        {
            File.WriteAllBytes(path,data);
        }

        public long GetFileParts(long filesize)
        {
            var parts = filesize / HeaderConstants.MaxPacketSize;
            return parts * HeaderConstants.MaxPacketSize == filesize ? parts : parts + 1;
        }
    }
}