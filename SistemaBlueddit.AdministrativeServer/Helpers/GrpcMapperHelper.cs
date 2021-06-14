using Google.Protobuf.Collections;
using SistemaBlueddit.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaBlueddit.AdministrativeServer.Helpers
{
    public static class GrpcMapperHelper
    {
        public static List<TopicInPost> ToGrpcTopics(List<Topic> topics)
        {
            var grpcTopics = new List<TopicInPost>();
            foreach (var topic in topics)
            {
                var grpcTopic = new TopicInPost
                {
                    Name = topic.Name,
                    Description = topic.Description
                };
                grpcTopics.Add(grpcTopic);
            }
            return grpcTopics;
        }

        public static List<Post> ToDomainPost(RepeatedField<PostResponse.Types.Post> grpcPosts)
        {
            var posts = new List<Post>();
            foreach (var grpcPost in grpcPosts)
            {
                var post = new Post
                {
                    Name = grpcPost.Name,
                    Content = grpcPost.Content,
                    CreationDate = grpcPost.CreationDate.ToDateTime().ToLocalTime(),
                    Topics = ToDomainTopic(grpcPost.Topics)
                };
                posts.Add(post);
            }
            return posts;
        }

        private static List<Topic> ToDomainTopic(RepeatedField<TopicInPost> grpcTopics)
        {
            var topics = new List<Topic>();
            foreach (var grpcTopic in grpcTopics)
            {
                var topic = new Topic
                {
                    Name = grpcTopic.Name,
                    Description = grpcTopic.Description
                };
                topics.Add(topic);
            }
            return topics;
        }
    }
}
