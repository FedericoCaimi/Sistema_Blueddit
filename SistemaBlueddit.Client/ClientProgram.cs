using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SistemaBlueddit.Client.Logic;
using SistemaBlueddit.Client.Logic.Interfaces;
using System;
using System.IO;

namespace SistemaBlueddit.Client
{
    public class ClientProgram
    {
        public static LocalRequestHandler localRequestHandler;
        public static IConfigurationRoot configuration;

        static void Main(string[] args)
        {
            BuildConfig();

            var clientIP = configuration.GetSection("clientIP").Value;
            var serverIP = configuration.GetSection("serverIP").Value;
            var serverPort = Convert.ToInt32(configuration.GetSection("port").Value);

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<IResponseLogic, ResponseLogic>();
                    services.AddTransient<ITopicLogic, TopicLogic>();
                    services.AddTransient<IPostLogic, PostLogic>();
                    services.AddTransient<IFileLogic, FileLogic>();
                })
                .Build();

            var responseLogic = ActivatorUtilities.CreateInstance<ResponseLogic>(host.Services);
            var topicLogic = ActivatorUtilities.CreateInstance<TopicLogic>(host.Services);
            var postLogic = ActivatorUtilities.CreateInstance<PostLogic>(host.Services);
            var fileLogic = ActivatorUtilities.CreateInstance<FileLogic>(host.Services);

            localRequestHandler = new LocalRequestHandler(topicLogic, postLogic, fileLogic, responseLogic);

            localRequestHandler.HandleLocalRequests(clientIP, serverIP, serverPort);
        }

        private static void BuildConfig()
        {
            // Build configuration
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();
        }
    }
}