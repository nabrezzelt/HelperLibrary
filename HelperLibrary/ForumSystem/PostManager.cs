using System;
using System.Collections.Generic;
using HelperLibrary.Database;

namespace HelperLibrary.ForumSystem
{
    public class PostManager
    {
        private static readonly MySqlDatabaseManager DbManager = MySqlDatabaseManager.GetInstance();

        public static List<Post> GetPosts(Topic topic)
        {
            throw new NotImplementedException();
        }

        public static List<Post> GetPosts(int topicId)
        {
            throw new NotImplementedException();
        }

        public static Post GetPost(int postId)
        {
            throw new NotImplementedException();
        }

        public static int CreatePost(int post)
        {
            throw new NotImplementedException();
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