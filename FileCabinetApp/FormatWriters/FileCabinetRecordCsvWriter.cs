using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.FormatWriters;

namespace FileCabinetApp
{
    /// <summary>
    /// Contains logics to work with csv.
    /// </summary>
    public class FileCabinetRecordCsvWriter
    {
        private readonly TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="writer">Stream writer.</param>
        public FileCabinetRecordCsvWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Writes to a csv file.
        /// </summary>
        /// <param name="record">Record to write.</param>
        public void Write(FileCabinetRecord record)
        {
            this.writer.WriteLine(WriterAssistant.GetPropertiesValuesString(record));
        }

        /// <summary>
        /// Writes header to csv file.
        /// </summary>
        /// <param name="type">Type of data.</param>
        public void WriteHeader(Type type)
        {
            this.writer.WriteLine(WriterAssistant.GetPropertiesNameString(type));
        }
    }
}
