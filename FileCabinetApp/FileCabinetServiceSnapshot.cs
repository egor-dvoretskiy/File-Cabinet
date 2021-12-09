using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// holds snapshot for FileCabinetService.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="list">Stocked list of records.</param>
        public FileCabinetServiceSnapshot(List<FileCabinetRecord> list)
        {
            this.records = list.ToArray();
        }

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
    }
}
