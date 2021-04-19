using SistemaBlueddit.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SistemaBlueddit.Domain
{
    public class Post: ISerializable<Post>
    {
        public string Name { get; set; }

        public List<Topic> Topics { get; set; }

        public DateTime CreationDate { get; set; }

        public string SerializeObejct()
        {
            var serializedTopics = "";
            foreach(var topic in Topics)
            {
                serializedTopics += topic.SerializeObejct() + ",";
            }
            serializedTopics = serializedTopics.Remove(serializedTopics.Length - 1);
            return $"name:{Name},topics:[{serializedTopics}],creationDate:'{CreationDate.Year + "-" + CreationDate.Month + "-" + CreationDate.Day}'";
        }

        public Post DeserializeObject(string objectToDeserialize)
        {
            var curlyBracesPattern = new Regex("{|}");
            var objectProperties = curlyBracesPattern.Replace(objectToDeserialize, "").Split(",");
            foreach (var property in objectProperties)
            {
                var propertyName = property.Split(":")[0];
                var propertyValue = property.Split(":")[1];
                switch (propertyName)
                {
                    case ("name"):
                        Name = propertyValue;
                        break;
                    case ("topics"):
                        var topics = new List<Topic>();
                        var straigthBracesPattern = new Regex("[|]");
                        var serializedTopics = straigthBracesPattern.Replace(propertyValue, "").Split(",");
                        foreach(var serializedTopic in serializedTopics)
                        {
                            var topic = new Topic().DeserializeObject(serializedTopic);
                            topics.Add(topic);
                        }
                        Topics = topics;
                        break;
                    case ("creationDate"):
                        CreationDate = DateTime.ParseExact(propertyValue, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    default:
                        throw new Exception("Objeto no es del tipo post");
                }
            }
            return this;
        }
    }
}
