using System;
using System.Collections.Generic;
using HelperLibrary.Database;
using HelperLibrary.PermissionManagement;

namespace HelperLibrary.ForumSystem
{
    public class PostManager
    {
        private static readonly MySqlDatabaseManager DbManager = MySqlDatabaseManager.GetInstance();

        public static List<Post> GetPosts(Topic topic)
        {
            return GetPosts(topic.Id);
        }

        public static List<Post> GetPosts(int topicId)
        {
            string query = $"SELECT post_id, content, user_id, create_time FROM posts WHERE topic_id = {topicId}";
            var reader = DbManager.Select(query);

            var posts = new List<Post>();

            while (reader.Read())
            {
                var postId = reader.GetInt32(0);
                var content = reader.GetString(1);
                var userId = reader.GetInt32(2);
                var createTime = reader.GetDateTime(3);                

                posts.Add(new Post(postId, topicId, content, userId, createTime));
            }

            reader.Close();

            return posts;
        }

        public static Post GetPost(int postId)
        {
            string query = $"SELECT topic_id, content, user_id, create_time FROM posts WHERE post_id = {postId}";
            var reader = DbManager.Select(query);

            reader.Read();

            Post post = null;

            if (reader.HasRows)
            {
                var topicId = reader.GetInt32(0);
                var content = reader.GetString(1);
                var userId = reader.GetInt32(2);
                var createTime = reader.GetDateTime(3);

                post = new Post(postId, topicId, content, userId, createTime);
            }

            reader.Close();

            return post;
        }

        public static int CreatePost(Post post)
        {
            const string query =
                "INSERT INTO posts (topic_id, content, user_id, create_time) VALUES (@topicId, @content, @userId, @createTime)";
            DbManager.PrepareQuery(query);
            DbManager.BindValue("@topicId", post.TopicId);
            DbManager.BindValue("@content", post.Content);
            DbManager.BindValue("@userId", post.UserId);
            DbManager.BindValue("@createTime", post.CreateTime);
            DbManager.ExecutePreparedInsertUpdateDelete();

            return DbManager.GetLastID();
        }

        public static int ChangePostContent(int postId, string content)
        {
            throw new NotImplementedException();
        }

        public static void DeletePost(Post post)
        {
            DeletePost(post.Id);
        }

        public static void DeletePost(int postId)
        {

            throw new NotImplementedException();
        }

        public static void DeletePosts(Topic topic)
        {
            DeletePosts(topic.Id);
        }

        public static void DeletePosts(int topicId)
        {
            throw new NotImplementedException();
        }

        public static void AddLikeDislikeToPost(Post post, int userId, PostVoteType type)
        {
            throw new NotImplementedException();
        }

        public static void AddLikeDislikeToPost(int postId, int userId, PostVoteType type)
        {
            throw new NotImplementedException();
        }

        public static void RemoveLikeDislikeFromPost(Post post, int userId)
        {
            throw new NotImplementedException();
        }

        public static void RemoveLikeDislikeFromPost(int postId, int userId)
        {
            throw new NotImplementedException();
        }

        public static (int Likes, int Dislikes) GetLikesAndDislikes()
        {
            throw new NotImplementedException();
        }
    }
}