using HelperLibrary.Database.Exceptions;
using System.Collections.Generic;

namespace HelperLibrary.Database
{
    public class MySqlDatabaseManagerV2
    {
        private static readonly Dictionary<string, DatabaseConnection> Instances = new Dictionary<string, DatabaseConnection>();

        public static DatabaseConnection GetInstance(string instanceName)
        {
            return GetInstanceByName(instanceName) ?? throw new InstanceAlreadyExistsException($"Instance with name {instanceName} not found!");
        }      

        private static DatabaseConnection GetInstanceByName(string instanceName)
        {
            foreach (KeyValuePair<string, DatabaseConnection> instance in Instances)
            {
                if (instance.Key == instanceName)
                {
                    return instance.Value;
                }
            }

            return null;
        }

        public void AddInstance(string instanceName, DatabaseConnection databaseConnection)
        {
            if (GetInstanceByName(instanceName) != null)
                throw new InstanceAlreadyExistsException($"Instance with name {instanceName} already exists.");

            Instances.Add(instanceName, databaseConnection);
        }
    }
}
