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

        public void ShowPosts()
        {
            Console.WriteLine("¿Por que desea filtrar? Elija una opcion");
            Console.WriteLine("1: Tema");
            Console.WriteLine("2: Fecha");
            Console.WriteLine("3: Fecha y Tema");
            Console.WriteLine("4: Tema y Fecha");
            var filterOption = Console.ReadLine();
            var filteredPosts = new List<Post>();
            switch (filterOption)
            {
                case "1":
                    filteredPosts = _posts.OrderBy(post => post.Topics
                        .OrderBy(topic => topic.Name)
                        .Select(topic=>topic.Name)
                        .FirstOrDefault())
                        .ToList();
                    PrintPosts(filteredPosts);
                    break;
                case "2":
                    filteredPosts = _posts.OrderBy(post => post.CreationDate).ToList();
                    PrintPosts(filteredPosts);
                    break;
                case "3":
                    filteredPosts = _posts.OrderBy(post => post.CreationDate)
                        .ThenBy(post => post.Topics
                        .OrderBy(topic => topic.Name)
                        .Select(topic => topic.Name)
                        .FirstOrDefault())
                        .ToList();
                    PrintPosts(filteredPosts);
                    break;
                case "4":
                    filteredPosts = _posts.OrderBy(post => post.Topics
                        .OrderBy(topic => topic.Name)
                        .Select(topic => topic.Name)
                        .FirstOrDefault())
                        .ThenBy(post => post.CreationDate)
                        .ToList();
                    PrintPosts(filteredPosts);
                    break;
                default:
                    Console.WriteLine("Opcion invalida...");
                    break;
            }                        
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
