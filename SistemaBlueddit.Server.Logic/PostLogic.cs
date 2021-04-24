using SistemaBlueddit.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace SistemaBlueddit.Server.Logic
{
    public class PostLogic
    {
        private List<Post> _posts;

        public PostLogic()
        {
            _posts = new List<Post>();
        }

        public Post RecievePost(Header header, NetworkStream stream)
        {
            var data = new byte[header.DataLength];
            stream.Read(data, 0, header.DataLength);
            var postJson = Encoding.UTF8.GetString(data);
            var post = new Post();
            return post.DeserializeObject(postJson);
        }

        public void AddPost(Post post)
        {
            _posts.Add(post);
        }

        public void ClearPosts()
        {
            _posts = new List<Post>();
        }

        public void AddPosts(List<Post> posts)
        {
            _posts.AddRange(posts);
        }

        public void ShowPostsByTopic(){
            var filteredPosts = new List<Post>();
            filteredPosts = _posts.OrderBy(post => post.Topics
                        .OrderBy(topic => topic.Name)
                        .Select(topic=>topic.Name)
                        .FirstOrDefault())
                        .ToList();
                    PrintPosts(filteredPosts);
        }

        public void ShowPostsByDate(){
            var filteredPosts = new List<Post>();
            filteredPosts = _posts.OrderBy(post => post.CreationDate).ToList();
                    PrintPosts(filteredPosts);
        }

        public void ShowPostsByDateAndTopic(){
            var filteredPosts = new List<Post>();
            filteredPosts = _posts.OrderBy(post => post.CreationDate)
                        .ThenBy(post => post.Topics
                        .OrderBy(topic => topic.Name)
                        .Select(topic => topic.Name)
                        .FirstOrDefault())
                        .ToList();
                    PrintPosts(filteredPosts);
        }

        public void ShowPostsByTopicAndDate(){
            var filteredPosts = new List<Post>();
            filteredPosts = _posts.OrderBy(post => post.Topics
                        .OrderBy(topic => topic.Name)
                        .Select(topic => topic.Name)
                        .FirstOrDefault())
                        .ThenBy(post => post.CreationDate)
                        .ToList();
                    PrintPosts(filteredPosts);
        }

        public void ShowPostByName(string postName)
        {
            var post = _posts.Where(post => post.Name == postName).FirstOrDefault();
            Console.WriteLine(post.PrintPost());
        }

        public void ShowTopicsWithMorePosts(List<Topic> topics)
        {
            var returnedTopics = TopicsWithMorePosts(topics);
            foreach (var t in returnedTopics)
            {
                Console.WriteLine(t.PrintTopic());
            }
        }

        private List<Topic> TopicsWithMorePosts(List<Topic> topics){
            var returnedTopics = new List<Topic>();
            var maxNumberOfPosts = 0;
            var numberOfPosts = 0;
            foreach (var topic in topics)
            {
                numberOfPosts = _posts.Count(p => p.Topics.Contains(topic));
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

        private void PrintPosts(List<Post> posts)
        {
            foreach(var post in posts)
            {
                Console.WriteLine(post.PrintPost());
            }
        }

        public Post GetPostWithName(Header header, NetworkStream networkStream)
        {
            var data = new byte[header.DataLength];
            networkStream.Read(data, 0, header.DataLength);
            var postName = Encoding.UTF8.GetString(data);
            return _posts.FirstOrDefault(p => p.Name.Equals(postName));
        }

        public void ValidatePost(Post post)
        {
            if (_posts.Exists(p => p.Name.Equals(post.Name)))
            {
                throw new Exception("Error. No se pueden agregar posts duplicados");
            }
        }

        public void AddFileToPost(BluedditFile file, Post existingPost)
        {
            existingPost.File = file;
        }

        public BluedditFile GetFileFromPostName(string name)
        {
            var post = _posts.Where(p => p.Name.Equals(name)).FirstOrDefault();
            return post != null ? post.File : null;
        }

        public void ShowFilesByTopicsOrderByName(List<Topic> topicFiltrer){
            var posts = new List<Post>();
            posts = _posts.FindAll(p => haveTopics(topicFiltrer, p));
            posts.OrderBy(p => p.File.FileName);
            foreach (var post in posts)
            {
                if(post.File != null) 
                    Console.WriteLine(post.File.PrintFile(true));
            }
        }

        public void ShowFilesByTopicsOrderBySize(List<Topic> topicFiltrer){
            var posts = new List<Post>();
            posts = _posts.FindAll(p => haveTopics(topicFiltrer, p));
            posts.OrderBy(p => p.File.FileSize);
            foreach (var post in posts)
            {
                if(post.File != null) 
                    Console.WriteLine(post.File.PrintFile(true));
            }
        }
        public void ShowFilesByTopicsOrderByDate(List<Topic> topicFiltrer){
            var posts = new List<Post>();
            posts = _posts.FindAll(p => haveTopics(topicFiltrer, p));
            posts.OrderBy(p => p.File.CreationDate);
            foreach (var post in posts)
            {
                if(post.File != null) 
                    Console.WriteLine(post.File.PrintFile(true));
            }
        }

        private bool haveTopics(List<Topic> topics, Post p)
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
    }
}
