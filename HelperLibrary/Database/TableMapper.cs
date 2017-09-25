using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace HelperLibrary.Database
{
    class TableMapper<T> where T : new()
    {
        public static List<T> MapReaderToTable(MySqlDataReader reader)
        {
            List<T> tableItems = new List<T>();

            while(reader.Read())
            {
                T tableRow = new T();

                foreach (var prop in typeof(T).GetProperties())
                {
                    object[] attributes = prop.GetCustomAttributes(true);

                    string columnName = prop.Name;

                    foreach (object attribute in attributes)
                    {
                        ColumnNameAttribute columnNameAttr = attribute as ColumnNameAttribute;

                        if(columnNameAttr != null && String.IsNullOrEmpty(columnNameAttr.ColumnName))
                        {
                            columnName = columnNameAttr.ColumnName;
                        }
                    }                    

                    tableRow.GetType().GetProperty(prop.Name).SetValue(prop, Convert.ChangeType(reader[columnName], prop.PropertyType), null);                                        
                }
                
                tableItems.Add(tableRow);
            }

            reader.Close();
            return tableItems;
        }
    }
}
