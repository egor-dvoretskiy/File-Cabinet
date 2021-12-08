using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public static class WriterAssistant
    {
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

                string? addStringValue = value.GetType() != typeof(DateTime) ? value.ToString() : ((DateTime)value).ToString("yyyy-MMM-dd");

                lstStringRecordValues.Add(addStringValue);
            }

            string result = string.Join(",", lstStringRecordValues);

            return result;
        }

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
