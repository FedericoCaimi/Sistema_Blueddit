namespace SistemaBlueddit.Protocol.Library
{
    public static class HeaderConstants
    {
        public const int CommandLength = 2;
        public const int DataLength = 4;
        public const int MethodLength = 3;
        public const string Request = "REQ";
        public const string Response = "RES";
        public const int FileNameLength = 4;
        public const int MaxPacketSize = 32768;
        public const long MaxFileSize = 104857600;
    }
}