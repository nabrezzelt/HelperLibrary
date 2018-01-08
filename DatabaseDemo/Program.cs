using System;
using System.Collections.Generic;
using HelperLibrary.Database;

namespace DatabaseDemo
{
    public class Program
    {
        private static MySQLDatabaseManager _dbManagerDefault;
        private static MySQLDatabaseManager _dbManagerAuth;
        private static void Main(string[] args)
        {
            InitializeDBConnection();
            RecreateUserTable();

            Console.ReadLine();
        }

        public static void RecreateUserTable()
        {
            string query = "DROP TABLE IF EXISTS users";
            _dbManagerAuth.InsertUpdateDelete(query);

            query = "CREATE TABLE `users` ( " +
                        "`id` INT(11) NOT NULL AUTO_INCREMENT, " +
                        "`username` VARCHAR(64) NOT NULL, " +
                        "PRIMARY KEY(`id`) " +
                    ") ENGINE = InnoDB";
            _dbManagerAuth.InsertUpdateDelete(query);   
            
            Console.WriteLine("Usertable recreated!");
        }

        private static void InitializeDBConnection()
        {
            //Create first instance for default DB:
            MySQLDatabaseManager.CreateInstance();
            _dbManagerDefault = MySQLDatabaseManager.GetInstance();

            _dbManagerDefault.SetConnectionString("localhost", "root", "123465", "test-default");
            _dbManagerDefault.Connect();
            _dbManagerDefault.SQLQueryExecuted += OnSQLQueryExecuted;
            Console.WriteLine("Default-DB connected!");

            //Create second instance for Auth-DB:
            MySQLDatabaseManager.CreateInstance("Auth");
            _dbManagerAuth = MySQLDatabaseManager.GetInstance("Auth");

            _dbManagerAuth.SetConnectionString("localhost", "root", "123465", "test-auth");
            _dbManagerAuth.Connect();
            _dbManagerAuth.SQLQueryExecuted += OnSQLQueryExecuted;
            Console.WriteLine("Auth-DB connected!");            
        }

        private static void OnSQLQueryExecuted(object sender, SQLQueryEventArgs e)
        {
            Console.WriteLine($"[TYPE: {e.Type}] {e.Query}");
        }

        public static void SelectWithoutPrepare()
        {
            var userID = 1;
            string query = "";
        }

        public static void SelectWithPrepare()
        {
            var filter = "test";

            const string query = "SELECT * FROM users WHERE username LIKE @filter";
            _dbManagerAuth.PrepareQuery(query);
            _dbManagerAuth.BindValue("@filter", $"%{filter}%");
            var reader = _dbManagerAuth.ExecutePreparedSelect();

            var foundUsers = new List<User>();

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var username = reader.GetString(1);
            }
        }

        public static void PreparedInsertStatement()
        {
            Dictionary<int, string> usersToCreate = new Dictionary<int, string>
            {
                { 1, "User 1" },
                { 2, "TestUser 2" },
                { 3, "User 3" },
                { 4, "TestUser 4" }
            };

            const string insertQuery = "INSERT INTO users (@id, @username)";
            _dbManagerAuth.PrepareQuery(insertQuery);

            foreach (KeyValuePair<int, string> userToCreate in usersToCreate)
            {
                _dbManagerAuth.BindValue("@id", userToCreate.Key);
                _dbManagerAuth.BindValue("@username", userToCreate.Value);
                _dbManagerAuth.ExecutePreparedInsertUpdateDelete();
            }
        }
    }

    public class User
    {      
        public int Id { get; set; }
        public string Username { get; set; }


        public User(int id, string username)
        {
            Id = id;
            Username = username;
        }

        public override string ToString()
        {
            return $"{Username} has id: {Id}";
        }
    }
}
