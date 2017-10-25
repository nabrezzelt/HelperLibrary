using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace HelperLibrary.FileSystem
{
    public class IniFile
    {
        public readonly string Path;

        [DllImport("kernel32")]
        static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string section, string key, string Default, StringBuilder retVal, int size, string filePath);

        public static IniFile OpenFile()
        {            
            var path = new FileInfo(GetAssembyName() + ".ini").FullName;

            return new IniFile(path);
        }

        public static IniFile OpenFile(string pathToIni)
        {
            var path = new FileInfo(pathToIni).FullName;

            return new IniFile(path);
        }

        private IniFile(string path)
        {
            Path = path;
        }

        public string ReadValue(string key)
        {
            var retVal = new StringBuilder(255);
            GetPrivateProfileString(GetAssembyName(), key, "", retVal, 255, Path);

            return retVal.ToString();
        }

        public string ReadValue(string key, string section)
        {
            var retVal = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", retVal, 255, Path);

            return retVal.ToString();
        }

        public void WriteValue(string key, string value)
        {
            WritePrivateProfileString(GetAssembyName(), key, value, Path);
        }

        public void WriteValue(string key, string value, string section)
        {
            WritePrivateProfileString(section, key, value, Path);
        }     

        public void DeleteKey(string key)
        {
            WriteValue(key, null, GetAssembyName());
        }

        public void DeleteKey(string key, string section)
        {
            WriteValue(key, null, section);
        }

        /// <summary>
        /// Delete all Key-Values in given Section.
        /// </summary>
        /// <param name="section">Section identifier</param>
        public void DeleteSection(string section)
        {
            WriteValue(null, null, section);
        }

        /// <summary>
        /// Delete all values that are not attached to a specific section.
        /// </summary>
        public void DeleteValuesWithoutSection()
        {
            WriteValue(null, null, GetAssembyName());
        }

        public bool KeyExists(string key)
        {
            return ReadValue(key).Length > 0;
        }

        public bool KeyExists(string key, string section)
        {
            return ReadValue(key, section).Length > 0;
        }

        private static string GetAssembyName()
        {
            return Assembly.GetExecutingAssembly().GetName().Name;
        }
    }
}
