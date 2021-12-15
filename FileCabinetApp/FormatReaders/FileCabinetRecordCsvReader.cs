using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Validators;

namespace FileCabinetApp.FormatReaders
{
    /// <summary>
    /// Reader For CSV.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private readonly StreamReader reader;
        private readonly IRecordValidator recordValidator = new DefaultValidator();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="reader">Stream reader.</param>
        /// <param name="recordValidator">Validator for reading records from file.</param>
        public FileCabinetRecordCsvReader(StreamReader reader, IRecordValidator recordValidator)
        {
            this.reader = reader;
            this.recordValidator = recordValidator;
        }

        /// <summary>
        /// Reads all data from file.
        /// </summary>
        /// <returns>List if data.</returns>
        public List<FileCabinetRecord> ReadAll()
        {
            _ = this.reader.ReadLine(); // csv header

            return new List<FileCabinetRecord>();
        }
    }
}
