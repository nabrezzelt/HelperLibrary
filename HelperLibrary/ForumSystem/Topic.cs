using System;
using System.Collections.Generic;

namespace HelperLibrary.ForumSystem
{
    public class Topic
    {        
        public int Id { get; set; }

        public Category Category { get; set; }

        public string Title { get; set; }

        public int UserId { get; set; }

        public bool Closed { get; set; }

        public bool Sticky { get; set; }

        public DateTime CreateTime { get; set; }

        //public Lazy<List<Post>> Posts => new Lazy<List<Post>>(() => PostManager.GetPosts(Id));
        public List<Post> Posts => PostManager.GetPosts(Id);

        public Topic(int id, Category category, string title, int userId, bool closed, bool sticky, DateTime createTime)
        {
            Id = id;
            Category = category;
            Title = title;
            UserId = userId;
            Closed = closed;
            Sticky = sticky;
            CreateTime = createTime;
        }
    }
}