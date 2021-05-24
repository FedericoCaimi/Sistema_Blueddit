using Newtonsoft.Json;
using RabbitMQ.Client;
using SistemaBlueddit.Domain;
using SistemaBlueddit.Domain.Interface;
using SistemaBlueddit.Server.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SistemaBlueddit.Server.Logic
{
    public class Logic<T> : ILogic<T> where T : ISerializable<T>, ISemaphoreSlim
    {
        private Type _type;

        protected List<T> _elements;

        public Logic()
        {
            _elements = new List<T>();
            _type = typeof(T);
        }

        public void Add(T objectToAdd)
        {
            _elements.Add(objectToAdd);
        }

        public void AddMultiple(ICollection<T> objectsToAdd)
        {
            _elements.AddRange(objectsToAdd);
        }

        public void Clear()
        {
            _elements = new List<T>();
        }

        public void Delete(T objectToDelete)
        {
            _elements = _elements.Where(t => !t.Equals(objectToDelete)).ToList();
        }

        public List<T> GetAll()
        {
            return _elements;
        }

        public virtual T GetByName(string name)
        {
            return default;
        }

        public async Task RecieveAsync(Header header, NetworkStream stream, T objectToRecieve)
        {
            var data = new byte[header.DataLength];
            await stream.ReadAsync(data, 0, header.DataLength);
            var json = Encoding.UTF8.GetString(data);
            objectToRecieve.DeserializeObject(json);
        }

        public string Show(T objectToShow)
        {
            return objectToShow.Print();
        }

        public string ShowMultiple(ICollection<T> objectsToShow)
        {
            var objectsPrinted = "";
            foreach(var objectToPrint in objectsToShow)
            {
                objectsPrinted += objectToPrint.Print() + "\n";
            }
            if(objectsPrinted == "")
                return "No se encontraron datos para la opcion seleccionada...";
            return objectsPrinted;
        }

        public string ShowAll()
        {
            return ShowMultiple(_elements);
        }

        public bool Validate(T objectToValidate)
        {
            return !_elements.Exists(p => p.Equals(objectToValidate));
        }

        public Task<bool> SendMessageAsync(IModel channel, string message)
        {
            bool returnVal;
            var type = _type.ToString();
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
                channel.BasicPublish(exchange: "",
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
    }
}
