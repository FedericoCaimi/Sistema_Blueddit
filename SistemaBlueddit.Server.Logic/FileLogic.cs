using SistemaBlueddit.Domain;
using SistemaBlueddit.Protocol.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace SistemaBlueddit.Server.Logic
{
    public class FileLogic
    {

        public FileLogic()
        {
        }

        public void GetFile(Header header, NetworkStream networkStream)
        {
            var fileHandler = new FileHandler();
            //var networkStream = client.GetStream();
            //var header = Read(ProtocolHelper.GetLength(),networkStream);
            var fileNameSize = header.FileNameLength;//BitConverter.ToInt32(header,0);
            var fileSize = header.DataLength;//BitConverter.ToInt32(header,ProtocolSpecification.FileNameLength);

            var fileName = Encoding.UTF8.GetString(Read(fileNameSize, networkStream));

            var parts = fileHandler.GetFileParts(fileSize);
            var offset = 0;
            var currentPart = 1;

            var rawFileInMemory = new byte[fileSize];
            
            while (fileSize > offset)
            {
                if (currentPart == parts)
                {
                    var lastPartSize = fileSize - offset;
                    var data = Read(lastPartSize, networkStream);
                    Array.Copy(data,0,rawFileInMemory,offset,lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    var data = Read(HeaderConstants.MaxPacketSize, networkStream);
                    Array.Copy(data,0,rawFileInMemory,offset,HeaderConstants.MaxPacketSize);
                    offset += HeaderConstants.MaxPacketSize;
                }
                currentPart++;
            }
            fileHandler.WriteFile("./Archivos/"+fileName,rawFileInMemory);

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
