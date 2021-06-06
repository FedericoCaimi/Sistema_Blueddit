using SistemaBlueddit.Domain;
using SistemaBlueddit.Server.Logic.Interfaces;
using System;
using System.Collections.Generic;

namespace SistemaBlueddit.Server
{
    public class LocalRequestHandler
    {
        private IUserLogic _userLogic;

        private ITopicLogic _topicLogic;

        private IPostLogic _postLogic;

        private IRabbitMQMessageLogic _messageLogic;

        public LocalRequestHandler(IUserLogic userLogic, ITopicLogic topicLogic, IPostLogic postLogic, IRabbitMQMessageLogic messageLogic)
        {
            _userLogic = userLogic;
            _topicLogic = topicLogic;
            _postLogic = postLogic;
            _messageLogic = messageLogic;
        }

        public void HandleLocalRequests(ServerState serverState)
        {
            Console.WriteLine("**********************************************************************");
            Console.WriteLine("*        Bienvenido al Servidor del Sistema Blueddit                 *");
            Console.WriteLine("**********************************************************************");
            Console.WriteLine("* 0  - Cargar datos de prueba (si ya estan cargados se sobrescriben) *");
            Console.WriteLine("* 1  - Listar clientes conectados                                    *");
            Console.WriteLine("* 2  - Listar todos los temas del sistema                            *");
            Console.WriteLine("* 3  - Listar posts por tema                                         *");
            Console.WriteLine("* 4  - Listar posts por orden de creado                              *");
            Console.WriteLine("* 5  - Listar posts por orden de creado y tema                       *");
            Console.WriteLine("* 6  - Listar posts por tema y orden de creado                       *");
            Console.WriteLine("* 7  - Mostrar un post especifico                                    *");
            Console.WriteLine("* 8  - Mostrar un tema o temas con mas publicaciones                 *");
            Console.WriteLine("* 9  - Mostrar archivo asociado a un post                            *");
            Console.WriteLine("* 10 - Mostrar listado de archivos                                   *");
            Console.WriteLine("* 99 - salir                                                         *");
            Console.WriteLine("**********************************************************************");
            var option = Console.ReadLine();
            switch (option)
            {
                case "0":
                    var mockedTopics = LoadMockData.LoadTopicsMockData();
                    var mockedPosts = LoadMockData.LoadPostsMockData(mockedTopics);
                    _topicLogic.Clear();
                    _postLogic.Clear();
                    _topicLogic.AddMultiple(mockedTopics);
                    _postLogic.AddMultiple(mockedPosts);
                    LogAddedTopicsAndPosts(mockedTopics, mockedPosts);
                    break;
                case "1":
                    var usersToShow = _userLogic.ShowAll();
                    Console.WriteLine(usersToShow);
                    break;
                case "2":
                    var topicsToShow = _topicLogic.ShowAll();
                    Console.WriteLine(topicsToShow);
                    break;
                case "3":
                    var postsByTopic = _postLogic.ShowPostsByTopic();
                    Console.WriteLine(postsByTopic);
                    break;
                case "4":
                    var postsByDate = _postLogic.ShowPostsByDate();
                    Console.WriteLine(postsByDate);
                    break;
                case "5":
                    var postsByDateAndTopic = _postLogic.ShowPostsByDateAndTopic();
                    Console.WriteLine(postsByDateAndTopic);
                    break;
                case "6":
                    var postsByTopicAndDate = _postLogic.ShowPostsByTopicAndDate();
                    Console.WriteLine(postsByTopicAndDate);
                    break;
                case "7":
                    Console.WriteLine("Nombre del post a mostrar:");
                    var postName = Console.ReadLine();
                    var postsByName = _postLogic.ShowPostByName(postName);
                    Console.WriteLine(postsByName);
                    break;
                case "8":
                    var topics = _topicLogic.GetAll();
                    var topicsWithMorePosts = _postLogic.ShowTopicsWithMorePosts(topics);
                    Console.WriteLine(topicsWithMorePosts);
                    break;
                case "9":
                    Console.WriteLine("Nombre del post del cual se desea obtener el archivo:");
                    var post = Console.ReadLine();
                    var file = _postLogic.GetFileFromPostName(post);
                    if (file != null)
                    {
                        Console.WriteLine(file.PrintFile(false));
                    }
                    else
                    {
                        Console.WriteLine("El post no existe o no tiene archivos");
                    }

                    break;
                case "10":
                    if (_postLogic.ExistFilesInPosts())
                    {
                        var topicFilter = HandleFilterByTopic();
                        Console.WriteLine("Mostrar por:");
                        Console.WriteLine("1 - Fecha");
                        Console.WriteLine("2 - Nombre");
                        Console.WriteLine("3 - Tamaño");
                        var filterOption = Console.ReadLine();
                        var filesToShow = ShowFiles(filterOption, topicFilter);
                        Console.WriteLine(filesToShow);
                    }
                    else
                    {
                        Console.WriteLine("No existen post con archivos");
                    }
                    break;
                case "99":
                    _userLogic.CloseAll();
                    serverState.IsServerTerminated = true;
                    break;
                default:
                    Console.WriteLine("Opcion invalida...");
                    break;
            }
        }

        private List<Topic> HandleFilterByTopic()
        {
            Console.WriteLine("Desea filtar por tema? (s para confirmar)");
            var confirm = Console.ReadLine();
            var topicFilter = new List<Topic>();
            if (confirm.Equals("s"))
            {
                Console.WriteLine("Elija los nombres de los temas a filtrar: (s para salir)");
                var exitFilter = false;
                while (!exitFilter)
                {
                    Console.WriteLine("Nombre del tema:");
                    var topicName = Console.ReadLine();
                    if (!topicName.Equals("s"))
                    {
                        var topic = _topicLogic.GetByName(topicName);
                        if (topic == null)
                        {
                            Console.WriteLine("El tema no existe");
                        }
                        else
                        {
                            topicFilter.Add(topic);
                        }
                    }
                    else
                        exitFilter = true;
                }
            }
            return topicFilter;
        }

        private string ShowFiles(string filterOption, List<Topic> topicFilter)
        {
            switch (filterOption)
            {
                case "1":
                    var postsByTopicsOrderByDate = _postLogic.ShowFilesByTopicsOrderByDate(topicFilter);
                    return postsByTopicsOrderByDate;
                case "2":
                    var postsByTopicsOrderByName = _postLogic.ShowFilesByTopicsOrderByName(topicFilter);
                    return postsByTopicsOrderByName;
                case "3":
                    var postsByTopicsOrderBySize = _postLogic.ShowFilesByTopicsOrderBySize(topicFilter);
                    return postsByTopicsOrderBySize;
                default:
                    return "Opcion invalida...";
            }
        }

        private void LogAddedTopicsAndPosts(List<Topic> mockedTopics, List<Post> mockedPosts)
        {
            foreach(var topic in mockedTopics)
            {

                _messageLogic.SendMessageAsync($"El topic {topic.Name} se ha creado con exito.", "Topic");
            }
            foreach (var post in mockedPosts)
            {
                _messageLogic.SendMessageAsync($"El topic {post.Name} se ha creado con exito.", "Post");
            }
        }
    }
}
