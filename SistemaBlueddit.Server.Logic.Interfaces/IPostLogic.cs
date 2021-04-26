using SistemaBlueddit.Domain;
using System.Collections.Generic;

namespace SistemaBlueddit.Server.Logic.Interfaces
{
    public interface IPostLogic: ILogic<Post>
    {
        string ShowPostsByTopic();

        string ShowPostsByDate();

        string ShowPostsByDateAndTopic();

        string ShowPostsByTopicAndDate();

        string ShowPostByName(string postName);

        string ShowTopicsWithMorePosts(List<Topic> topics);

        void AddFileToPost(BluedditFile file, Post existingPost);

        bool IsTopicInPost(Topic existingTopic);

        BluedditFile GetFileFromPostName(string name);

        string ShowFilesByTopicsOrderByName(List<Topic> topicFiltrer);

        string ShowFilesByTopicsOrderBySize(List<Topic> topicFiltrer);

        string ShowFilesByTopicsOrderByDate(List<Topic> topicFiltrer);

        bool ExistFilesInPosts();

        string ModifyPost(Post postToModify);
    }
}
