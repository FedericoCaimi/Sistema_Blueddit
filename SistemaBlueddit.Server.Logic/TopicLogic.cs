using SistemaBlueddit.Domain;
using System.Collections.Generic;
using System.Linq;

namespace SistemaBlueddit.Server.Logic
{
    public class TopicLogic: Logic<Topic>
    {
        public bool ValidateTopics(List<Topic> topics)
        {
            var areTopicsValid = true;
            if(topics.Count == 0)
                return false;
            foreach(var topic in topics)
            {
                if (!_elements.Exists(t => t.Equals(topic)))
                {
                    areTopicsValid = false;
                    break;
                }
            }
            return areTopicsValid;
        }

        public string ModifyTopic(Topic topic)
        {
            var existingTopic = GetByName(topic.Name);
            if(existingTopic != null)
            {
                var topicIndex = _elements.FindIndex(t => t.Equals(topic));
                _elements[topicIndex] = topic;
                return "El tema se modifico con exito";
            }
            else
            {
                return "El tema no existe";
            }
        }

        public override Topic GetByName(string name)
        {
            return _elements.FirstOrDefault(t => t.Name.Equals(name));
        }
    }
}
