using System;
using System.Collections.Generic;
using HelperLibrary.Database;

namespace HelperLibrary.ForumSystem
{
    public class TopicManager
    {
        private static readonly MySqlDatabaseManager DbManager = MySqlDatabaseManager.GetInstance();

        public static Topic GetTopicById(int topicId)
        {
            throw new NotImplementedException();
        }

        public static List<Topic> GetTopicsByCategory(int categoryId)
        {
            throw new NotImplementedException();
        }

        public static int CreateTopic(Topic topic)
        {
            throw new NotImplementedException();
        }

        public static void RenameTopic(int topicId, string newName)
        {
            throw new NotImplementedException();
        }

        public static void DeleteTopic(Topic topic)
        {
            DeleteTopic(topic.Id);
        }

        public static void DeleteTopic(int topicId)
        {
            throw new NotImplementedException();
        }

        public static void DeleteTopics(Category category)
        {
            DeleteTopic(category.Id);
        }

        public static void DeleteTopics(int categoryId)
        {
            throw new NotImplementedException();
        }
    }
}