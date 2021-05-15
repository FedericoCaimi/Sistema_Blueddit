using System;
using System.IO;
using System.Threading.Tasks;

namespace SistemaBlueddit.Protocol.Library
{
    public class FileHandler
    {
        private bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public string GetFileName(string path)
        {
            if (FileExists(path))
            {
                return new FileInfo(path).Name;
            }

            throw new Exception("El archivo seleccionado no Existe.");
        }

        public long GetFileSize(string path)
        {
            if (FileExists(path))
            {
                return new FileInfo(path).Length;
            }

            throw new Exception("El archivo seleccionado no Existe.");
        }
    
        public async Task<byte[]> ReadFileAsync(string path)
        {
            return await File.ReadAllBytesAsync(path);
        }

        public async Task WriteFileAsync(string path, byte[] data)
        {
            await File.WriteAllBytesAsync(path,data);
        }

        public long GetFileParts(long filesize)
        {
            var parts = filesize / HeaderConstants.MaxPacketSize;
            return parts * HeaderConstants.MaxPacketSize == filesize ? parts : parts + 1;
        }
    }
}