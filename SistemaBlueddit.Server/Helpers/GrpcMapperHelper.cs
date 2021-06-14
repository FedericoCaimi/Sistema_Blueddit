using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using SistemaBlueddit.Domain;
using System.Collections.Generic;

namespace SistemaBlueddit.Server.Helpers
{
    public class GrpcMapperHelper
    {
        public static List<TopicResponse.Types.Topic> ToGrpcTopics(List<Topic> topics)
        {
            var grpcTopics = new List<TopicResponse.Types.Topic>();
            foreach (var topic in topics)
            {
                var grpcTopic = new TopicResponse.Types.Topic
                {
                    Name = topic.Name,
                    Description = topic.Description
                };
                grpcTopics.Add(grpcTopic);
            }
            return grpcTopics;
        }

        public static List<PostResponse.Types.Post> ToGrpcPosts(List<Post> posts)
        {
            var grpcPosts = new List<PostResponse.Types.Post>();
            foreach (var post in posts)
            {
                var grpcPost = new PostResponse.Types.Post
                {
                    Name = post.Name,
                    Content = post.Content,
                    Topics = { ToGrpcTopicsInPost(post.Topics) },
                    CreationDate = Timestamp.FromDateTime(post.CreationDate.ToUniversalTime())
                };
                grpcPosts.Add(grpcPost);
            }
            return grpcPosts;
        }

        public static List<Topic> ToTopicList(RepeatedField<TopicInPost> grpcTopics)
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

        private static List<TopicInPost> ToGrpcTopicsInPost(List<Topic> topics)
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
    }
}
