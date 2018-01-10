using System;
using System.Collections.Generic;

namespace HelperLibrary.ForumSystem
{
    public class Topic
    {
        public int Id { get; set; }

        public int Title { get; set; }

        public int UserId { get; set; }

        public DateTime CreateTime { get; set; }

        public List<Post> Posts => PostManager.GetPosts(Id);
    }
}