using SistemaBlueddit.Protocol.Library;
using System;
using System.Net.Sockets;
using System.Text;

namespace SistemaBlueddit.Client.Logic
{
    public class FileLogic
    {
        public void SendFile(string option, string path, TcpClient connectedClient)
        {
            var fileHandler = new FileHandler();

            var fileSize = fileHandler.GetFileSize(path);
            var fileName = fileHandler.GetFileName(path);
            var fileNameLength = fileName.Length;

            var command = Convert.ToInt16(option);
            var header = HeaderHandler.EncodeHeader(HeaderConstants.Request, command, fileSize, fileNameLength);
            var connectionStream = connectedClient.GetStream();
            
            if (fileSize < HeaderConstants.MaxFileSize)
            {
                connectionStream.Write(header);
                connectionStream.Write(Encoding.UTF8.GetBytes(fileName));

                var rawFile = fileHandler.ReadFile(path);
                var parts = fileHandler.GetFileParts(fileSize);

                long offset = 0;
                long currentPart = 1;

                while (fileSize > offset)
                {
                    if (currentPart == parts)
                    {
                        var lastPartSize = fileSize - offset;
                        var dataToSend = new byte[lastPartSize];
                        Array.Copy(rawFile, offset, dataToSend, 0, lastPartSize);
                        offset += lastPartSize;
                        connectionStream.Write(dataToSend);
                    }
                    else
                    {
                        var dataToSend = new byte[HeaderConstants.MaxPacketSize];
                        Array.Copy(rawFile, offset, dataToSend, 0, HeaderConstants.MaxPacketSize);
                        offset += HeaderConstants.MaxPacketSize;
                        connectionStream.Write(dataToSend);
                    }
                    currentPart++;
                }
            }
            else
            {
                throw new Exception("Archivo demasiado grande para ser enviado");
            }
        }
    }
}
