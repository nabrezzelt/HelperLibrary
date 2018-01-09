namespace HelperLibrary.PermissionManagement
{
    public class PermissionGroup
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public PermissionGroup(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}