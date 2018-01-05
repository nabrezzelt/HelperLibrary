namespace HelperLibrary.PermissionManagement
{
    public interface IUser 
    {
        int Id { get; set; }              

        bool HasPermission(Permission permission);
    }
}