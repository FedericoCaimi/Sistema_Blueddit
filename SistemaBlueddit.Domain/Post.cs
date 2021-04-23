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

        public BluedditFile File { get; set; }

        public string SerializeObejct()
        {
            var serializedTopics = "";
            foreach(var topic in Topics)
            {
                serializedTopics += topic.SerializeObejct() + ",";
            }
            serializedTopics = serializedTopics.Remove(serializedTopics.Length - 1);
            return $"{{name:{Name},topics:[{serializedTopics}],creationDate:'{CreationDate:yyyy-MM-dd HH:mm:ss}'}}";
        }

        public Post DeserializeObject(string objectToDeserialize)
        {
            var curlyBracesPattern = new Regex("^{|}$");
            var objectsInListPattern = new Regex("(?<=\\[).*(?=\\])");
            var objectsInList = objectsInListPattern.Match(objectToDeserialize).Value;
            objectsInList = objectsInList.Replace(",", "|");
            var objectWithoutInitialCurlyBrances = curlyBracesPattern.Replace(objectToDeserialize, "");
            var objectProperties = objectsInListPattern.Replace(objectWithoutInitialCurlyBrances, objectsInList).Split(",");
            foreach (var property in objectProperties)
            {
                var propertyName = property.Split(":")[0];
                var propertyValue = property.Replace(propertyName + ":", "");
                switch (propertyName)
                {
                    case ("name"):
                        Name = propertyValue;
                        break;
                    case ("topics"):
                        var topics = new List<Topic>();
                        var straigthBracesPattern = new Regex("\\[|\\]");
                        var serializedTopics = straigthBracesPattern.Replace(propertyValue, "").Replace("}|{", "},{").Split(",");
                        foreach(var serializedTopic in serializedTopics)
                        {
                            var topicString = serializedTopic.Replace("|",",");
                            var topic = new Topic().DeserializeObject(topicString);
                            topics.Add(topic);
                        }
                        Topics = topics;
                        break;
                    case ("creationDate"):
                        var creationDate = propertyValue.Replace("'", "");
                        CreationDate = Convert.ToDateTime(creationDate);
                        break;
                    default:
                        throw new Exception("Objeto no es del tipo post");
                }
            }
            return this;
        }

        public string PrintPost()
        {
            var topicsPrinted = "Topics:\n";
            foreach(var topic in Topics)
            {
                topicsPrinted += $"\tTopic Name: {topic.Name}\n";
            }
            var filePrinted = File != null ? "\nFile:\n" + File.PrintFile(true) : "";
            return $"Name: {Name}\n{topicsPrinted}CreationDate: '{CreationDate:yyyy-MM-dd HH:mm:ss}'{filePrinted}";
        }
    }
}
