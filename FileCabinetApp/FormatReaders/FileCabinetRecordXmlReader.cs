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
    /// Class provides Xml file reading.
    /// </summary>
    public class FileCabinetRecordXmlReader
    {
        private readonly StreamReader reader;
        private readonly IRecordValidator recordValidator = new DefaultValidator();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="reader">Stream reader.</param>
        /// <param name="validator">Record validator.</param>
        public FileCabinetRecordXmlReader(StreamReader reader, IRecordValidator validator)
        {
            this.reader = reader;
            this.recordValidator = validator;
        }

        /// <summary>
        /// Reads all data from xml file.
        /// </summary>
        /// <returns>List of data.</returns>
        public List<FileCabinetRecord> ReadAll()
        {


            return new List<FileCabinetRecord>();
        }
    }
}
