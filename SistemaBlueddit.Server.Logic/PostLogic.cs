using SistemaBlueddit.Domain;
using System;
using System.Collections.Generic;
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
    }
}
