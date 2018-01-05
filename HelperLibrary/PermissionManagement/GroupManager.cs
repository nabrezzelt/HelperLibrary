using System;
using System.Collections.Generic;
using HelperLibrary.Database;

namespace HelperLibrary.PermissionManagement
{
    public class GroupManager
    {
        private static readonly MySQLDatabaseManager DBManager = MySQLDatabaseManager.GetInstance();

        public static List<PermissionGroup> GetAllGroups()
        {
            throw new NotImplementedException();
        }

        public static PermissionGroup GetPermissionGroupById(int groupId)
        {
            throw new NotImplementedException();
        }

        public static int CreateGroup(string groupName)
        {
            throw new NotImplementedException();
        }

        public static void RenameGroup(PermissionGroup group, string newName)
        {
            RenameGroup(group.Id, newName);
        }

        public static void RenameGroup(int groupId, string newName)
        {
            throw new NotImplementedException();
        }

        public static void DeleteGroup(int groupId)
        {
            PermissionManager.RevokeAllPermissionsFromGroup(groupId);
            throw new NotImplementedException();
        }

        public static void AddUserToGroup(IUser user, PermissionGroup group)
        {
            AddUserToGroup(user.Id, group.Id);
        }

        public static void AddUserToGroup(IUser user, int groupId)
        {
            AddUserToGroup(user.Id, groupId);
        }

        public static void AddUserToGroup(int userId, PermissionGroup group)
        {
            AddUserToGroup(userId, group.Id);
        }

        public static void AddUserToGroup(int userId, int groupId)
        {
            throw new NotImplementedException();
        }
       
        public static void RemoveUserFromGroup(IUser user, PermissionGroup group)
        {
            RemoveUserFromGroup(user.Id, group.Id);
        }

        public static void RemoveUserFromGroup(IUser user, int groupId)
        {
            RemoveUserFromGroup(user.Id, groupId);
        }

        public static void RemoveUserFromGroup(int userId, PermissionGroup group)
        {
            RemoveUserFromGroup(userId, group.Id);
        }

        public static void RemoveUserFromGroup(int userId, int groupId)
        {
            throw new NotImplementedException();
        }
    }
}