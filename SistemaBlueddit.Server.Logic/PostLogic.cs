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
            post.CreationDate = _posts.Count > 0 ? _posts.LastOrDefault().CreationDate.AddDays(-1) : post.CreationDate;
            _posts.Add(post);
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
            var post = _posts.Find(post => post.Name == postName);
            post.PrintPost();
        }

        private void PrintPosts(List<Post> posts)
        {
            foreach(var post in posts)
            {
                Console.WriteLine(post.PrintPost());
            }
        }
    }
}
