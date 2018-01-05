namespace HelperLibrary.PermissionManagement
{
    public class Permission
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        protected Permission(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
