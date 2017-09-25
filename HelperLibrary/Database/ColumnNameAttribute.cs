using System;

namespace HelperLibrary.Database
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class ColumnNameAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236

        public ColumnNameAttribute(string positionalString)
        {
            ColumnName = positionalString;            
        }

        public string ColumnName { get; }
    }
}
