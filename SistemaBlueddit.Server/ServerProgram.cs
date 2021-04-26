using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SistemaBlueddit.Domain;
using SistemaBlueddit.Server.Logic;
using SistemaBlueddit.Server.Logic.Interfaces;

namespace SistemaBlueddit.Server
{
    public class ServerProgram
    {
        public static LocalRequestHandler localRequestHandler;
        public static ClientHandler clientHandler;
        public static IConfigurationRoot configuration;
        public static ServerState serverState = new ServerState();

        static void Main(string[] args)
        {
            Console.WriteLine("Server esta iniciando...");

            BuildConfig();

            var serverIP = configuration.GetSection("serverIP").Value;
            var port = Convert.ToInt32(configuration.GetSection("port").Value);

            var tcpListener = new TcpListener(IPAddress.Parse(serverIP), port);
            tcpListener.Start(10);

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<IUserLogic, UserLogic>();
                    services.AddTransient<ITopicLogic, TopicLogic>();
                    services.AddTransient<IPostLogic, PostLogic>();
                    services.AddTransient<IFileLogic, FileLogic>();
                })
                .Build();

            var userLogic = ActivatorUtilities.CreateInstance<UserLogic>(host.Services);
            var topicLogic = ActivatorUtilities.CreateInstance<TopicLogic>(host.Services);
            var postLogic = ActivatorUtilities.CreateInstance<PostLogic>(host.Services);
            var fileLogic = ActivatorUtilities.CreateInstance<FileLogic>(host.Services);

            localRequestHandler = new LocalRequestHandler(userLogic, topicLogic, postLogic);
            clientHandler = new ClientHandler(topicLogic, postLogic, fileLogic, userLogic);

            serverState.IsServerTerminated = false;

            var threadServer = new Thread(()=> ListenForConnections(tcpListener, serverState));
            threadServer.Start();

            while (!serverState.IsServerTerminated)
            {
                localRequestHandler.HandleLocalRequests(serverState);
            }
            tcpListener.Stop();
        }

        private static void ListenForConnections(TcpListener tcpListener, ServerState serverState)
        {
            while (!serverState.IsServerTerminated)
            {
                try
                {
                    var acceptedClient = tcpListener.AcceptTcpClient();
                    var threadClient = new Thread(() => clientHandler.HandleClient(acceptedClient, serverState));
                    threadClient.Start();
                }
                catch (SocketException se)
                {
                    Console.WriteLine("El servidor está cerrándose...");
                    serverState.IsServerTerminated = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
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
