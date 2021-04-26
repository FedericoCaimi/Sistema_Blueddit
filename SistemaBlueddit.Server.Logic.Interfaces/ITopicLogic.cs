using SistemaBlueddit.Domain;
using System.Collections.Generic;

namespace SistemaBlueddit.Server.Logic.Interfaces
{
    public interface ITopicLogic : ILogic<Topic>
    {
        bool ValidateTopics(List<Topic> topics);

        string ModifyTopic(Topic topic);
    }
}
