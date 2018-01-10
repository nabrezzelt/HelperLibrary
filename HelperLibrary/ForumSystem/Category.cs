namespace HelperLibrary.ForumSystem
{
    public class Category
    {
        public int Id { get; set; }

        public int? ParentCategoryId  { get; set; }

        public bool HasParentCategory => ParentCategoryId is null;

        public bool HasSubCategories => CategoryManager.CategoryHasSubCategories(Id);

        public string Name { get; set; }
    }
}
