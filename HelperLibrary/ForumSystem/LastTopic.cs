using System;

namespace HelperLibrary.ForumSystem
{
    public class LastTopic
    {       
        public int TopicId { get; set; }

        public Category Category { get; set; }

        public string TopicTitle { get; set; }

        public int LastPostId { get; set; }

        public int LastPostUserId { get; set; }

        public DateTime LastPostDateTime { get; set; }

        public int PostCount { get; set; }

        public LastTopic(int topicId, Category category, string topicTitle, int lastPostId, int lastPostUserId, DateTime lastPostDateTime, int postCount)
        {
            TopicId = topicId;
            Category = category;
            TopicTitle = topicTitle;
            LastPostId = lastPostId;
            LastPostUserId = lastPostUserId;
            LastPostDateTime = lastPostDateTime;
            PostCount = postCount;
        }
    }
}