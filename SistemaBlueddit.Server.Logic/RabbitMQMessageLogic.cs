using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SistemaBlueddit.Domain;
using SistemaBlueddit.Server.Logic.Interfaces;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SistemaBlueddit.Server.Logic
{
    public class RabbitMQMessageLogic: IRabbitMQMessageLogic
    {
        private IModel _channel;

        private IConnection _connection;

        public RabbitMQMessageLogic(IConfiguration configuration)
        {
            InitializeRabbitMq(configuration.GetSection("serverIP").Value);
        }

        public Task<bool> SendMessageAsync(string message, string type)
        {
            bool returnVal;
            var log = new Log
            {
                LogType = type,
                Message = message,
                CreationDate = DateTime.Now
            };
            try
            {
                var logJson = JsonConvert.SerializeObject(log);
                var body = Encoding.UTF8.GetBytes(logJson);
                _channel.BasicPublish(exchange: "",
                    routingKey: "log_queue",
                    basicProperties: null,
                    body: body);
                returnVal = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                returnVal = false;
            }

            return Task.FromResult(returnVal);
        }

        public void DisposeConnections()
        {
            _connection.Dispose();
            _channel.Dispose();
        }

        private void InitializeRabbitMq(string serverIP)
        {
            var factory = new ConnectionFactory()
            {
                HostName = serverIP
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "log_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }
    }
}
