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
            const string query = "SELECT category_id, name FROM categories WHERE parent_category_id IS NULL";
            var reader = DbManager.Select(query);

            var categories = new List<Category>();

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var name = reader.GetString(1);

                var cat = new Category(id, name);
                categories.Add(cat);
            }

            reader.Close();

            return categories;
        }

        public static Category GetCategory(int categoryId)
        {
            string query = $"SELECT parent_category_id, name FROM categories WHERE category_id = {categoryId}";
            var reader = DbManager.Select(query);

            reader.Read();

            if (!reader.HasRows)
            {
                reader.Close();
                return null;
            }

            int? parentCategoryId = reader.IsDBNull(0) ? (int?) null : reader.GetInt32(0);
            var name = reader.GetString(1);

            reader.Close();

            return new Category(categoryId, parentCategoryId, name);
        }

        public static List<Category> GetSubCategories(int categoryId)
        {
            string query = $"SELECT category_id, name FROM categories WHERE parent_category_id = {categoryId}";
            var reader = DbManager.Select(query);

            var categories = new List<Category>();

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var parentCategoryId = categoryId;
                var categoryName = reader.GetString(1);
                                                
                categories.Add(new Category(id, parentCategoryId, categoryName));
            }

            reader.Close();

            return categories;
        }

        public static bool CategoryHasSubCategories(int categoryId)
        {
            string query = $"SELECT count(category_id) FROM categories WHERE category_id = {categoryId}";
            var reader = DbManager.Select(query);

            int count = 0;

            reader.Read();

            if (reader.HasRows)
            {
                count = reader.GetInt32(0);                                
            }

            reader.Close();
            return count > 0;
        }

        public static int CreateCategory(Category category)
        {
            const string query = "INSERT INTO categories (parent_category_id, name) VALUES (@parentCategoryId, @name)";
            DbManager.PrepareQuery(query);
            DbManager.BindValue("@parentCategoryId", category.ParentCategoryId);
            DbManager.BindValue("@name", category.Name);
            DbManager.ExecutePreparedInsertUpdateDelete();

            return DbManager.GetLastID();
        }

        public static void RenameCategory(int categoryId, string name)
        {
            const string query = "UPDATE categories SET name = @name WHERE categoryId = @categoryId";
            DbManager.PrepareQuery(query);            
            DbManager.BindValue("@name", name);
            DbManager.BindValue("@categoryId", categoryId);

            DbManager.ExecutePreparedInsertUpdateDelete();
        }

        public static void ChangeParentCategory(int categoryId, int newParentCategoryId)
        {
            string query =
                $"UPDATE categories SET parent_category_id = {newParentCategoryId} WHERE category_id = {categoryId}";
            DbManager.InsertUpdateDelete(query);
        }

        public static void DeleteCategory(Category category)
        {
            DeleteCategory(category.Id);
        }

        public static void DeleteCategory(int categoryId)
        {
            TopicManager.DeleteTopics(categoryId);

            if(CategoryHasSubCategories(categoryId))
            {
                foreach (var subCategory in GetSubCategories(categoryId))
                {
                    DeleteCategory(subCategory);
                }
            }

            string query = $"DELETE FROM categories WHERE category_id = {categoryId}";
            DbManager.InsertUpdateDelete(query);
        }
    }
}