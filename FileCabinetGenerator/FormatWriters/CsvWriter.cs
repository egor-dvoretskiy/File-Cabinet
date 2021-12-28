using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp;
using FileCabinetApp.FormatWriters;

namespace FileCabinetGenerator.FormatWriters
{
    /// <summary>
    /// Contains logics to work with csv.
    /// </summary>
    public static class CsvWriter
    {
        /// <summary>
        /// Writes to a csv file.
        /// </summary>
        /// <param name="record">Record to write.</param>
        public static void Write(FileCabinetRecord record, StreamWriter writer)
        {
            writer.WriteLine(WriterAssistant.GetPropertiesValuesString(record));
        }

        /// <summary>
        /// Writes header to csv file.
        /// </summary>
        /// <param name="type">Type of data.</param>
        public static void WriteHeader(Type type, StreamWriter writer)
        {
            writer.WriteLine(WriterAssistant.GetPropertiesNameString(type));
        }
    }
}
