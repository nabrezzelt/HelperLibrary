using System;
using System.Collections.Generic;
using HelperLibrary.Database;
using HelperLibrary.Database.Exceptions;

namespace HelperLibrary.PermissionManagement
{
    public abstract class GroupManager
    {
        private static readonly MySqlDatabaseManager DbManager = MySqlDatabaseManager.GetInstance();

        public static List<PermissionGroup> GetAllPermissionGroups()
        {
            const string query = "SELECT * FROM permission_groups";
            var reader = DbManager.Select(query);

            var groups = new List<PermissionGroup>();

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var name = reader.GetString(1);

                groups.Add(new PermissionGroup(id, name));
            }

            reader.Close();

            return groups;
        }

        public static PermissionGroup GetPermissionGroupById(int groupId)
        {
            string query = "SELECT * " +
                           "FROM permission_groups " +
                          $"WHERE permission_group_id = {groupId}";
            var reader = DbManager.Select(query);            

            reader.Read();

            PermissionGroup group = null;

            if (reader.HasRows)
            {
                var id = reader.GetInt32(0);
                var name = reader.GetString(1);

                group = new PermissionGroup(id, name);                
            }
                            
            reader.Close();

            return group;
        }       

        public static int CreatePermissionGroup(string groupName)
        {
            const string query = "INSERT INTO (name) permission_groups VALUES (@name)";
            DbManager.PrepareQuery(query);
            DbManager.BindValue("@name", groupName);
            DbManager.ExecutePreparedInsertUpdateDelete();

            return DbManager.GetLastId();
        }

        public static void RenamePermissionGroup(PermissionGroup group, string newName)
        {
            RenamePermissionGroup(group.Id, newName);
        }

        public static void RenamePermissionGroup(int groupId, string newName)
        {
            const string query = "UPDATE permission_groups SET name = @name WHERE permission_group_id = @id";
            DbManager.PrepareQuery(query);
            DbManager.BindValue("@name", newName);
            DbManager.BindValue("@id", groupId);
            DbManager.ExecutePreparedInsertUpdateDelete();
        }

        public static void DeletePermissionGroup(int groupId)
        {
            PermissionManager.RevokeAllPermissionsFromGroup(groupId);
            RemoveAllUsersFromGroup(groupId);

            string query = $"DELETE FROM permission_groups WHERE permission_group_id = {groupId}";
            DbManager.InsertUpdateDelete(query);
        }

        public static void RemoveAllUsersFromGroup(int groupId)
        {
            string query = $"DELETE FROM permission_group_user_relation WHERE permission_group_id = {groupId}";
            DbManager.InsertUpdateDelete(query);
        }

        public static void RemoveAllUsersFromGroup(PermissionGroup group)
        {
            RemoveAllUsersFromGroup(group.Id);
        }

        public static void AddUserToPermissionGroup(IUser user, PermissionGroup group)
        {
            AddUserToPermissionGroup(user.Id, group.Id);
        }

        public static void AddUserToPermissionGroup(IUser user, int groupId)
        {
            AddUserToPermissionGroup(user.Id, groupId);
        }

        public static void AddUserToPermissionGroup(int userId, PermissionGroup group)
        {
            AddUserToPermissionGroup(userId, group.Id);
        }

        public static void AddUserToPermissionGroup(int userId, int groupId)
        {
            try
            {
                string query = 
                    $"INSERT INTO group_user_relation (permission_group_id, user_id) VALUES ({groupId}, {userId})";
                DbManager.InsertUpdateDelete(query);
            }
            catch (SqlQueryFailedException)
            {
                //Seems that the user is already member of this group
            }
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
            string query =
                $"DELETE FROM group_user_relation WHERE permission_group_id = {groupId} AND user_id = {userId}";
            DbManager.InsertUpdateDelete(query);
        }

        public List<(Permission Permission, bool HasPermission)> GetAllGroupPermissions(PermissionGroup group)
        {
            return GetAllGroupPermissions(group.Id);
        }

        public List<(Permission Permission, bool HasPermission)> GetAllGroupPermissions(int groupId)
        {
            string query = "SELECT A2.permission_id, name, has_permission " +
                           "FROM( " +
                           "SELECT permission_id, SUM(has_permission) AS has_permission " +
                           "FROM( " +
                           "SELECT permission_id, 0 AS has_permission " +
                           "FROM permissions  " +
                           "UNION ALL " +
                           "SELECT permission_id, 1 AS has_permission " +
                           "FROM group_permission_relation " +            
                           $"WHERE(permission_group_id = {groupId})) AS inner_table " +
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
                    new Permission
                    {
                        Id = permissionId,
                        Name = permissionName
                    },
                    hasPermission));
            }

            reader.Close();

            return permissions;            
        }

        public static List<PermissionGroup> GetAssignedPermissionGroups(int userId)
        {
            string query = "SELECT permission_groups.*  " +
                           "FROM group_user_relation " +
                           "JOIN permission_groups " +
                           "ON group_user_relation.permission_group_id = permission_groups.permission_group_id " +
                          $"WHERE user_id = {userId}";
            var reader = DbManager.Select(query);

            var groups = new List<PermissionGroup>();

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var name = reader.GetString(1);

                groups.Add(new PermissionGroup(id, name));
            }

            reader.Close();

            return groups;
        }

        public static List<PermissionGroup> GetAssignedPermissionGroups(IUser user)
        {
            return GetAssignedPermissionGroups(user.Id);
        }

        public static List<PermissionGroup> GetUnassignedPermissionGroups(int userId)
        {
            string query = "SELECT permission_groups.permission_group_id, name " +
                           "FROM permission_groups " +
                           "LEFT JOIN( " +
                           "SELECT user_id, permission_group_id " +
                           "FROM group_user_relation " +
                           $"WHERE user_id = {userId}) AS A " +
                           "ON permission_groups.permission_group_id = A.permission_group_id " +
                           "WHERE user_id IS NULL";
            var reader = DbManager.Select(query);

            var groups = new List<PermissionGroup>();

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var name = reader.GetString(1);

                groups.Add(new PermissionGroup(id, name));
            }

            reader.Close();

            return groups;
        }

        public static List<PermissionGroup> GetUnassignedPermissionGroups(IUser user)
        {
            return GetUnassignedPermissionGroups(user.Id);
        }
    }   
}                                                                                                                             
        