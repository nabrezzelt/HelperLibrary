using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HelperLibrary.FileSystem;
using System.IO;
using System.Text;

namespace HelperLibraryUnitTests
{
    [TestClass]
    public class UnitTest_IniFile
    {
        [TestMethod, TestInitialize]
        public void WriteValues()
        {
            IniFile settingsFile = IniFile.OpenFile();
            Assert.IsNotNull(settingsFile);

            settingsFile.WriteValue("Test", "Test");

            settingsFile = IniFile.OpenFile("sections-test.ini");
            Assert.IsNotNull(settingsFile);

            settingsFile.WriteValue("Test", "Test", "TestSection");
        }

        [TestMethod]
        public void ReadValues()
        {
            IniFile settingsFile = IniFile.OpenFile();
            Assert.IsNotNull(settingsFile);

            Assert.AreEqual(settingsFile.ReadValue("Test"), "Test");

            settingsFile = IniFile.OpenFile("sections-test.ini");
            Assert.IsNotNull(settingsFile);

            Assert.AreEqual(settingsFile.ReadValue("Test", "TestSection"), "Test");
        }

        [TestCleanup]
        public void CleanUp()
        {
            File.Delete("sections-test.ini");
            File.Delete("RSAKeyManagement.ini");
        }
    }        
}

