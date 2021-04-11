using System;
using System.Text;

namespace SistemaBlueddit.Protocol.Library
{
    public class HeaderHandler
    {
        public byte[] EncodeHeader(short command, char[] headerMethod, int dataLength)
        {
            var header = new byte[HeaderConstants.CommandLength + HeaderConstants.MethodLength + HeaderConstants.DataLength];
            Array.Copy(BitConverter.GetBytes(command), 0, header, 0, HeaderConstants.CommandLength);
            Array.Copy(Encoding.UTF8.GetBytes(headerMethod), 0, header, HeaderConstants.CommandLength, HeaderConstants.MethodLength);
            Array.Copy(BitConverter.GetBytes(dataLength), 0, header, HeaderConstants.CommandLength + HeaderConstants.MethodLength, HeaderConstants.DataLength);
            return header;
        }

        public Tuple<short, int> DecodeHeader(byte[] data)
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