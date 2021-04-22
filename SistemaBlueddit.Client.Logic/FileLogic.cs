using SistemaBlueddit.Domain;
using SistemaBlueddit.Protocol.Library;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace SistemaBlueddit.Client.Logic
{
    public class FileLogic
    {
        public FileLogic()
        {
            
        }
        public void SendFile(string option, string path, TcpClient connectedClient)
        {
            var fileHandler = new FileHandler();
            //var fileStreamHandler = new FileStreamHandler();
            var fileSize = fileHandler.GetFileSize(path);
            var fileName = fileHandler.GetFileName(path);
            var fileNameLength = fileName.Length;
            //var header = ProtocolHelper.CreateHeader(fileName, fileSize);
            var command = Convert.ToInt16(option);
            var header = HeaderHandler.EncodeHeader(HeaderConstants.Request, command, fileSize, fileNameLength);
            var connectionStream = connectedClient.GetStream();
            
            Console.WriteLine($"FileName is: {fileName}, file size is: {fileSize}");
            
            connectionStream.Write(header);
            connectionStream.Write(Encoding.UTF8.GetBytes(fileName));

            var rawFile = fileHandler.ReadFile(path);
            var parts = fileHandler.GetFileParts(fileSize);

            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                Console.WriteLine($"Voy a enviar parte {currentPart} de {parts}");
                if (currentPart == parts)
                {
                    var lastPartSize = fileSize - offset;
                    var dataToSend = new byte[lastPartSize];
                    Array.Copy(rawFile,offset,dataToSend,0,lastPartSize);
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
    }
}
