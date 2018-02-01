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
            //ToDo: Check if given category exists
            string query = "SELECT topics.category_id, categories.parent_category_id, categories.name AS category_name, title, user_id, closed, sticky, create_time " +
                           "FROM topics " +
                           "JOIN categories " +
                           "ON topics.category_id = categories.category_id " +
                           $"WHERE topic_id = {topicId}";
            var reader = DbManager.Select(query);

            reader.Read();

            if (reader.HasRows)
            {
                var categoryId = reader.GetInt32(0);
                var parentCategoryId = reader.GetInt32(1);
                var categoryName = reader.GetString(2);
                var topicTitle = reader.GetString(3);
                var topicUserId = reader.GetInt32(4);
                var isTopicClosed = reader.GetBoolean(5);
                var isTopicSticky = reader.GetBoolean(6);
                var topicCreateTime = reader.GetDateTime(7);

                var category = new Category(categoryId, parentCategoryId, categoryName);

                reader.Close();
                return new Topic(topicId, category, topicTitle, topicUserId, isTopicClosed, isTopicSticky, topicCreateTime);
            }

            reader.Close();
            return null;
        }

        public static List<Topic> GetTopicsByCategory(int categoryId)
        {
            string query = "SELECT topic_id, categories.parent_category_id, categories.name AS category_name, title, user_id, closed, sticky, create_time  " +
                           "FROM topics " +
                           "JOIN categories " +
                           "ON topics.category_id = categories.category_id  " +
                           $"WHERE topics.category_id = {categoryId}";
            var reader = DbManager.Select(query);

            var topics = new List<Topic>();

            while (reader.Read())
            {
                var topicId = reader.GetInt32(0);
                var parentCategoryId = reader.GetInt32(1);
                var categoryName = reader.GetString(2);
                var topicTitle = reader.GetString(3);
                var topicUserId = reader.GetInt32(4);
                var isTopicClosed = reader.GetBoolean(5);
                var isTopicSticky = reader.GetBoolean(6);
                var topicCreateTime = reader.GetDateTime(7);

                var category = new Category(categoryId, parentCategoryId, categoryName);
                
                topics.Add(new Topic(topicId, category, topicTitle, topicUserId, isTopicClosed, isTopicSticky,
                    topicCreateTime));
            }

            reader.Close();

            return topics;
        }

        public static int CreateTopic(Topic topic)
        {
            const string query =
                "INSERT INTO topics (parent_category_id, title, user_id, closed, sticky, create_time) VALUES (@parent_category_id, @title, @user_id, @closed, @sticky, @create_time)";
            DbManager.PrepareQuery(query);
            DbManager.BindValue("@parent_category_id", topic.Category.Id);
            DbManager.BindValue("@title", topic.Title);
            DbManager.BindValue("@closed", topic.Closed);
            DbManager.BindValue("@sticky", topic.Sticky);
            DbManager.BindValue("@create_time", topic.CreateTime);
            DbManager.ExecutePreparedInsertUpdateDelete();

            return DbManager.GetLastID();
        }

        public static void ChangeTopicTitle(int topicId, string newTitle)
        {
            const string query = "UPDATE topics SET title = @title WHERE topic_id = @topicId";
            DbManager.PrepareQuery(query);            
            DbManager.BindValue("@title", newTitle);
            DbManager.BindValue("@topicId", topicId);
            DbManager.ExecutePreparedInsertUpdateDelete();
        }

        public static void DeleteTopic(Topic topic)
        {
            DeleteTopic(topic.Id);
        }

        public static void DeleteTopic(int topicId)
        {
            PostManager.DeletePosts(topicId);
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

        public static List<LastTopic> GetLatestTopics(int max)
        {
            throw new NotImplementedException();
        }

        public static void CloseTopic(int topicId)
        {
            throw new NotImplementedException();
        }

        public static void CloseTopic(Topic topic)
        {
            throw new NotImplementedException();
        }

        public static void ReOpenTopic(int topicId)
        {
            throw new NotImplementedException();
        }

        public static void ReOpenTopic(Topic topic)
        {
            throw new NotImplementedException();
        }

        public static void MarkTopicAsSticky(int topicId)
        {
            throw new NotImplementedException();
        }

        public static void MarkTopicAsSticky(Topic topic)
        {
            MarkTopicAsSticky(topic.Id);
        }

        public static void MarkTopicAsNotSticky(int topicId)
        {
            throw new NotImplementedException();
        }

        public static void MarkTopicAsNotSticky(Topic topic)
        {
            MarkTopicAsNotSticky(topic.Id);
        }
    }
}