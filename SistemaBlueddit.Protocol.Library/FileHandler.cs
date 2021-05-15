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
    
        /*public async Task<byte[]> ReadFileAsync(string path)
        {
            return await File.ReadAllBytesAsync(path);
        }

        public async Task WriteFileAsync(string path, byte[] data)
        {
            await File.WriteAllBytesAsync(path,data);
        }*/

        public async Task<byte[]> ReadFileAsync(string path, long offset, int length)
        {
            var data = new byte[length];

            using (var fs = new FileStream(path, FileMode.Open))
            {
                fs.Position = offset;
                var bytesRead = 0;
                while (bytesRead < length)
                {
                    var read = await fs.ReadAsync(data, bytesRead, length - bytesRead);
                    if (read == 0)
                    {
                        throw new Exception("No se puede enviar el archivo");
                    }
                    bytesRead += read;
                }
            }

            return data;
        }

        public async Task WriteFileAsync(string path, byte[] data)
        {
            if (File.Exists(path))
            {
                using (var fs = new FileStream(path, FileMode.Append))
                {
                    await fs.WriteAsync(data, 0, data.Length);
                }
            }
            else
            {
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    await fs.WriteAsync(data, 0, data.Length);
                }
            }
        }
        public long GetFileParts(long filesize)
        {
            var parts = filesize / HeaderConstants.MaxPacketSize;
            return parts * HeaderConstants.MaxPacketSize == filesize ? parts : parts + 1;
        }
    }
}