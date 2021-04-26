using SistemaBlueddit.Domain;
using SistemaBlueddit.Protocol.Library;
using System;
using System.Net.Sockets;
using System.Text;

namespace SistemaBlueddit.Server.Logic
{
    public class FileLogic
    {
        public BluedditFile GetFile(Header header, NetworkStream networkStream)
        {
            var fileHandler = new FileHandler();
            var fileNameSize = header.FileNameLength;
            var fileSize = header.DataLength;

            var fileName = DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + "-" + Encoding.UTF8.GetString(Read(fileNameSize, networkStream));
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
                        var data = Read(lastPartSize, networkStream);
                        Array.Copy(data, 0, rawFileInMemory, offset, lastPartSize);
                        offset += lastPartSize;
                    }
                    else
                    {
                        var data = Read(HeaderConstants.MaxPacketSize, networkStream);
                        Array.Copy(data, 0, rawFileInMemory, offset, HeaderConstants.MaxPacketSize);
                        offset += HeaderConstants.MaxPacketSize;
                    }
                    currentPart++;
                }
                fileHandler.WriteFile(filePath, rawFileInMemory);

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

        private byte[] Read(int length, NetworkStream stream)
        {
            int dataReceived = 0;
            var data = new byte[length];
            while (dataReceived < length)
            {
                var received = stream.Read(data, dataReceived, length - dataReceived);
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