using System;
using System.Collections.Generic;
using HelperLibrary.Database;

namespace HelperLibrary.ForumSystem
{
    public class CategoryManager
    {
        private static readonly MySqlDatabaseManager DbManager = MySqlDatabaseManager.GetInstance();

        public static List<Category> GetFirstHierarchyCategories()
        {
            throw new NotImplementedException();
        }

        public static Category GetCategory(int categoryId)
        {
            throw new NotImplementedException();
        }

        public static List<Category> GetCategories(int categoryId)
        {
            throw new NotImplementedException();
        }

        public static bool CategoryHasSubCategories(int categoryId)
        {
            throw new NotImplementedException();
        }

        public static int CreateCategory(Category category)
        {
            throw new NotImplementedException();
        }

        public static void RenameCategory(int categoryId, string name)
        {
            throw new NotImplementedException();
        }

        public static void DeleteCategory(Category category)
        {
            DeleteCategory(category.Id);
        }

        public static void DeleteCategory(int categoryId)
        {
            TopicManager.DeleteTopics(categoryId);
            throw new NotImplementedException();
        }
    }
}