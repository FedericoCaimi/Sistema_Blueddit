using SistemaBlueddit.Domain;
using SistemaBlueddit.Protocol.Library;
using SistemaBlueddit.Server.Logic;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace SistemaBlueddit.Server
{
    public class ClientRequestHandler
    {
        private TopicLogic _topicLogic;

        private PostLogic _postLogic;

        private FileLogic _fileLogic;

        public ClientRequestHandler(TopicLogic topicLogic, PostLogic postLogic, FileLogic fileLogic)
        {
            _topicLogic = topicLogic;
            _postLogic = postLogic;
            _fileLogic = fileLogic;
        }

        public void HandleClientRequests(TcpClient acceptedClient)
        {
            var networkStream = acceptedClient.GetStream();
            var header = HeaderHandler.DecodeHeader(networkStream);
            HeaderHandler.ValidateHeader(header, HeaderConstants.Request);
            switch (header.Command)
            {
                case 01:
                    var topicRecieved = new Topic();
                    _topicLogic.Recieve(header, networkStream, topicRecieved);
                    if (_topicLogic.Validate(topicRecieved))
                    {
                        _topicLogic.Add(topicRecieved);
                        DataHandler.SendResponse(acceptedClient, "El tema se ha creado con exito");
                    }
                    else
                    {
                        DataHandler.SendResponse(acceptedClient, "Error. El tema ya existe");
                    }
                    break;
                case 02:
                    var postRecieved = new Post();
                    _postLogic.Recieve(header, networkStream, postRecieved);
                    if (_postLogic.Validate(postRecieved) && _topicLogic.ValidateTopics(postRecieved.Topics))
                    {
                        _postLogic.Add(postRecieved);
                        DataHandler.SendResponse(acceptedClient, "El post se ha creado con exito");
                    }
                    else
                    {
                        DataHandler.SendResponse(acceptedClient, "Error. El post ya existe o los temas no son validos");
                    }
                    break;
                case 03:
                    var postToAddFile = new Post();
                    _postLogic.Recieve(header, networkStream, postToAddFile);
                    var existingPost = _postLogic.GetByName(postToAddFile.Name);
                    if (existingPost != null)
                    {
                        DataHandler.SendResponse(acceptedClient, "existe");
                        header = HeaderHandler.DecodeHeader(networkStream);
                        HeaderHandler.ValidateHeader(header, HeaderConstants.Request);
                        var bluedditFile = _fileLogic.GetFile(header, networkStream);
                        Console.WriteLine(bluedditFile.FileName);
                        _postLogic.AddFileToPost(bluedditFile, existingPost);
                        DataHandler.SendResponse(acceptedClient, "El archivo se ha agregado al post con exito");
                    }
                    else
                    {
                        DataHandler.SendResponse(acceptedClient, "noexiste");
                    }
                    break;
                case 04:
                    var topicToRemove = new Topic();
                    _topicLogic.Recieve(header, networkStream, topicToRemove);
                    var existingTopic = _topicLogic.GetByName(topicToRemove.Name);
                    if (existingTopic != null)
                    {
                        if (_postLogic.IsTopicInPost(existingTopic))
                        {
                            DataHandler.SendResponse(acceptedClient, "Error. No se puede borrar el tema porque esta asociado a un post.");
                        }
                        else
                        {
                            _topicLogic.Delete(existingTopic);
                            DataHandler.SendResponse(acceptedClient, "El tema ingresado se ha borrado con exito.");
                        }
                    }
                    else
                    {
                        DataHandler.SendResponse(acceptedClient, "No existe el tema con el nombre ingresado.");
                    }
                    break;
                case 05:
                    var topicToModify = new Topic();
                    _topicLogic.Recieve(header, networkStream, topicToModify);
                    var topicResponse = _topicLogic.ModifyTopic(topicToModify);
                    DataHandler.SendResponse(acceptedClient, topicResponse);
                    break;
                case 06:
                    var postToRemove = new Post();
                    _postLogic.Recieve(header, networkStream, postToRemove);
                    var existingPostToRemove = _postLogic.GetByName(postToRemove.Name);
                    if (existingPostToRemove != null)
                    {
                        _postLogic.Delete(existingPostToRemove);
                        DataHandler.SendResponse(acceptedClient, "El post ingresado se ha borrado con exito.");
                    }
                    else
                    {
                        DataHandler.SendResponse(acceptedClient, "No existe el post con el nombre ingresado.");
                    }
                    break;
                case 07:
                    var postToModify = new Post();
                    _postLogic.Recieve(header, networkStream, postToModify);
                    if (_topicLogic.ValidateTopics(postToModify.Topics))
                    {
                        var postResponse = _postLogic.ModifyPost(postToModify);
                        DataHandler.SendResponse(acceptedClient, postResponse);
                    }
                    else
                    {
                        DataHandler.SendResponse(acceptedClient, "Error. Los temas ingresados no son validos");
                    }
                    break;
                default:
                    Console.WriteLine("Opcion invalida...");
                    break;
            }
        }
    }
}
