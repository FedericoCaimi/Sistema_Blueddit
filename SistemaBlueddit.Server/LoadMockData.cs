using SistemaBlueddit.Domain;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SistemaBlueddit.Server
{
    public static class LoadMockData
    {


        public static List<Topic> LoadTopicsMockData()
        {            
            var topic1 = new Topic
            {
                Name = "tema1",
                Description = "descripcionTema1"
            };
            var topic2 = new Topic
            {
                Name = "tema2",
                Description = "descripcionTema2"
            };
            var topic3 = new Topic
            {
                Name = "tema3",
                Description = "descripcionTema3"
            };
            return new List<Topic>
            {
                topic1, topic2, topic3
            };
        }

        public static List<Post> LoadPostsMockData(List<Topic> mockedTopicsData)
        {
            var post1 = new Post
            {
                Name = "post1",
                Content = "contentPost1",
                Topics = new List<Topic> { mockedTopicsData[2] },
                CreationDate = DateTime.Now
            };
            var post2 = new Post
            {
                Name = "post2",
                Content = "contentPost2",
                Topics = new List<Topic> { mockedTopicsData[0] },
                CreationDate = DateTime.Now.AddSeconds(1)
            };
            var post3 = new Post
            {
                Name = "post3",
                Content = "contentPost3",
                Topics = new List<Topic> { mockedTopicsData[1] },
                CreationDate = DateTime.Now.AddSeconds(2)
            };
            return new List<Post>
            {
                post1, post2, post3
            };
        }
    }
}
