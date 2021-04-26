using System.Threading;
using SistemaBlueddit.Domain;
using SistemaBlueddit.Protocol.Library;
using SistemaBlueddit.Server.Logic;
using System;
using System.Net.Sockets;

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
                        var topicCreatedSuccess = new Response { ServerResponse = "El tema se ha creado con exito" };
                        DataHandler<Response>.SendData(acceptedClient, "00", HeaderConstants.Response, topicCreatedSuccess);
                    }
                    else
                    {
                        var topicCreatedError = new Response { ServerResponse = "Error. El tema ya existe" };
                        DataHandler<Response>.SendData(acceptedClient, "00", HeaderConstants.Response, topicCreatedError);
                    }
                    break;
                case 02:
                    var postRecieved = new Post();
                    _postLogic.Recieve(header, networkStream, postRecieved);
                    if (_postLogic.Validate(postRecieved) && _topicLogic.ValidateTopics(postRecieved.Topics))
                    {
                        _postLogic.Add(postRecieved);
                        var postCreatedSuccess = new Response { ServerResponse = "El post se ha creado con exito" };
                        DataHandler<Response>.SendData(acceptedClient, "00", HeaderConstants.Response, postCreatedSuccess);
                    }
                    else
                    {
                        var postCreatedError = new Response { ServerResponse = "Error. El post ya existe o los temas no son validos" };
                        DataHandler<Response>.SendData(acceptedClient, "00", HeaderConstants.Response, postCreatedError);
                    }
                    break;
                case 03:
                    var postToAddFile = new Post();
                    _postLogic.Recieve(header, networkStream, postToAddFile);
                    var existingPost = _postLogic.GetByName(postToAddFile.Name);
                    if (existingPost != null)
                    {
                        var existsPost = new Response { ServerResponse = "existe" };
                        DataHandler<Response>.SendData(acceptedClient, "00", HeaderConstants.Response, existsPost);
                        header = HeaderHandler.DecodeHeader(networkStream);
                        HeaderHandler.ValidateHeader(header, HeaderConstants.Request);
                        var bluedditFile = _fileLogic.GetFile(header, networkStream);
                        _postLogic.AddFileToPost(bluedditFile, existingPost);
                        var fileAddedSuccess = new Response { ServerResponse = "El archivo se ha agregado al post con exito" };
                        DataHandler<Response>.SendData(acceptedClient, "00", HeaderConstants.Response, fileAddedSuccess);
                    }
                    else
                    {
                        var notExistsPost = new Response { ServerResponse = "noexiste" };
                        DataHandler<Response>.SendData(acceptedClient, "00", HeaderConstants.Response, notExistsPost);
                    }
                    break;
                case 04:
                    var topicToRemove = new Topic();
                    _topicLogic.Recieve(header, networkStream, topicToRemove);
                    var topicToRemoveLock = _topicLogic.GetByName(topicToRemove.Name);
                    if(topicToRemoveLock != null)
                    {
                        lock (topicToRemoveLock)
                        {
                            var existingTopic = _topicLogic.GetByName(topicToRemove.Name);
                            if (existingTopic != null)
                            {
                                if (_postLogic.IsTopicInPost(existingTopic))
                                {
                                    var topicDeleteError = new Response { ServerResponse = "Error. No se puede borrar el tema porque esta asociado a un post." };
                                    DataHandler<Response>.SendData(acceptedClient, "00", HeaderConstants.Response, topicDeleteError);
                                }
                                else
                                {
                                    _topicLogic.Delete(existingTopic);
                                    var topicDeleteSuccess = new Response { ServerResponse = "El tema ingresado se ha borrado con exito." };
                                    DataHandler<Response>.SendData(acceptedClient, "00", HeaderConstants.Response, topicDeleteSuccess);
                                }
                            }
                            else
                            {
                                var topicDoesntExist = new Response { ServerResponse = "No existe el tema con el nombre ingresado." };
                                DataHandler<Response>.SendData(acceptedClient, "00", HeaderConstants.Response, topicDoesntExist);
                            }
                        }
                    }
                    else
                    {
                        var topicDoesntExist = new Response { ServerResponse = "No existe el tema con el nombre ingresado." };
                        DataHandler<Response>.SendData(acceptedClient, "00", HeaderConstants.Response, topicDoesntExist);
                    }
                    break;
                case 05:
                    var topicToModify = new Topic();
                    _topicLogic.Recieve(header, networkStream, topicToModify);
                    var topicToModifyLock = _topicLogic.GetByName(topicToModify.Name);
                    if(topicToModifyLock != null)
                    {
                        lock (topicToModifyLock)
                        {
                            var topicResponse = _topicLogic.ModifyTopic(topicToModify);
                            var topicModifiedResponse = new Response { ServerResponse = topicResponse };
                            DataHandler<Response>.SendData(acceptedClient, "00", HeaderConstants.Response, topicModifiedResponse);
                        }
                    }
                    else
                    {
                        var topicModifiedErrorResponse = new Response { ServerResponse = "El tema no existe" };
                        DataHandler<Response>.SendData(acceptedClient, "00", HeaderConstants.Response, topicModifiedErrorResponse);
                    }
                    break;
                case 06:
                    var postToRemove = new Post();
                    _postLogic.Recieve(header, networkStream, postToRemove);
                    var postToRemoveLock = _postLogic.GetByName(postToRemove.Name);
                    if(postToRemoveLock != null)
                    {
                        lock (postToRemoveLock)
                        {
                            var existingPostToRemove = _postLogic.GetByName(postToRemove.Name);
                            if(existingPostToRemove != null)
                            {
                                _postLogic.Delete(existingPostToRemove);
                                var deletedPostSuccess = new Response { ServerResponse = "El post ingresado se ha borrado con exito." };
                                DataHandler<Response>.SendData(acceptedClient, "00", HeaderConstants.Response, deletedPostSuccess);
                            }
                            else
                            {
                                var postDeleteError = new Response { ServerResponse = "No existe el post con el nombre ingresado." };
                                DataHandler<Response>.SendData(acceptedClient, "00", HeaderConstants.Response, postDeleteError);
                            };
                        }
                    }
                    else
                    {
                        var postDeleteError = new Response { ServerResponse = "No existe el post con el nombre ingresado." };
                        DataHandler<Response>.SendData(acceptedClient, "00", HeaderConstants.Response, postDeleteError);
                    };
                    break;
                case 07:
                    var postToModify = new Post();
                    _postLogic.Recieve(header, networkStream, postToModify);
                    if (_topicLogic.ValidateTopics(postToModify.Topics))
                    {
                        var modifyPostLock = _postLogic.GetByName(postToModify.Name);
                        if(modifyPostLock != null)
                        {
                            lock (modifyPostLock)
                            {
                                var postResponse = _postLogic.ModifyPost(postToModify);
                                var modifyPostResponse = new Response { ServerResponse = postResponse };
                                DataHandler<Response>.SendData(acceptedClient, "00", HeaderConstants.Response, modifyPostResponse);
                            }
                        }
                        else
                        {
                            var postDeleteError = new Response { ServerResponse = "No existe el post con el nombre ingresado." };
                            DataHandler<Response>.SendData(acceptedClient, "00", HeaderConstants.Response, postDeleteError);
                        }
                    }
                    else
                    {
                        var modifyPostResponseTopicError = new Response { ServerResponse = "Error. Los temas ingresados no son validos" };
                        DataHandler<Response>.SendData(acceptedClient, "00", HeaderConstants.Response, modifyPostResponseTopicError);
                    }
                    break;
                default:
                    Console.WriteLine("Opcion invalida...");
                    break;
            }
        }
    }
}
