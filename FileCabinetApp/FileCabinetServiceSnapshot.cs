using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using FileCabinetApp.FormatReaders;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// holds snapshot for FileCabinetService.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly IRecordValidator recordValidator;
        private FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="recordValidator">Validator for import file.</param>
        /// <param name="list">Stocked list of records.</param>
        public FileCabinetServiceSnapshot(List<FileCabinetRecord> list, IRecordValidator recordValidator)
        {
            this.records = list.ToArray();
            this.recordValidator = recordValidator;
        }

        /// <summary>
        /// Gets stored records.
        /// </summary>
        /// <value>
        /// Stored records.
        /// </value>
        public ReadOnlyCollection<FileCabinetRecord> Records { get; private set; } = new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());

        /// <summary>
        /// Save records to csv file.
        /// </summary>
        /// <param name="writer">Stream Writer.</param>
        public void SaveToCsv(StreamWriter writer)
        {
            var csvWriter = new FileCabinetRecordCsvWriter(writer);

            csvWriter.WriteHeader(typeof(FileCabinetRecord));

            for (int i = 0; i < this.records.Length; i++)
            {
                csvWriter.Write(this.records[i]);
            }
        }

        /// <summary>
        /// Save records to xml file.
        /// </summary>
        /// <param name="writer">Stream Writer.</param>
        public void SaveToXml(StreamWriter writer)
        {
            var xmlWriter = new FileCabinetRecordXmlWriter(writer);

            xmlWriter.Write(this.records);
        }

        /// <summary>
        /// Loads data from csv file using FileCabinetRecordCsvReader.
        /// </summary>
        /// <param name="reader">Stream reader.</param>
        public void LoadFromCsv(StreamReader reader)
        {
            FileCabinetRecordCsvReader csvReader = new FileCabinetRecordCsvReader(reader, this.recordValidator);

            var records = csvReader.ReadAll();
            this.Records = new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <summary>
        /// Loads data from xml file using FileCabinetRecordXmlReader.
        /// </summary>
        /// <param name="reader">Stream reader.</param>
        public void LoadFromXml(StreamReader reader)
        {
            FileCabinetRecordXmlReader xmlReader = new FileCabinetRecordXmlReader(reader, this.recordValidator);

            var records = xmlReader.ReadAll();
            this.Records = new ReadOnlyCollection<FileCabinetRecord>(records);
        }
    }
}
