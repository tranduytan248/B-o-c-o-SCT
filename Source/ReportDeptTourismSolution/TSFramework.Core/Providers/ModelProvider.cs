using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace TSFramework.Core.Providers
{
    public class ModelProvider
    {
        public static T CreateModelFromRow<T>(DataRow row) where T : new()
        {
            // create a new object
            var item = new T();

            // set the item
            SetDataFromRow(item, row);

            // return 
            return item;
        }

        public static void SetDataFromRow<T>(T item, DataRow row) where T : new()
        {
            // go through each column
            foreach (DataColumn c in row.Table.Columns)
            {
                // find the property for the column
                var p = item.GetType().GetProperty(c.ColumnName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

                // if exists, set the value
                if (p != null && row[c] != DBNull.Value) p.SetValue(item, row[c], null);
            }
        }

        public static List<T> CreateListFromTable<T>(DataTable tbl) where T : new()
        {
            // define return list
            var lst = new List<T>();

            // go through each row
            foreach (DataRow r in tbl.Rows)
                // add to the list
                lst.Add(CreateModelFromRow<T>(r));

            // return the list
            return lst;
        }

        public static DataTable ToDataTable<T>(IList<T> data)
        {
            var props = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            for (var i = 0; i < props.Count; i++)
            {
                var prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            var values = new object[props.Count];
            foreach (var item in data)
            {
                for (var i = 0; i < values.Length; i++) values[i] = props[i].GetValue(item);
                table.Rows.Add(values);
            }

            return table;
        }
    }
}