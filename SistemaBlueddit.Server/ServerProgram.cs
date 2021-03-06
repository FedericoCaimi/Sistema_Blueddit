using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        public static void Main(string[] args)
        {
            Console.WriteLine("Server esta iniciando...");

            BuildConfig();

            var serverIP = configuration.GetSection("serverIP").Value;
            var port = Convert.ToInt32(configuration.GetSection("port").Value);

            var tcpListener = new TcpListener(IPAddress.Parse(serverIP), port);
            tcpListener.Start(10);

            var host = Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IRabbitMQMessageLogic, RabbitMQMessageLogic>();
                    services.AddSingleton<IUserLogic, UserLogic>();
                    services.AddSingleton<ITopicLogic, TopicLogic>();
                    services.AddSingleton<IPostLogic, PostLogic>();
                    services.AddSingleton<IFileLogic, FileLogic>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .Build();
            
            _ = HandleCustomRequestsAsync(host, tcpListener);

            host.Run();
        
        }

        private static async Task ListenForConnectionsAsync(TcpListener tcpListener, ServerState serverState)
        {
            while (!serverState.IsServerTerminated)
            {
                try
                {
                    var acceptedClient = await tcpListener.AcceptTcpClientAsync();
                    _ = clientHandler.HandleClientAsync(acceptedClient, serverState);
                }
                catch (SocketException se)
                {
                    Console.WriteLine("El servidor est?? cerr??ndose...");
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

        public static Task HandleCustomRequestsAsync(IHost host, TcpListener tcpListener)
        {
            return Task.Run(() => 
            {
                var messageLogic = host.Services.GetRequiredService<IRabbitMQMessageLogic>();
                var userLogic = host.Services.GetRequiredService<IUserLogic>();
                var topicLogic = host.Services.GetRequiredService<ITopicLogic>();
                var postLogic = host.Services.GetRequiredService<IPostLogic>();
                var fileLogic = host.Services.GetRequiredService<IFileLogic>();

                localRequestHandler = new LocalRequestHandler(userLogic, topicLogic, postLogic, messageLogic);
                clientHandler = new ClientHandler(topicLogic, postLogic, fileLogic, userLogic, messageLogic);

                serverState.IsServerTerminated = false;

                _ = ListenForConnectionsAsync(tcpListener, serverState);

                while (!serverState.IsServerTerminated)
                {
                    localRequestHandler.HandleLocalRequests(serverState);
                }
                tcpListener.Stop();
                messageLogic.DisposeConnections();
            });
        }
            
    }
}
