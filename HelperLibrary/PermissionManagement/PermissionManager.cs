using System;
using System.Collections.Generic;
using HelperLibrary.Database;
using HelperLibrary.Database.Exceptions;

namespace HelperLibrary.PermissionManagement
{
    public class PermissionManager
    {
        private static readonly MySqlDatabaseManager DbManager = MySqlDatabaseManager.GetInstance();

        public static bool UserHasPermission(int userId, Permission permission)
        {
            return UserHasPermission(userId, permission.Id);
        }

        public static bool UserHasPermission(int userId, int permissionId)
        {
            string query = "SELECT permission_id " +
                           "FROM " +
                           "( " +
                           "SELECT group_permission_relation.permission_id " +
                           "FROM group_user_relation " +
                           "JOIN group_permission_relation  " +
                           "ON group_user_relation.permission_group_id = group_permission_relation.permission_group_id " +
                           $"WHERE(user_id = {userId} " +
                           $"AND permission_id = {permissionId})) AS grp_permissions " +
                           "UNION " +
                           "SELECT permission_id " +
                           "FROM " +
                           "( " +
                           "SELECT permission_id " +
                           "FROM user_permission_relation " +
                           $"WHERE(user_id = {userId} " +
                           $"AND permission_id = {permissionId})) AS acc_permissions";
            var reader = DbManager.Select(query);

            if (reader.HasRows)
            {
                reader.Close();
                return true;
            }
            reader.Close();
            return false;
        }

        public static bool UserHasPermission(IUser user, Permission permission)
        {
            return UserHasPermission(user.Id, permission.Id);
        }

        public static bool UserHasPermission(IUser user, int permissionId)
        {
            return UserHasPermission(user.Id, permissionId);
        }

        #region Assign Permission to User
        public static void AssignPermissionToUser(int userId, Permission permission)
        {
            AssignPermissionToUser(userId, permission.Id);
        }

        public static void AssignPermissionToUser(int userId, int permissionId)
        {
            try
            {
                string query =
                    $"INSERT INTO user_permission_relation (user_id, permission_id) VALUES ({userId}, {permissionId})";
                DbManager.InsertUpdateDelete(query);
            }
            catch (SQLQueryFailException)
            {
                //Seems that the group aleady have this permission
            }
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
            string query =
                $"DELETE FROM user_permission_relation WHERE user_id = {userId} AND permission_id = {permissionId}";
            DbManager.InsertUpdateDelete(query);
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
            try
            {
                string query =
                    $"INSERT INTO group_permission_relation(permission_group_id, permission_id) VALUES({groupId}, {permissionId})";
                DbManager.InsertUpdateDelete(query);
            }
            catch (SQLQueryFailException)
            {
                //Seems that the group aleady have this permission
            }
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
            string query = 
                $"DELETE FROM group_permission_relation WHERE group_id = {groupId} " +
                $"AND permission_id = {permissionId}";
            DbManager.InsertUpdateDelete(query);
            
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
            string query = $"DELETE FROM group_permission_relation WHERE group_id = {groupId}";
            DbManager.InsertUpdateDelete(query);
        }

        public static void RevokeAllPermissionsFromUser(IUser user)
        {
            RevokeAllPermissionsFromUser(user.Id);
        }

        public static void RevokeAllPermissionsFromUser(int userId)
        {
            string query = 
                $"DELETE FROM user_permission_relation WHERE user_id = {userId}";
            DbManager.InsertUpdateDelete(query);
        }

        public static List<(Permission Permission, bool HasPermission)> GetAllUserPermissions(int userId)
        {
            string query = "SELECT A2.permission_id, name, has_permission " +
                           "FROM( " +
                           "SELECT permission_id, SUM(has_permission) AS has_permission " +
                           "FROM( " +
                           "SELECT permission_id, 0 AS has_permission " +
                           "FROM permissions  " +
                           "UNION ALL " +
                           "SELECT permission_id, 1 AS has_permission " +
                           "FROM user_permission_relation " +
                          $"WHERE(user_id = {userId})) AS inner_table " +
                           "GROUP BY permission_id) A1 " +
                           "LEFT JOIN( " +
                           "SELECT * " +
                           "FROM permissions) AS A2  " +
                           "ON A1.permission_id = A2.permission_id";
            var reader = DbManager.Select(query);

            var permissions = new List<(Permission Permission, bool HasPermission)>();

            while (reader.Read())
            {
                var permissionId = reader.GetInt32(0);
                var permissionName = reader.GetString(1);

                var hasPermission = reader.GetBoolean(2);

                permissions.Add(new ValueTuple<Permission, bool>(
                    new Permission{
                        Id = permissionId,
                        Name = permissionName},
                    hasPermission));
            }

            reader.Close();

            return permissions;
        }        
    }
}