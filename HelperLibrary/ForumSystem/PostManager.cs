using HelperLibrary.Database;
using System;
using System.Collections.Generic;
using HelperLibrary.Database.Exceptions;
using HelperLibrary.ForumSystem.Enums;

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
            return CreatePost(post.TopicId, post.Content, post.UserId, post.CreateTime);
        }

        public static int CreatePost(int topicId, string content, int userId, DateTime createTime)
        {
            const string query =
                "INSERT INTO posts (topic_id, content, user_id, create_time) VALUES (@topicId, @content, @userId, @createTime)";
            DbManager.PrepareQuery(query);
            DbManager.BindValue("@topicId", topicId);
            DbManager.BindValue("@content", content);
            DbManager.BindValue("@userId", userId);
            DbManager.BindValue("@createTime", createTime);
            DbManager.ExecutePreparedInsertUpdateDelete();

            return DbManager.GetLastId();
        }

        public static void ChangePostContent(int postId, string content)
        {
            const string query = "UPDATE posts SET content = @content WHERE post_id = postId";
            DbManager.PrepareQuery(query);
            DbManager.BindValue("@content", content);
            DbManager.BindValue("@postId", postId);

            DbManager.ExecutePreparedInsertUpdateDelete();
        }

        public static void DeletePost(Post post)
        {
            DeletePost(post.Id);
        }

        public static void DeletePost(int postId)
        {
            string query = $"DELETE FROM posts WHERE post_id = {postId}";
            DbManager.InsertUpdateDelete(query);
        }

        public static void DeletePosts(Topic topic)
        {
            DeletePosts(topic.Id);
        }

        public static void DeletePosts(int topicId)
        {
            string query = $"DELETE FROM posts WHERE topic_id = {topicId}";
            DbManager.InsertUpdateDelete(query);
        }

        public static void AddVoteToPost(Post post, int userId, VoteType type)
        {
            AddVoteToPost(post.Id, userId, type);
        }

        public static void AddVoteToPost(int postId, int userId, VoteType type)
        {
            try
            {
                string query = "INSERT INTO post_votes (post_id, user_id, type) VALUES (@postId, @userId, @type)";
                DbManager.PrepareQuery(query);
                DbManager.BindValue("@postId", postId);
                DbManager.BindValue("@userId", userId);
                DbManager.BindValue("@type", (int) type);

                DbManager.ExecutePreparedInsertUpdateDelete();
            }
            catch (SqlQueryFailedException)
            {
                Console.WriteLine("User maybe already liked/disliked this post");                
            }            
        }

        public static void RemoveVoteFromPost(Post post, int userId)
        {
            RemoveVoteFromPost(post.Id, userId);
        }

        public static void RemoveVoteFromPost(int postId, int userId)
        {
            string query = $"DELETE FROM post_votes WHERE post_id = {postId} AND user_id {userId}";
            DbManager.InsertUpdateDelete(query);
        }

        public static Votes GetVotes(Post post)
        {
            return GetVotes(post.Id);
        }

        public static Votes GetVotes(int postId)
        {
            string query = $"SELECT user_id, type FROM post_votes WHERE post_id = {postId}";
            var reader = DbManager.Select(query);

            var likeUserIds = new List<int>();
            var dislikeUserIds = new List<int>();

            while (reader.Read())
            {
                var userId = reader.GetInt32(0);
                var type = (VoteType) reader.GetInt32(1);

                switch (type)
                {
                    case VoteType.Like:
                        likeUserIds.Add(userId);
                        break;
                    case VoteType.Dislike:
                        dislikeUserIds.Add(userId);
                        break;                        
                }
            }

            reader.Close();

            return new Votes
            {
                DislikeUserIds = dislikeUserIds,
                LikeUserIds = likeUserIds
            };
        }

        public static void RemoveAllVotes(Post post)
        {
            RemoveAllVotes(post.Id);
        }

        public static void RemoveAllVotes(int postId)
        {
            string query = $"DELETE FROM post_votes WHERE post_id = {postId}";
            DbManager.InsertUpdateDelete(query);
        }
    }
}