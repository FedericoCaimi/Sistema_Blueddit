using Microsoft.Extensions.Configuration;
using SistemaBlueddit.Client.Logic;
using System;
using System.IO;

namespace SistemaBlueddit.Client
{
    public class ClientProgram
    {
        public static TopicLogic topicLogic = new TopicLogic();
        public static PostLogic postLogic = new PostLogic();
        public static FileLogic fileLogic = new FileLogic();
        public static ResponseLogic responseLogic = new ResponseLogic();
        public static LocalRequestHandler localRequestHandler = new LocalRequestHandler(topicLogic, postLogic, fileLogic, responseLogic);
        public static IConfigurationRoot configuration;

        static void Main(string[] args)
        {
            ConfigureServices();

            var clientIP = configuration.GetSection("clientIP").Value;
            var serverIP = configuration.GetSection("serverIP").Value;
            var serverPort = Convert.ToInt32(configuration.GetSection("port").Value);

            localRequestHandler.HandleLocalRequests(clientIP, serverIP, serverPort);
        }

        private static void ConfigureServices()
        {
            // Build configuration
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();
        }
    }
}