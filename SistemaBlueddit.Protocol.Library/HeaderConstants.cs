namespace SistemaBlueddit.Protocol.Library
{
    public static class HeaderConstants
    {
        public static int CommandLength = 2;
        public static int DataLength = 4;
        public static int MethodLength = 3;
        public static char[] Request = "REQ".ToCharArray();
        public static char[] Response = "RES".ToCharArray();
    }
}