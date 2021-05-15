using SistemaBlueddit.Domain;
using SistemaBlueddit.Protocol.Library;
using SistemaBlueddit.Server.Logic.Interfaces;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SistemaBlueddit.Server.Logic
{
    public class FileLogic: IFileLogic
    {
        public async Task<BluedditFile> GetFileAsync(Header header, NetworkStream networkStream)
        {
            var fileHandler = new FileHandler();
            var fileNameSize = header.FileNameLength;
            var fileSize = header.DataLength;

            var fileName = DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + "-" + Encoding.UTF8.GetString(await ReadAsync(fileNameSize, networkStream));
            var filePath = "./Files/" + fileName;
            var parts = fileHandler.GetFileParts(fileSize);
            var offset = 0;
            var currentPart = 1;

            var rawFileInMemory = new byte[fileSize];
            
            if(fileSize < HeaderConstants.MaxFileSize)
            {
                while (fileSize > offset)
                {
                    if (currentPart == parts)
                    {
                        var lastPartSize = fileSize - offset;
                        var data = await ReadAsync(lastPartSize, networkStream);
                        Array.Copy(data, 0, rawFileInMemory, offset, lastPartSize);
                        offset += lastPartSize;
                    }
                    else
                    {
                        var data = await ReadAsync(HeaderConstants.MaxPacketSize, networkStream);
                        Array.Copy(data, 0, rawFileInMemory, offset, HeaderConstants.MaxPacketSize);
                        offset += HeaderConstants.MaxPacketSize;
                    }
                    currentPart++;
                }
                await fileHandler.WriteFileAsync(filePath, rawFileInMemory);

                return new BluedditFile
                {
                    FileName = fileName,
                    FilePath = filePath,
                    FileSize = fileSize,
                    CreationDate = DateTime.Now
                };
            }
            else
            {
                throw new Exception("Archivo demasiado grande para ser recibido");
            }
        }

        private async Task<byte[]> ReadAsync(int length, NetworkStream stream)
        {
            int dataReceived = 0;
            var data = new byte[length];
            while (dataReceived < length)
            {
                var received = await stream.ReadAsync(data, dataReceived, length - dataReceived);
                if (received == 0)
                {
                    throw new Exception("La conexion se cayo");
                }

                dataReceived += received;
            }
            return data;    
        }
    }
}