using SistemaBlueddit.Domain.Interface;
using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace SistemaBlueddit.Domain
{
    public class Topic: ISerializable<Topic>, ISemaphoreSlim
    {
        public string Name { get; set; }
        public string Description { get; set; }
        private SemaphoreSlim _SemaphoreSlim;

        public SemaphoreSlim SemaphoreSlim
        {
            get
            {
                if (_SemaphoreSlim == null)
                {
                    _SemaphoreSlim = new SemaphoreSlim(1, 1);
                }
                return _SemaphoreSlim;
            }
        }

        public string Print()
        {
            return $"Nombre Tema: {Name}\nDescripcion del Tema: {Description}";
        }

        public string SerializeObejct()
        {
            return $"{{name:{Name},description:{Description}}}";
        }

        public Topic DeserializeObject(string objectToDeserialize)
        {
            var pattern = new Regex("{|}");
            var objectProperties = pattern.Replace(objectToDeserialize, "").Split(",");
            foreach(var property in objectProperties)
            {
                var propertyName = property.Split(":")[0];
                var propertyValue = property.Split(":")[1];
                switch (propertyName)
                {
                    case ("name"):
                        Name = propertyValue;
                        break;
                    case ("description"):
                        Description = propertyValue;
                        break;
                    default:
                        throw new Exception("Objeto no es del tipo tema");
                }
            }
            return this;
        }

        public override bool Equals(object obj) 
        { 
            var topic = obj as Topic; 
            return topic != null && Name == topic.Name; 
        }
    }
}