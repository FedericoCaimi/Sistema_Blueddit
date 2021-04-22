using SistemaBlueddit.Domain;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace SistemaBlueddit.Protocol.Library
{
    public static class HeaderHandler
    {
        public static byte[] EncodeHeader(string headerMethod, short command, long dataLength, int fileNameLength)
        {
            var method = Encoding.UTF8.GetBytes(headerMethod);
            var header = new byte[HeaderConstants.MethodLength + HeaderConstants.CommandLength + HeaderConstants.DataLength + HeaderConstants.FileNameLength];
            Array.Copy(method, 0, header, 0, HeaderConstants.MethodLength);
            Array.Copy(BitConverter.GetBytes(command), 0, header, HeaderConstants.MethodLength, HeaderConstants.CommandLength);
            Array.Copy(BitConverter.GetBytes(dataLength), 0, header, HeaderConstants.MethodLength + HeaderConstants.CommandLength, HeaderConstants.DataLength);
            Array.Copy(BitConverter.GetBytes(fileNameLength), 0, header, HeaderConstants.MethodLength + HeaderConstants.CommandLength + HeaderConstants.DataLength, HeaderConstants.FileNameLength);
            return header;
        }

        public static Header DecodeHeader(NetworkStream stream)
        {
            try
            {
                var headerSize = HeaderConstants.MethodLength + HeaderConstants.CommandLength + HeaderConstants.DataLength + HeaderConstants.FileNameLength;
                var data = new byte[headerSize];
                stream.Read(data, 0, headerSize);
                var hasAllZeroes = data.All(singleByte => singleByte == 0);
                if(hasAllZeroes)
                {   
                    throw new Exception("El header vino vacio se cierra la conexion");
                }
                var headerMethodBytes = new byte[HeaderConstants.MethodLength];
                Array.Copy(data, 0, headerMethodBytes, 0, HeaderConstants.MethodLength);
                var headerMethod = Encoding.Default.GetString(headerMethodBytes);
                var command = BitConverter.ToInt16(data, HeaderConstants.MethodLength);                
                var dataLength = BitConverter.ToInt32(data, HeaderConstants.MethodLength + HeaderConstants.CommandLength);
                var fileNameLength = BitConverter.ToInt32(data, HeaderConstants.MethodLength + HeaderConstants.CommandLength + HeaderConstants.DataLength);
                var header = new Header
                {
                    Command = command,
                    DataLength = dataLength,
                    FileNameLength = fileNameLength,
                    HeaderMethod = headerMethod
                };
                return header;

            }
            catch (Exception e)
            {
                throw new Exception("Error decodificando datos");
            }
        }

        public static void ValidateHeader(Header header, string headerMethod, short? command = null)
        {
            if (!(header.HeaderMethod.Equals(headerMethod) && (command.HasValue ? header.Command == command : true)))
            {
                throw new Exception("Header invalido");
            }
        }
    }
}