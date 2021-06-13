using SistemaBlueddit.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;

namespace SistemaBlueddit.Domain
{
    public class Post: ISerializable<Post>, ISemaphoreSlim
    {
        public string Name { get; set; }

        public string Content { get; set; }

        public List<Topic> Topics { get; set; }

        public DateTime CreationDate { get; set; }

        [JsonIgnore]
        public BluedditFile File { get; set; }

        private SemaphoreSlim _semaphoreSlim;

        [JsonIgnore]
        public SemaphoreSlim SemaphoreSlim { 
            get {
                if(_semaphoreSlim == null)
                {
                    _semaphoreSlim = new SemaphoreSlim(1, 1);
                }
                return _semaphoreSlim;
            }
        }

        public string SerializeObejct()
        {
            var serializedTopics = "";
            foreach(var topic in Topics)
            {
                serializedTopics += topic.SerializeObejct() + ",";
            }
            serializedTopics = !serializedTopics.Equals("") 
                ? serializedTopics.Remove(serializedTopics.Length - 1) 
                : serializedTopics;
            return $"{{name:{Name},Content:{Content},topics:[{serializedTopics}],creationDate:'{CreationDate:yyyy-MM-dd HH:mm:ss}'}}";
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
                    case ("Content"):
                        Content = propertyValue;
                        break;
                    case ("topics"):
                        var topics = new List<Topic>();
                        var straigthBracesPattern = new Regex("\\[|\\]");
                        var serializedTopics = straigthBracesPattern.Replace(propertyValue, "").Replace("}|{", "},{").Split(",");
                        foreach(var serializedTopic in serializedTopics)
                        {
                            var topicString = serializedTopic.Replace("|",",");
                            if (!topicString.Equals(""))
                            {
                                var topic = new Topic().DeserializeObject(topicString);
                                topics.Add(topic);
                            }
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

        public string Print()
        {
            var topicsPrinted = "Topics:\n";
            foreach(var topic in Topics)
            {
                topicsPrinted += $"\tTopic Name: {topic.Name}\n";
            }
            var filePrinted = File != null ? "\nFile:\n" + File.PrintFile(true) : "";
            return $"Name: {Name}\nContent: {Content}\n{topicsPrinted}CreationDate: '{CreationDate:yyyy-MM-dd HH:mm:ss}'{filePrinted}";
        }

        public override bool Equals(object obj)
        {
            var topic = obj as Post;
            return topic != null && Name == topic.Name;
        }
    }
}
