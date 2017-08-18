using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace HelperLibrary.FileSystem
{
    public class IniFile
    {
        private string path;

        public string Path { get => path; }

        [DllImport("kernel32")]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public static IniFile OpenFile()
        {            
            var path = new FileInfo(GetAssembyName() + ".ini").FullName.ToString();

            return new IniFile(path);
        }

        public static IniFile OpenFile(string pathToIni)
        {
            var path = new FileInfo(pathToIni).FullName.ToString();

            return new IniFile(path);
        }

        private IniFile(string path)
        {
            this.path = path;
        }

        public string ReadValue(string key)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(GetAssembyName(), key, "", RetVal, 255, path);

            return RetVal.ToString();
        }

        public string ReadValue(string key, string section)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", RetVal, 255, path);

            return RetVal.ToString();
        }

        public void WriteValue(string key, string value)
        {
            WritePrivateProfileString(GetAssembyName(), key, value, path);
        }

        public void WriteValue(string key, string value, string section)
        {
            WritePrivateProfileString(section, key, value, path);
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
