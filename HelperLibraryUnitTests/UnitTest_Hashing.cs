using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HelperLibraryUnitTests
{
    [TestClass]
    public class UnitTest_Hashing
    {
        [TestMethod]
        public void HashTestMethod()
        {
            string textToHash = "Diesen Text in einen Hash umwandel";

            Console.WriteLine(HelperLibrary.Cryptography.HashManager.HashMD5(textToHash));
            Console.WriteLine(HelperLibrary.Cryptography.HashManager.HashSHA1(textToHash));
            Console.WriteLine(HelperLibrary.Cryptography.HashManager.HashSHA256(textToHash));
            Console.WriteLine(HelperLibrary.Cryptography.HashManager.HashSHA384(textToHash));
            Console.WriteLine(HelperLibrary.Cryptography.HashManager.HashSHA512(textToHash));

            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(HelperLibrary.Cryptography.HashManager.GenerateSecureRandomToken());
            }
        }        

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

