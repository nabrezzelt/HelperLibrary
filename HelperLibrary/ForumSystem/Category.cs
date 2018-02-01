namespace HelperLibrary.ForumSystem
{
    public class Category
    {        
        public int Id { get; set; }

        public int? ParentCategoryId  { get; set; }

        public bool HasParentCategory => ParentCategoryId is null;

        public bool HasSubCategories => CategoryManager.CategoryHasSubCategories(Id);

        public string Name { get; set; }

        public Category(int id, string name)
        {
            Id = id;
            ParentCategoryId = null;
            Name = name;
        }

        public Category(int id, int? parentCategoryId, string name)
        {
            Id = id;
            ParentCategoryId = parentCategoryId;
            Name = name;
        }
    }
}
