using System;
using System.Text;

namespace SistemaBlueddit.Protocol.Library
{

        /// XXXX YYYY
        /// XXXX-> Largo del nombre del archivo, YYYY -> Largo del file
        /// ZZZZZZZZZZZ... -> Nombre del archivo
        /// N Segmentos (YYYY / MaxPacketSize) -> Cada Segmento mide MaxPacketSize o menos
        ///
        /// Crear nuevo post
        /// REQ05XXXX <Titulo de Post>#<Contenido del post>
        ///
        /// Listar ususarios conectados
        /// REQ010000
        ///  

        /// REQ05XXXXYYYY
    public class HeaderHandler
    {
        /*public byte[] EncodeHeader(short command, char[] headerMethod, int dataLength)
        {
            var header = new byte[HeaderConstants.CommandLength + HeaderConstants.MethodLength + HeaderConstants.DataLength];
            Array.Copy(BitConverter.GetBytes(command), 0, header, 0, HeaderConstants.CommandLength);
            Array.Copy(Encoding.UTF8.GetBytes(headerMethod), 0, header, HeaderConstants.CommandLength, HeaderConstants.MethodLength);
            Array.Copy(BitConverter.GetBytes(dataLength), 0, header, HeaderConstants.CommandLength + HeaderConstants.MethodLength, HeaderConstants.DataLength);
            return header;
        }*/
        public byte[] EncodeHeader(short command, string headerMethod, int dataLength, int fileNameLength)
        {
            byte[] method = System.Text.Encoding.UTF8.GetBytes(headerMethod);
            var header = new byte[HeaderConstants.CommandLength + HeaderConstants.MethodLength + HeaderConstants.DataLength];
            Array.Copy(BitConverter.GetBytes(command), 0, header, 0, HeaderConstants.CommandLength);
            Array.Copy(method, 0, header, HeaderConstants.CommandLength, HeaderConstants.MethodLength);
            Array.Copy(BitConverter.GetBytes(dataLength), 0, header, HeaderConstants.CommandLength + HeaderConstants.MethodLength, HeaderConstants.DataLength);
            Array.Copy(BitConverter.GetBytes(fileNameLength), 0, header, HeaderConstants.CommandLength + HeaderConstants.MethodLength+HeaderConstants.DataLength, HeaderConstants.FileNameLength);
            return header;
        }

        public Header DecodeHeader(byte[] data)
        {
            try
            {
                short command = BitConverter.ToInt16(data, 0);
                int dataLength = BitConverter.ToInt32(data, HeaderConstants.CommandLength);
                return new Tuple<short, int>(command, dataLength);

            }
            catch (Exception e)
            {
                Console.WriteLine("Error decoding data: " + e.Message);
                return null;
            }
        }
    }
}