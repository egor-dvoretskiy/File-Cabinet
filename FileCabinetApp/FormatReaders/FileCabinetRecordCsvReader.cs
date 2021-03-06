using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileCabinetApp.Interfaces;
using FileCabinetApp.ServiceTools;

namespace FileCabinetApp.FormatReaders
{
    /// <summary>
    /// Reader For CSV.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private readonly StreamReader reader;
        private readonly IRecordValidator recordValidator;

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
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            _ = this.reader.ReadLine(); // csv header

            while (true)
            {
                var csvLine = this.reader.ReadLine();

                if (csvLine is null)
                {
                    break;
                }

                var lineParameters = csvLine.Split(",");

                FileCabinetRecord record = new FileCabinetRecord();

                record.Id = InputConverter.IdConverter(lineParameters[0]).Item3;
                record.FirstName = InputConverter.StringConverter(lineParameters[1]).Item3;
                record.LastName = InputConverter.StringConverter(lineParameters[2]).Item3;
                record.DateOfBirth = InputConverter.BirthDateConverter(lineParameters[3]).Item3;
                record.PersonalRating = InputConverter.PersonalRatingConverter(lineParameters[4]).Item3;
                record.Salary = InputConverter.SalaryConverter(lineParameters[5]).Item3;
                record.Gender = InputConverter.GenderConverter(lineParameters[6]).Item3;

                bool isValid = this.recordValidator.ValidateParameters(record);

                if (isValid)
                {
                    records.Add(record);
                }
            }

            return records;
        }
    }
}
