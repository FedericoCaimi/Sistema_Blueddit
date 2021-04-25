using SistemaBlueddit.Client.Logic;

namespace SistemaBlueddit.Client
{
    public class ClientProgram
    {
        public static TopicLogic topicLogic = new TopicLogic();
        public static PostLogic postLogic = new PostLogic();
        public static FileLogic fileLogic = new FileLogic();
        public static ResponseLogic responseLogic = new ResponseLogic();
        public static LocalRequestHandler localRequestHandler = new LocalRequestHandler(topicLogic, postLogic, fileLogic, responseLogic);

        static void Main(string[] args)
        {
            localRequestHandler.HandleLocalRequests();
        }
    }
}