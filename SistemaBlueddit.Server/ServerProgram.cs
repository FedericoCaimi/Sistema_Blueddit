using System.Collections.Generic;
using SistemaBlueddit.Protocol.Library;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SistemaBlueddit.Domain;
using SistemaBlueddit.Server.Logic;
using System.Text;

namespace SistemaBlueddit.Server
{
    public class ServerProgram
    {
        private static bool _exit = false;
        public static UserLogic userLogic = new UserLogic();
        public static TopicLogic topicLogic = new TopicLogic();
        public static PostLogic postLogic = new PostLogic();
        public static FileLogic fileLogic = new FileLogic();

        static void Main(string[] args)
        {
            Console.WriteLine("Server esta iniciando...");
            var tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 50000);
            tcpListener.Start(10);

            var threadServer = new Thread(()=> ListenForConnections(tcpListener));
            threadServer.Start();

            while (!_exit)
            {
                Console.WriteLine("Bienvenido al Servidor del Sistema Blueddit");
                Console.WriteLine("0  - Cargar datos de prueba (si ya estan cargados se sobrescriben)");
                Console.WriteLine("1  - Listar clientes conectados");
                Console.WriteLine("2  - Listar todos los temas del sistema");
                Console.WriteLine("3  - Listar posts por tema");
                Console.WriteLine("4  - Listar posts por orden de creado");
                Console.WriteLine("5  - Listar posts por orden de creado y tema");
                Console.WriteLine("6  - Listar posts por tema y orden de creado");
                Console.WriteLine("7  - Mostrar un post especifico");
                Console.WriteLine("8  - Mostrar un tema o temas con mas publicaciones");
                Console.WriteLine("9  - Mostrar archivo asociado a un post");
                Console.WriteLine("10 - Mostrar listado de archivos");
                Console.WriteLine("99 - salir");
                var option = Console.ReadLine();
                switch (option)
                {
                    case "0":
                        var mockedTopics = LoadMockData.LoadTopicsMockData();
                        var mockedPosts = LoadMockData.LoadPostsMockData(mockedTopics);
                        topicLogic.ClearTopics();
                        postLogic.ClearPosts();
                        topicLogic.AddTopics(mockedTopics);
                        postLogic.AddPosts(mockedPosts);
                        break;
                    case "1":
                        userLogic.ShowUsers();
                        break;
                    case "2":
                        topicLogic.ShowTopics();
                        break;
                    case "3":
                        postLogic.ShowPostsByTopic();
                        break;
                    case "4":
                        postLogic.ShowPostsByDate();
                        break;
                    case "5":
                        postLogic.ShowPostsByDateAndTopic();
                        break;
                    case "6":
                        postLogic.ShowPostsByTopicAndDate();
                        break;
                    case "7":
                        Console.WriteLine("Nombre del post a mostrar:");
                        var postName = Console.ReadLine();
                        postLogic.ShowPostByName(postName);
                        break;
                    case "8":
                        var topics = topicLogic.Topics();
                        postLogic.ShowTopicsWithMorePosts(topics);
                        break;
                    case "9":
                        Console.WriteLine("Nombre del post del cual se desea obtener el archivo:");
                        var post = Console.ReadLine();
                        var file = postLogic.GetFileFromPostName(post);
                        if(file != null)
                        {
                            Console.WriteLine(file.PrintFile(false));
                        }
                        else
                        {
                            Console.WriteLine("El post no existe o no tiene archivos");
                        }
                        
                        break;
                    case "10":
                        Console.WriteLine("Desea filtar por tema? (s para confirmar)");
                        var confirm = Console.ReadLine();
                        var topicFiltrer = new List<Topic>();
                        if(confirm.Equals("s"))
                        {
                            Console.WriteLine("Elija los nombres de los temas a filtrar: (s para salir)");
                            var exitFilter = "";
                            while(!exitFilter.Equals("s"))
                            {
                               Console.WriteLine("Nombre del tema:");
                               var topicName = Console.ReadLine();
                               var topic = topicLogic.GetTopicByName(topicName);
                               if(topic == null){
                                   Console.WriteLine("El tema no existe");
                               }else
                               {
                                   topicFiltrer.Add(topic);
                               }
                            }
                        }
                        Console.WriteLine("Mostrar por:");
                        Console.WriteLine("1 - Fecha");
                        Console.WriteLine("2 - Nombre");
                        Console.WriteLine("3 - Tamaño");
                        switch (option)
                        {
                            case "1":
                                break;
                            case "2":
                                var fileName = Console.ReadLine();
                                postLogic.ShowFilesByNameAndTopics(fileName, topicFiltrer);
                                break;
                            case "3":
                                break;
                            default:
                                Console.WriteLine("Opcion invalida...");
                            break;
                        }
                        break;
                    case "99":
                        _exit = true;
                        tcpListener.Stop();
                        userLogic.CloseAll();
                        break;
                    default:
                        Console.WriteLine("Opcion invalida...");
                        break;
                }
            }
            
        }

        private static void ListenForConnections(TcpListener tcpListener)
        {
            while (!_exit)
            {
                try
                {
                    var acceptedClient = tcpListener.AcceptTcpClient();
                    var user = new User
                    {
                        StartConnection = DateTime.Now,
                        TcpClient = acceptedClient
                    };
                    userLogic.AddUser(user);
                    Console.WriteLine("Acepte una nueva conexion");
                    var threadClient = new Thread(() => HandleClient(user));
                    threadClient.Start();

                }
                catch (SocketException se)
                {
                    Console.WriteLine("El servidor está cerrándose...");
                    _exit = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            Console.WriteLine("Saliendo del listen...");
        }

        private static void HandleClient(User user)
        {
            var acceptedClient = user.TcpClient;
            try
            {
                while (!_exit)
                {
                    var networkStream = acceptedClient.GetStream();
                    var header = HeaderHandler.DecodeHeader(networkStream);
                    HeaderHandler.ValidateHeader(header, HeaderConstants.Request);
                    var serverResponse = "";
                    switch (header.Command)
                    {
                        case 01:
                            var topic = topicLogic.RecieveTopic(header, networkStream);
                            Console.WriteLine(topic.PrintTopic());
                            topicLogic.AddTopic(topic);
                            serverResponse = "El tema se ha creado con exito";
                            DataHandler.SendResponse(acceptedClient, serverResponse);
                            break;
                        case 02:
                            var post = postLogic.RecievePost(header, networkStream);
                            postLogic.ValidatePost(post);
                            topicLogic.ValidateTopics(post.Topics);
                            Console.WriteLine("Nombre de la publicacion: " + post.Name);
                            foreach (var postTopic in post.Topics)
                            {
                                Console.WriteLine(postTopic.PrintTopic());
                            }
                            postLogic.AddPost(post);
                            serverResponse = "El post se ha creado con exito";
                            DataHandler.SendResponse(acceptedClient, serverResponse);
                            break;
                        case 03:
                            var existingPost = postLogic.GetPostWithName(header, networkStream);
                            if (existingPost != null)
                            {
                                DataHandler.SendResponse(acceptedClient, "existe");
                                header = HeaderHandler.DecodeHeader(networkStream);
                                HeaderHandler.ValidateHeader(header, HeaderConstants.Request);
                                var bluedditFile = fileLogic.GetFile(header, networkStream);
                                Console.WriteLine(bluedditFile.FileName);
                                postLogic.AddFileToPost(bluedditFile, existingPost);
                                serverResponse = "El archivo se ha agregado al post con exito";
                                DataHandler.SendResponse(acceptedClient, serverResponse);
                            }
                            else
                            {
                                DataHandler.SendResponse(acceptedClient, "noexiste");
                            }
                            break;
                        default:
                            Console.WriteLine("Opcion invalida...");
                            break;
                    }
                }
                Console.WriteLine("Sali del while...");
            }
            catch (Exception e)
            {
                Console.WriteLine("Borrando el cliente. " + e.Message);
                userLogic.RemoveUser(user);
            }
            Console.WriteLine("El cliente con hora de conexion "+ user.StartConnection.ToString()+" se desconecto");
        }
    }
}
