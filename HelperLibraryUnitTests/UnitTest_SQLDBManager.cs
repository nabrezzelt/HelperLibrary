using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HelperLibrary.FileSystem;
using HelperLibrary.Database;
using HelperLibrary.Cryptography;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;

namespace HelperLibraryUnitTests
{
    [TestClass]
    public class UnitTest_SQLDBManager
    {
        //[TestMethod, TestInitialize]
        //public void ConnectTestMethod()
        //{
        //    var instance = MySQLDatabaseManager.GetInstance();
        //    instance.SQLQueryExcecuted += OnSQLQueryExcecuted;
        //    Assert.IsNotNull(instance);

        //    instance.SetConnectionString("127.0.0.1", "root", "ascent", "test");

        //    instance.Connect();
        //}

        //private void OnSQLQueryExcecuted(object sender, SQLQueryEventArgs e)
        //{           
        //    Console.WriteLine("Type: " + e.Type + "\nQuery: " + e.Query);
        //}
       
        //[TestMethod]
        //public void SelectAndInsert()
        //{
        //    var instance = MySQLDatabaseManager.GetInstance();

        //    int id = 1;
        //    string query = "SELECT name FROM test WHERE id = " + id;
        //    string query_to_prepare = "SELECT name FROM test WHERE id = @id";                       

        //    if(instance.IsConnected())
        //    {
        //        MySqlDataReader reader = instance.Select(query);
        //        reader.Read();
        //        Assert.IsTrue(reader.GetString(0) == "Hans");
        //        reader.Close();

        //        instance.PrepareQuery(query_to_prepare);
        //        instance.BindValue("@id", id);
        //        reader = instance.ExecutePreparedSelect();

        //        reader.Read();
        //        Assert.IsTrue(reader.GetString(0) == "Peter");
        //        reader.Close();

        //        instance.BindValue("@id", 3);
        //        reader = instance.ExecutePreparedSelect();

        //        reader.Read();
        //        Assert.IsTrue(reader.GetString(0) == "Peter");
        //        reader.Close();
        //    }
        //    else
        //    {
        //        Assert.Fail();
        //    }
        //}
    }
}

