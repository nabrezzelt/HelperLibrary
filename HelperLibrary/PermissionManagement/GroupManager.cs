using System;
using System.Collections.Generic;
using HelperLibrary.Database;
using HelperLibrary.Database.Exceptions;

namespace HelperLibrary.PermissionManagement
{
    public abstract class GroupManager
    {
        private static readonly MySqlDatabaseManager DbManager = MySqlDatabaseManager.GetInstance();

        public static List<PermissionGroup> GetAllGroups()
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

        public static int CreateGroup(string groupName)
        {
            const string query = "INSERT INTO (name) permission_groups VALUES (@name)";
            DbManager.PrepareQuery(query);
            DbManager.BindValue("@name", groupName);
            DbManager.ExecutePreparedInsertUpdateDelete();

            return DbManager.GetLastId();
        }

        public static void RenameGroup(PermissionGroup group, string newName)
        {
            RenameGroup(group.Id, newName);
        }

        public static void RenameGroup(int groupId, string newName)
        {
            const string query = "UPDATE permission_groups SET name = @name WHERE permission_group_id = @id";
            DbManager.PrepareQuery(query);
            DbManager.BindValue("@name", newName);
            DbManager.BindValue("@id", groupId);
            DbManager.ExecutePreparedInsertUpdateDelete();
        }

        public static void DeleteGroup(int groupId)
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
    }   
}                                                                                                                             
        