using HelperLibrary.Database;
using System;
using System.Collections.Generic;

namespace HelperLibrary.ForumSystem
{
    public static class TopicManager
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

            return DbManager.GetLastId();
        }

        public static void ChangeTopicTitle(int topicId, string newTitle)
        {
            const string query = "UPDATE topics SET title = @title WHERE topic_id = @topicId";
            DbManager.PrepareQuery(query);            
            DbManager.BindValue("@title", newTitle);
            DbManager.BindValue("@topicId", topicId);
            DbManager.ExecutePreparedInsertUpdateDelete();
        }

        public static void ChangeTopicCategory(int topicId, int newCategoryId)
        {
            string query = $"UPDATE topics SET category_id = {newCategoryId} WHERE topic_id = {topicId}";
            DbManager.InsertUpdateDelete(query);
        }

        public static void DeleteTopic(Topic topic)
        {
            DeleteTopic(topic.Id);
        }

        public static void DeleteTopic(int topicId)
        {
            PostManager.DeletePosts(topicId);

            string query = $"DELETE FROM topics WHERE topic_id = {topicId}";
            DbManager.InsertUpdateDelete(query);
        }

        public static void DeleteTopics(Category category)
        {
            DeleteTopic(category.Id);
        }

        public static void DeleteTopics(int categoryId)
        {            
            foreach (var topic in GetTopicsByCategory(categoryId))
            {
                DeleteTopic(topic);
            }
        }

        public static List<LastTopic> GetLatestTopics(int max)
        {            
            if (max <= 0)
                throw new ArgumentOutOfRangeException(nameof(max));

            string query = "SELECT A.topic_id, AllowedCategoryId.category_id, AllowedCategoryId.parent_category_id, AllowedCategoryId.category_name, A.title, A.post_id,  A.user_id, A.create_time, A.anzahl AS post_count " +
                           "FROM( " +
                              "SELECT TopicIdNameAnz.title, TopicIdNameAnz.anzahl, TopicIdLastDatePosterId.create_time, TopicIdLastDatePosterId.user_id, TopicIdNameAnz.topic_id, TopicIdLastDatePosterId.post_id " +
                                  "FROM( " +
                                  "SELECT TopicIdLastDate.create_time, posts.user_id, TopicIdLastDate.tId, posts.post_id " +
                                  "FROM( " +
                                      "SELECT topic_id AS tId, MAX(create_time) AS create_time " +
                                      "FROM `posts` " +
                                      "GROUP BY topic_id) AS TopicIdLastDate " +
                                  "JOIN `posts` " +
                                  "ON (TopicIdLastDate.tId = posts.topic_id AND TopicIdLastDate.create_time = posts.create_time)) AS TopicIdLastDatePosterId " +
                              "JOIN " +
                              "( " +
                                  "SELECT TopicIdName.topic_id, TopicIdName.title, COUNT(*) AS anzahl " +
                                  "FROM( " +
                                      "SELECT topic_id, title " +
                                      "FROM `topics` " +
                                      "GROUP BY topic_id, title) AS TopicIdName " +
                                  "JOIN `posts`  " +
                                  "ON (TopicIdName.topic_id = posts.topic_id) " +
                                  "GROUP BY TopicIdName.topic_id,TopicIdName.title) AS TopicIdNameAnz  " +
                              "ON (TopicIdNameAnz.topic_id = TopicIdLastDatePosterId.tId)) AS A " +
                           "INNER JOIN " +
                           "( " +
                               "SELECT topics.topic_id, B.category_id, B.parent_category_id, B.category_name " +
                               "FROM ( " + 
                                   "SELECT category_id, parent_category_id, name AS category_name  " +
		                           "FROM `categories`) AS B " +
	                           "JOIN `topics` ON (topics.category_id = B.category_id)) AS AllowedCategoryId  " +
                           "ON (A.topic_id = AllowedCategoryId.topic_id) " +
                           "ORDER BY create_time DESC " +
                          $"LIMIT 0, {max}";
            var reader = DbManager.Select(query);

            var latetstTopics = new List<LastTopic>();

            while (reader.Read())
            {
                var topicId = reader.GetInt32(0);
                var categoryId = reader.GetInt32(1);
                var parentCategoryId = reader.GetInt32(2);
                var categoryName = reader.GetString(3);
                var topicTitle = reader.GetString(4);
                var lastPostId = reader.GetInt32(5);
                var postUserId = reader.GetInt32(6);
                var postCreateTime = reader.GetDateTime(7);
                var postCount = reader.GetInt32(8);

                var category = new Category(categoryId, parentCategoryId, categoryName);                

                latetstTopics.Add(new LastTopic(topicId, category, topicTitle, lastPostId, postUserId, postCreateTime, postCount));
            }

            reader.Close();

            return latetstTopics;
        }

        public static void CloseTopic(int topicId)
        {
            string query = $"UPDATE topics SET closed = 1 WHERE topic_id = {topicId}";
            DbManager.InsertUpdateDelete(query);
        }

        public static void CloseTopic(Topic topic)
        {
            CloseTopic(topic.Id);
        }

        public static void ReOpenTopic(int topicId)
        {
            string query = $"UPDATE topics SET closed = 0 WHERE topic_id = {topicId}";
            DbManager.InsertUpdateDelete(query);
        }

        public static void ReOpenTopic(Topic topic)
        {
            ReOpenTopic(topic.Id);
        }

        public static void MarkTopicAsSticky(int topicId)
        {
            string query = $"UPDATE topics SET sticky = 1 WHERE topic_id = {topicId}";
            DbManager.InsertUpdateDelete(query);
        }

        public static void MarkTopicAsSticky(Topic topic)
        {
            MarkTopicAsSticky(topic.Id);
        }

        public static void MarkTopicAsNotSticky(int topicId)
        {
            string query = $"UPDATE topics SET sticky = 0 WHERE topic_id = {topicId}";
            DbManager.InsertUpdateDelete(query);
        }

        public static void MarkTopicAsNotSticky(Topic topic)
        {
            MarkTopicAsNotSticky(topic.Id);
        }
    }
}