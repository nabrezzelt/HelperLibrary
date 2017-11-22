using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace HelperLibrary.Database
{
    public class TableMapper
    {
        public static List<T> MapToList<T>(MySqlDataReader reader)
        {
            List<T> tableRows = new List<T>();

            while (reader.Read())
            {
                var row = Activator.CreateInstance<T>();                
                
                foreach (PropertyInfo property in typeof(T).GetProperties())
                {
                    string propertyName = property.Name;
                    string columnName = propertyName;

                    var propertyAttributes = property.GetCustomAttributes();

                    foreach (Attribute propertyAttribute in propertyAttributes)
                    {
                        if (propertyAttribute is ColumnNameAttribute attribute)
                        {
                            if (!string.IsNullOrEmpty(attribute.ColumnName))
                                columnName = attribute.ColumnName;
                        }
                    }

                    property.SetValue(row, Convert.ChangeType(reader[columnName], property.PropertyType), null);
                }

                tableRows.Add(row);
            }

            reader.Close();

            return tableRows;
        }

        public static T MapToObject<T>(MySqlDataReader reader)
        {
            var row = Activator.CreateInstance<T>();

            reader.Read();

            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                string propertyName = property.Name;
                string columnName = propertyName;

                var propertyAttributes = property.GetCustomAttributes();

                foreach (Attribute propertyAttribute in propertyAttributes)
                {
                    if (propertyAttribute is ColumnNameAttribute attribute)
                    {
                        if (!string.IsNullOrEmpty(attribute.ColumnName))
                            columnName = attribute.ColumnName;
                    }
                }

                property.SetValue(row, Convert.ChangeType(reader[columnName], property.PropertyType), null);
            }

            reader.Close();

            return row;
        }
    }
}
