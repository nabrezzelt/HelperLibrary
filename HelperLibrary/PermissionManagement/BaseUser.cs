using System;
using System.Collections.Generic;

namespace HelperLibrary.PermissionManagement
{
    public class BaseUser : IUser
    {
        public int Id { get; set; }        

        public bool HasPermission(Permission permission)
        {
            return PermissionManager.UserHasPermission(Id, permission);
        }

        public (Permission Permission, bool HasPermission) GetAllUserPermissions()
        {
            throw new NotImplementedException();
        }

        public void JoinPermissionGroup(PermissionGroup group)
        {
            JoinPermissionGroup(group.Id);
        }

        public void JoinPermissionGroup(int groupId)
        {
            GroupManager.AddUserToPermissionGroup(Id, groupId);
        }

        public void LeavePermissionGroup(int groupId)
        {
            GroupManager.RemoveUserFromGroup(Id, groupId);
        }

        public void LeavePermissionGroup(PermissionGroup group)
        {
            LeavePermissionGroup(group.Id);
        }

        public List<PermissionGroup> GetAssignedPermissionGroups()
        {
            return GroupManager.GetAssignedPermissionGroups(Id);
        }

        public List<PermissionGroup> GetUnassignedPermissionGroups()
        {
            return GroupManager.GetUnassignedPermissionGroups(Id);
        }        
    }
}
