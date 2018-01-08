using System;
using HelperLibrary.Database;

namespace HelperLibrary.PermissionManagement
{
    public class PermissionManager
    {
        private static readonly MySqlDatabaseManager DBManager = MySqlDatabaseManager.GetInstance();

        public static bool HasPermission(int userId, Permission permission)
        {
            return HasPermission(userId, permission.Id);
        }

        public static bool HasPermission(int userId, int permissionId)
        {
            throw new NotImplementedException();
        }

        public static bool HasPermission(IUser user, Permission permission)
        {
            return HasPermission(user.Id, permission.Id);
        }

        public static bool HasPermission(IUser user, int permissionId)
        {
            return HasPermission(user.Id, permissionId);
        }

        #region Assign Permission to User
        public static void AssignPermissionToUser(int userId, Permission permission)
        {
            AssignPermissionToUser(userId, permission.Id);
        }

        public static void AssignPermissionToUser(int userId, int permissionId)
        {
            throw new NotImplementedException();
        }

        public static void AssignPermissionToUser(IUser user, Permission permission)
        {
            AssignPermissionToUser(user.Id, permission.Id);
        }

        public static void AssignPermissionToUser(IUser user, int permissionId)
        {
            AssignPermissionToUser(user.Id, permissionId);
        }
        #endregion

        #region Revoke Permission from User
        public static void RevokePermissionFromUser(int userId, Permission permission)
        {
            RevokePermissionFromUser(userId, permission.Id);
        }

        public static void RevokePermissionFromUser(int userId, int permissionId)
        {
            throw new NotImplementedException();
        }

        public static void RevokePermissionFromUser(IUser user, Permission permission)
        {
            RevokePermissionFromUser(user.Id, permission.Id);
        }

        public static void RevokePermissionFromUser(IUser user, int permissionId)
        {
            RevokePermissionFromUser(user.Id, permissionId);
        }

        #endregion

        #region Assign Permission to Group
        public static void AssignPermissionToGroup(int groupId, Permission permission)
        {
            AssignPermissionToGroup(groupId, permission.Id);
        }

        public static void AssignPermissionToGroup(int groupId, int permissionId)
        {
            throw new NotImplementedException();
        }

        public static void AssignPermissionToGroup(PermissionGroup group, Permission permission)
        {
            AssignPermissionToGroup(group.Id, permission.Id);
        }

        public static void AssignPermissionToGroup(PermissionGroup group, int permissionId)
        {
            AssignPermissionToGroup(group.Id, permissionId);
        }
        #endregion

        #region Revoke Permission from Group
        public static void RevokePermissionFromGroup(int groupId, Permission permission)
        {
            RevokePermissionFromGroup(groupId, permission.Id);
        }

        public static void RevokePermissionFromGroup(int groupId, int permissionId)
        {
            throw new NotImplementedException();
        }

        public static void RevokePermissionFromGroup(PermissionGroup group, Permission permission)
        {
            RevokePermissionFromGroup(group.Id, permission.Id);
        }

        public static void RevokePermissionFromGroup(PermissionGroup group, int permissionId)
        {
            RevokePermissionFromGroup(group.Id, permissionId);
        }
        #endregion

        public static void RevokeAllPermissionsFromGroup(PermissionGroup group)
        {
            RevokeAllPermissionsFromGroup(group.Id);
        }

        public static void RevokeAllPermissionsFromGroup(int groupId)
        {
            throw new NotImplementedException();
        }

        public static void RevokeAllPermissionsFromUsers(IUser user)
        {
            RevokeAllPermissionsFromUsers(user.Id);
        }

        public static void RevokeAllPermissionsFromUsers(int userId)
        {
            throw new NotImplementedException();
        }
        
    }
}