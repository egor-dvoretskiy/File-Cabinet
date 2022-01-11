using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace FileCabinetApp.FormatWriters
{
    /// <summary>
    /// Class contains additional methods to work with writers.
    /// </summary>
    public static class ReflectedRecordParams
    {
        /// <summary>
        /// Gets properties values in class using reflection.
        /// </summary>
        /// <param name="record">Neccessary type to get values in.</param>
        /// <returns>Concatenated properties values.</returns>
        public static string GetPropertiesValuesString(FileCabinetRecord record)
        {
            var t = record.GetType().GetProperties();

            List<string?> lstStringRecordValues = new ();

            for (int i = 0; i < t.Length; i++)
            {
                var value = t[i].GetValue(record, null);

                if (value is null)
                {
                    continue;
                }

                string? addStringValue = value.GetType() != typeof(DateTime) ? value.ToString().Replace(",", ".") : ((DateTime)value).ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);

                lstStringRecordValues.Add(addStringValue);
            }

            string result = string.Join(",", lstStringRecordValues);

            return result;
        }

        /// <summary>
        /// Gets properties names in class using reflection.
        /// </summary>
        /// <param name="type">Neccessary type to get names in.</param>
        /// <returns>Concatenated properties values.</returns>
        public static string GetPropertiesNameString(Type type)
        {
            var t = type.GetProperties();

            List<string> lstStringRecordValues = new ();

            for (int i = 0; i < t.Length; i++)
            {
                lstStringRecordValues.Add(t[i].Name);
            }

            string result = string.Join(",", lstStringRecordValues);

            return result;
        }
    }
}
