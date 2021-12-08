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

        public FileCabinetServiceSnapshot(List<FileCabinetRecord> list)
        {
            this.records = list.ToArray();
        }

        public void SaveToCsv(StreamWriter writer)
        {
            var csvWriter = new FileCabinetRecordCsvWriter(writer);

            csvWriter.WriteHeader(typeof(FileCabinetRecord));

            for (int i = 0; i < this.records.Length; i++)
            {
                csvWriter.Write(this.records[i]);
            }
        }
    }
}
