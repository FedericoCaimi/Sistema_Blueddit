﻿using SistemaBlueddit.Domain;
using SistemaBlueddit.Protocol.Library;
using SistemaBlueddit.Server.Logic.Interfaces;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaBlueddit.Server
{
    public class ClientHandler
    {
        private ITopicLogic _topicLogic;

        private IPostLogic _postLogic;

        private IFileLogic _fileLogic;

        private IUserLogic _userLogic;

        public ClientHandler(ITopicLogic topicLogic, IPostLogic postLogic, IFileLogic fileLogic, IUserLogic userLogic)
        {
            _topicLogic = topicLogic;
            _postLogic = postLogic;
            _fileLogic = fileLogic;
            _userLogic = userLogic;
        }

        public async Task HandleClientAsync(TcpClient acceptedClient, ServerState serverState)
        {
            var user = new User
            {
                StartConnection = DateTime.Now,
                TcpClient = acceptedClient
            };
            _userLogic.Add(user);
            try
            {
                while (!serverState.IsServerTerminated)
                {
                    await HandleRequestsAsync(acceptedClient);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Borrando el cliente. " + e.Message);
                _userLogic.Delete(user);
            }
            Console.WriteLine("El cliente con hora de conexion " + user.StartConnection.ToString() + " se desconecto");
        }

        public async Task HandleRequestsAsync(TcpClient acceptedClient)
        {
            var networkStream = acceptedClient.GetStream();
            var header = await HeaderHandler.DecodeHeaderAsync(networkStream);
            HeaderHandler.ValidateHeader(header, HeaderConstants.Request);
            switch (header.Command)
            {
                case Commands.CreateNewTopic:
                    await HandleCreateNewTopicAsync(acceptedClient, header, networkStream);
                    break;
                case Commands.CreateNewPost:
                    await HandleCreateNewPostAsync(acceptedClient, header, networkStream);
                    break;
                case Commands.UploadFile:
                    await HandleUploadFileAsync(acceptedClient, header, networkStream);
                    break;
                case Commands.DeleteTopic:
                    await HandleDeleteTopicAsync(acceptedClient, header, networkStream);
                    break;
                case Commands.ModifyTopic:
                    await HandleModifyTopicAsync(acceptedClient, header, networkStream);
                    break;
                case Commands.DeletePost:
                    await HandleDeletePostAsync(acceptedClient, header, networkStream);
                    break;
                case Commands.ModifyPost:
                    await HandleModifyPostAsync(acceptedClient, header, networkStream);
                    break;
                default:
                    Console.WriteLine("Opcion invalida...");
                    break;
            }
        }

        private async Task HandleCreateNewTopicAsync(TcpClient acceptedClient, Header header, NetworkStream networkStream)
        {
            var topicRecieved = new Topic();
            await _topicLogic.RecieveAsync(header, networkStream, topicRecieved);
            if (_topicLogic.Validate(topicRecieved))
            {
                _topicLogic.Add(topicRecieved);
                var topicCreatedSuccess = new Response { ServerResponse = "El tema se ha creado con exito" };
                await DataHandler<Response>.SendDataAsync(acceptedClient, Commands.Response.ToString(), HeaderConstants.Response, topicCreatedSuccess);
            }
            else
            {
                var topicCreatedError = new Response { ServerResponse = "Error. El tema ya existe" };
                await DataHandler<Response>.SendDataAsync(acceptedClient, Commands.Response.ToString(), HeaderConstants.Response, topicCreatedError);
            }
        }

        private async Task HandleCreateNewPostAsync(TcpClient acceptedClient, Header header, NetworkStream networkStream)
        {
            var postRecieved = new Post();
            await _postLogic.RecieveAsync(header, networkStream, postRecieved);
            if (_postLogic.Validate(postRecieved) && _topicLogic.ValidateTopics(postRecieved.Topics))
            {
                _postLogic.Add(postRecieved);
                var postCreatedSuccess = new Response { ServerResponse = "El post se ha creado con exito" };
                await DataHandler<Response>.SendDataAsync(acceptedClient, Commands.Response.ToString(), HeaderConstants.Response, postCreatedSuccess);
            }
            else
            {
                var postCreatedError = new Response { ServerResponse = "Error. El post ya existe o los temas no son validos" };
                await DataHandler<Response>.SendDataAsync(acceptedClient, Commands.Response.ToString(), HeaderConstants.Response, postCreatedError);
            }
        }

        private async Task HandleUploadFileAsync(TcpClient acceptedClient, Header header, NetworkStream networkStream)
        {
            var postToAddFile = new Post();
            await _postLogic.RecieveAsync(header, networkStream, postToAddFile);
            var existingPost = _postLogic.GetByName(postToAddFile.Name);
            if (existingPost != null)
            {
                var existsPost = new Response { ServerResponse = "existe" };
                await DataHandler<Response>.SendDataAsync(acceptedClient, Commands.Response.ToString(), HeaderConstants.Response, existsPost);
                header = await HeaderHandler.DecodeHeaderAsync(networkStream);
                HeaderHandler.ValidateHeader(header, HeaderConstants.Request);
                var bluedditFile = await _fileLogic.GetFileAsync(header, networkStream);
                _postLogic.AddFileToPost(bluedditFile, existingPost);
                var fileAddedSuccess = new Response { ServerResponse = "El archivo se ha agregado al post con exito" };
                await DataHandler<Response>.SendDataAsync(acceptedClient, Commands.Response.ToString(), HeaderConstants.Response, fileAddedSuccess);

            }
            else
            {
                var notExistsPost = new Response { ServerResponse = "noexiste" };
                await DataHandler<Response>.SendDataAsync(acceptedClient, Commands.Response.ToString(), HeaderConstants.Response, notExistsPost);
            }
        }

        private async Task HandleDeleteTopicAsync(TcpClient acceptedClient, Header header, NetworkStream networkStream)
        {
            var topicToRemove = new Topic();
            await _topicLogic.RecieveAsync(header, networkStream, topicToRemove);
            var topicToRemoveLock = _topicLogic.GetByName(topicToRemove.Name);
            if (topicToRemoveLock != null)
            {
                try
                {
                    Console.WriteLine("Esperando...");
                    await topicToRemoveLock.SemaphoreSlim.WaitAsync();
                    Console.WriteLine($"Borrando a {topicToRemoveLock.Name}...");
                    Thread.Sleep(2000);
                    var existingTopic = _topicLogic.GetByName(topicToRemove.Name);
                    if (existingTopic != null)
                    {
                        if (_postLogic.IsTopicInPost(existingTopic))
                        {
                            var topicDeleteError = new Response { ServerResponse = "Error. No se puede borrar el tema porque esta asociado a un post." };
                            await DataHandler<Response>.SendDataAsync(acceptedClient, Commands.Response.ToString(), HeaderConstants.Response, topicDeleteError);
                        }
                        else
                        {
                            _topicLogic.Delete(existingTopic);
                            var topicDeleteSuccess = new Response { ServerResponse = "El tema ingresado se ha borrado con exito." };
                            await DataHandler<Response>.SendDataAsync(acceptedClient, Commands.Response.ToString(), HeaderConstants.Response, topicDeleteSuccess);
                        }
                    }
                    else
                    {
                        var topicDoesntExist = new Response { ServerResponse = "No existe el tema con el nombre ingresado." };
                        await DataHandler<Response>.SendDataAsync(acceptedClient, Commands.Response.ToString(), HeaderConstants.Response, topicDoesntExist);
                    }
                }
                finally
                {
                    Console.WriteLine("Finally");
                    topicToRemoveLock.SemaphoreSlim.Release();               
                }
            }
            else
            {
                var topicDoesntExist = new Response { ServerResponse = "No existe el tema con el nombre ingresado." };
                await DataHandler<Response>.SendDataAsync(acceptedClient, Commands.Response.ToString(), HeaderConstants.Response, topicDoesntExist);
            }
        }

        private async Task HandleModifyTopicAsync(TcpClient acceptedClient, Header header, NetworkStream networkStream)
        {
            var topicToModify = new Topic();
            await _topicLogic.RecieveAsync(header, networkStream, topicToModify);
            var topicToModifyLock = _topicLogic.GetByName(topicToModify.Name);
            if (topicToModifyLock != null)
            {
                try
                {
                    Console.WriteLine("Esperando...");
                    await topicToModifyLock.SemaphoreSlim.WaitAsync();
                    Console.WriteLine($"Modificando a {topicToModifyLock.Name}...");
                    Thread.Sleep(2000);
                    var topicResponse = _topicLogic.ModifyTopic(topicToModify);
                    var topicModifiedResponse = new Response { ServerResponse = topicResponse };
                    await DataHandler<Response>.SendDataAsync(acceptedClient, Commands.Response.ToString(), HeaderConstants.Response, topicModifiedResponse);
                }
                finally
                {
                    topicToModifyLock.SemaphoreSlim.Release();
                }
            }
            else
            {
                var topicModifiedErrorResponse = new Response { ServerResponse = "El tema no existe" };
                await DataHandler<Response>.SendDataAsync(acceptedClient, Commands.Response.ToString(), HeaderConstants.Response, topicModifiedErrorResponse);
            }
        }

        private async Task HandleDeletePostAsync(TcpClient acceptedClient, Header header, NetworkStream networkStream)
        {
            var postToRemove = new Post();
            await _postLogic.RecieveAsync(header, networkStream, postToRemove);
            var postToRemoveLock = _postLogic.GetByName(postToRemove.Name);
            if (postToRemoveLock != null)
            {
                try
                {
                    Console.WriteLine("Esperando...");
                    await postToRemoveLock.SemaphoreSlim.WaitAsync();
                    Console.WriteLine($"Borrando a {postToRemoveLock.Name}...");
                    Thread.Sleep(2000);
                    var existingPostToRemove = _postLogic.GetByName(postToRemove.Name);
                    if (existingPostToRemove != null)
                    {
                        _postLogic.Delete(existingPostToRemove);
                        var deletedPostSuccess = new Response { ServerResponse = "El post ingresado se ha borrado con exito." };
                        await DataHandler<Response>.SendDataAsync(acceptedClient, Commands.Response.ToString(), HeaderConstants.Response, deletedPostSuccess);
                    }
                    else
                    {
                        var postDeleteError = new Response { ServerResponse = "No existe el post con el nombre ingresado." };
                        await DataHandler<Response>.SendDataAsync(acceptedClient, Commands.Response.ToString(), HeaderConstants.Response, postDeleteError);
                    };
                }
                finally
                {
                    postToRemoveLock.SemaphoreSlim.Release();
                }
            }
            else
            {
                var postDeleteError = new Response { ServerResponse = "No existe el post con el nombre ingresado." };
                await DataHandler<Response>.SendDataAsync(acceptedClient, Commands.Response.ToString(), HeaderConstants.Response, postDeleteError);
            };
        }

        private async Task HandleModifyPostAsync(TcpClient acceptedClient, Header header, NetworkStream networkStream)
        {
            var postToModify = new Post();
            await _postLogic.RecieveAsync(header, networkStream, postToModify);
            if (_topicLogic.ValidateTopics(postToModify.Topics))
            {
                var modifyPostLock = _postLogic.GetByName(postToModify.Name);
                if (modifyPostLock != null)
                {
                    try
                    {
                        Console.WriteLine("Esperando...");
                        await modifyPostLock.SemaphoreSlim.WaitAsync();
                        Console.WriteLine($"Modificando a {modifyPostLock.Name}...");
                        Thread.Sleep(2000);
                        var postResponse = _postLogic.ModifyPost(postToModify);
                        var modifyPostResponse = new Response { ServerResponse = postResponse };
                        await DataHandler<Response>.SendDataAsync(acceptedClient, Commands.Response.ToString(), HeaderConstants.Response, modifyPostResponse);
                    }
                    finally
                    {
                        modifyPostLock.SemaphoreSlim.Release();
                    }
                }
                else
                {
                    var postDeleteError = new Response { ServerResponse = "No existe el post con el nombre ingresado." };
                    await DataHandler<Response>.SendDataAsync(acceptedClient, Commands.Response.ToString(), HeaderConstants.Response, postDeleteError);
                }
            }
            else
            {
                var modifyPostResponseTopicError = new Response { ServerResponse = "Error. Los temas ingresados no son validos" };
                await DataHandler<Response>.SendDataAsync(acceptedClient, Commands.Response.ToString(), HeaderConstants.Response, modifyPostResponseTopicError);
            }
        }
    }
}