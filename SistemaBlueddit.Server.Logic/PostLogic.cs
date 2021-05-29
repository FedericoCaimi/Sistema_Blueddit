using SistemaBlueddit.Domain;
using SistemaBlueddit.Server.Logic.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SistemaBlueddit.Server.Logic
{
    public class PostLogic: Logic<Post>, IPostLogic
    {
        public string ShowPostsByTopic(){
            var filteredPosts = new List<Post>();
            filteredPosts = _elements.OrderBy(post => post.Topics
                        .OrderBy(topic => topic.Name)
                        .Select(topic=>topic.Name)
                        .FirstOrDefault())
                        .ToList();
            return ShowMultiple(filteredPosts);
        }

        public string ShowPostsByDate(){
            var filteredPosts = new List<Post>();
            filteredPosts = _elements.OrderBy(post => post.CreationDate).ToList();
            return ShowMultiple(filteredPosts);
        }

        public string ShowPostsByDateAndTopic(){
            var filteredPosts = new List<Post>();
            filteredPosts = _elements.OrderBy(post => post.CreationDate)
                        .ThenBy(post => post.Topics
                        .OrderBy(topic => topic.Name)
                        .Select(topic => topic.Name)
                        .FirstOrDefault())
                        .ToList();
            return ShowMultiple(filteredPosts);
        }

        public string ShowPostsByTopicAndDate(){
            var filteredPosts = new List<Post>();
            filteredPosts = _elements.OrderBy(post => post.Topics
                        .OrderBy(topic => topic.Name)
                        .Select(topic => topic.Name)
                        .FirstOrDefault())
                        .ThenBy(post => post.CreationDate)
                        .ToList();
            return ShowMultiple(filteredPosts);
        }

        public string ShowPostByName(string postName)
        {
            var post = _elements.Where(post => post.Name == postName).FirstOrDefault();
            return post != null ? Show(post) : "";
        }

        public string ShowTopicsWithMorePosts(List<Topic> topics)
        {
            var returnedTopics = TopicsWithMorePosts(topics);
            var topicsToReturn = "";
            foreach (var t in returnedTopics)
            {
                topicsToReturn += t.Print() + "\n";
            }
            return topicsToReturn;
        }

        private List<Topic> TopicsWithMorePosts(List<Topic> topics){
            var returnedTopics = new List<Topic>();
            var maxNumberOfPosts = 0;
            var numberOfPosts = 0;
            foreach (var topic in topics)
            {
                numberOfPosts = _elements.Count(p => p.Topics.Contains(topic));
                if(numberOfPosts > maxNumberOfPosts){
                    maxNumberOfPosts = numberOfPosts;
                    returnedTopics.Clear();
                    returnedTopics.Add(topic);
                }else if(numberOfPosts == maxNumberOfPosts){
                    returnedTopics.Add(topic);
                }
            }
            
            return returnedTopics;
        }

        public void AddFileToPost(BluedditFile file, Post existingPost)
        {
            existingPost.File = file;
        }

        public BluedditFile GetFileFromPostName(string name)
        {
            var post = _elements.Where(p => p.Name.Equals(name)).FirstOrDefault();
            return post != null ? post.File : null;
        }

        public string ShowFilesByTopicsOrderByName(List<Topic> topicFiltrer){
            var posts = new List<Post>();
            posts = _elements.FindAll(p => HasTopics(topicFiltrer, p));
            posts.OrderBy(p => p.File.FileName);
            var filesToReturn = "";
            foreach (var post in posts)
            {
                if(post.File != null)
                {
                    filesToReturn += post.File.PrintFile(true) + "\n";
                }
            }
            return filesToReturn;
        }

        public string ShowFilesByTopicsOrderBySize(List<Topic> topicFiltrer){
            var posts = new List<Post>();
            posts = _elements.FindAll(p => HasTopics(topicFiltrer, p));
            posts.OrderBy(p => p.File.FileSize);
            var filesToReturn = "";
            foreach (var post in posts)
            {
                if (post.File != null)
                {
                    filesToReturn += post.File.PrintFile(true) + "\n";
                }
            }
            return filesToReturn;
        }
        public string ShowFilesByTopicsOrderByDate(List<Topic> topicFiltrer){
            var posts = new List<Post>();
            posts = _elements.FindAll(p => HasTopics(topicFiltrer, p));
            posts.OrderBy(p => p.File.CreationDate);
            var filesToReturn = "";
            foreach (var post in posts)
            {
                if (post.File != null)
                {
                    filesToReturn += post.File.PrintFile(true) + "\n";
                }
            }
            return filesToReturn;
        }

        private bool HasTopics(List<Topic> topics, Post p)
        {
            var haveTopics = true;
            foreach (var topic in topics)
            {
                var haveTopic = p.Topics.Contains(topic);
                if(!haveTopic){
                    haveTopics = false;
                    break;
                }
            }
            return haveTopics;
        }

        public bool ExistFilesInPosts()
        {
            var existFiles = false;
            foreach(var post in _elements)
            {
                existFiles = post.File != null;
                if (existFiles)
                {
                    break;
                }
            }
            return existFiles;
        }

        public bool IsTopicInPost(Topic existingTopic)
        {
            return _elements.FirstOrDefault(p => p.Topics.Contains(existingTopic)) != null;
        }

        public string ModifyPost(Post postToModify)
        {
            var existingPost = GetByName(postToModify.Name);
            if (existingPost != null)
            {
                existingPost.Name = postToModify.Name;
                existingPost.Content = postToModify.Content;
                existingPost.Topics = postToModify.Topics;
                return $"Post {existingPost.Name} modificado con exito";
            }
            else
            {
                return "No existe el post con el nombre ingresado.";
            }
        }

        public override Post GetByName(string name)
        {
            return _elements.FirstOrDefault(t => t.Name.Equals(name));
        }
    }
}
