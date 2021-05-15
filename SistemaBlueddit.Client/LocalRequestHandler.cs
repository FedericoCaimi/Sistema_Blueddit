using SistemaBlueddit.Client.Logic.Interfaces;
using SistemaBlueddit.Domain;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SistemaBlueddit.Client
{
    public class LocalRequestHandler
    {
        private ITopicLogic _topicLogic;
        private IPostLogic _postLogic;
        private IFileLogic _fileLogic;
        private IResponseLogic _responseLogic;
        private bool exit;

        public LocalRequestHandler(ITopicLogic topicLogic, IPostLogic postLogic, IFileLogic fileLogic, IResponseLogic responseLogic)
        {
            _topicLogic = topicLogic;
            _postLogic = postLogic;
            _fileLogic = fileLogic;
            _responseLogic = responseLogic;
        }

        public async Task HandleLocalRequestsAsync(string clientIP, string serverIP, int serverPort)
        {
            try
            {
                Console.WriteLine("Cliente se esta iniciando");

                var tcpClient = new TcpClient(new IPEndPoint(IPAddress.Parse(clientIP), 0));
                await tcpClient.ConnectAsync(IPAddress.Parse(serverIP), serverPort);

                Console.WriteLine("Cliente se conectó al servidor.");

                while (!exit)
                {
                    Console.WriteLine("***************************************");
                    Console.WriteLine("*   Bienvenido al Sistema Blueddit    *");
                    Console.WriteLine("***************************************");
                    Console.WriteLine("* 1 - Crear un nuevo tema             *");
                    Console.WriteLine("* 2 - Crear un nuevo post             *");
                    Console.WriteLine("* 3 - Subir archivo a una publicacion *");
                    Console.WriteLine("* 4 - Dar de baja un tema             *");
                    Console.WriteLine("* 5 - Modificar un tema               *");
                    Console.WriteLine("* 6 - Dar de baja un post             *");
                    Console.WriteLine("* 7 - Modificar un post               *");
                    Console.WriteLine("* 99 - salir                          *");
                    Console.WriteLine("***************************************");
                    var option = Console.ReadLine();
                    switch (option)
                    {
                        case "1":
                            var topicToCreate = GetTopicToCreate();
                            await _topicLogic.CreateAsync(tcpClient, option, topicToCreate);
                            var CreateAsyncTopicResponse = await _responseLogic.HandleResponseAsync(tcpClient);
                            Console.WriteLine(CreateAsyncTopicResponse.ServerResponse);
                            break;
                        case "2":
                            var postToCreate = GetPostToCreate();
                            await _postLogic.CreateAsync(tcpClient, option, postToCreate);
                            var CreateAsyncPostResponse = await _responseLogic.HandleResponseAsync(tcpClient);
                            Console.WriteLine(CreateAsyncPostResponse.ServerResponse);
                            break;
                        case "3":
                            Console.WriteLine("Escriba el nombre del la publicacion");
                            var postName = Console.ReadLine();
                            var postToUploadFile = new Post
                            {
                                Name = postName,
                                Topics = new List<Topic>()
                            };
                            await _postLogic.ExistsAsync(tcpClient, option, postToUploadFile);
                            var existsPostResponse = await _responseLogic.HandleResponseAsync(tcpClient);
                            if (existsPostResponse.ServerResponse.Equals("existe"))
                            {
                                Console.WriteLine("Escriba el path completo del archivo a subir");
                                var path = Console.ReadLine();
                                try{
                                    await _fileLogic.SendFileAsync(option, path, tcpClient);
                                    var fileSendConfirmation = await _responseLogic.HandleResponseAsync(tcpClient);
                                    Console.WriteLine(fileSendConfirmation.ServerResponse);
                                }catch(Exception e){
                                    Console.WriteLine(e.Message);
                                }
                            }
                            else
                            {
                                Console.WriteLine("No existe el nombre de la publicacion ingresada");
                            }
                            break;
                        case "4":
                            Console.WriteLine("Escriba el nombre del tema a dar de baja");
                            var topicName = Console.ReadLine();
                            var topicToDelete = new Topic
                            {
                                Name = topicName
                            };
                            await _topicLogic.DeleteAsync(tcpClient, option, topicToDelete);
                            var topicDeleteResponse = await _responseLogic.HandleResponseAsync(tcpClient);
                            Console.WriteLine(topicDeleteResponse.ServerResponse);
                            break;
                        case "5":
                            var topicToModify = GetTopicToCreate();
                            await _topicLogic.UpdateAsync(tcpClient, option, topicToModify);
                            var topicModifiedResponse = await _responseLogic.HandleResponseAsync(tcpClient);
                            Console.WriteLine(topicModifiedResponse.ServerResponse);
                            break;
                        case "6":
                            Console.WriteLine("Escriba el nombre del post a dar de baja");
                            var postNameToDelete = Console.ReadLine();
                            var postToDelete = new Post
                            {
                                Name = postNameToDelete,
                                Topics = new List<Topic>()
                            };
                            await _postLogic.DeleteAsync(tcpClient, option, postToDelete);
                            var postDeletedResponse = await _responseLogic.HandleResponseAsync(tcpClient);
                            Console.WriteLine(postDeletedResponse.ServerResponse);
                            break;
                        case "7":
                            var postToModify = GetPostToCreate();
                            await _postLogic.UpdateAsync(tcpClient, option, postToModify);
                            var postModifiedResponse = await _responseLogic.HandleResponseAsync(tcpClient);
                            Console.WriteLine(postModifiedResponse.ServerResponse);
                            break;
                        case "99":
                            exit = true;
                            tcpClient.GetStream().Close();
                            tcpClient.Close();
                            break;
                        default:
                            Console.WriteLine("Opcion invalida...");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Ha habido un error: " + e.Message);
                Console.WriteLine("Se perdió la conexión con el servidor.");
            }
        }

        public Post GetPostToCreate()
        {
            var newPost = new Post();
            var exit = false;
            var postTopics = new List<Topic>();
            Console.WriteLine("Nombre del post:");
            var name = Console.ReadLine();
            Console.WriteLine("Contenido del post:");
            var content = Console.ReadLine();
            Console.WriteLine("Agregar temas. Cuando termine de agregar temas ingrese la letra s:");
            while (!exit)
            {
                Console.WriteLine("Ingrese el nombre del tema");
                var topicName = Console.ReadLine();
                if (topicName.Equals("s"))
                {
                    exit = true;
                }
                else
                {
                    if (postTopics.Exists(topic => topic.Name == topicName))
                    {
                        Console.WriteLine("Nombre del tema repetido");
                    }
                    else
                    {
                        var topic = new Topic
                        {
                            Name = topicName
                        };
                        postTopics.Add(topic);
                    }
                }
            };
            newPost.Name = name;
            newPost.Content = content;
            newPost.Topics = postTopics;
            newPost.CreationDate = DateTime.Now;
            return newPost;
        }

        public Topic GetTopicToCreate()
        {
            Console.WriteLine("Nombre del tema:");
            var name = Console.ReadLine();
            Console.WriteLine("Descripcion:");
            var description = Console.ReadLine();

            var topic = new Topic
            {
                Name = name,
                Description = description
            };
            return topic;
        }
    }
}
