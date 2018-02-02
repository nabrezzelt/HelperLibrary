using System;

namespace HelperLibrary.ForumSystem
{
    public class Post
    {        
        public int Id { get; set; }

        public int TopicId { get; set; }

        public string Content { get; set; }

        public int UserId { get; set; }

        public (int Likes, int Dislikes) LikesDislikesUserIds => PostManager.GetLikesAndDislikes();

        public DateTime CreateTime { get; set; }

        public Post(int id, int topicId, string content, int userId, DateTime createTime)
        {
            Id = id;
            TopicId = topicId;
            Content = content;
            UserId = userId;
            CreateTime = createTime;
        }
    }
}