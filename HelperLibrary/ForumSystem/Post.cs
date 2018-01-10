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
    }
}