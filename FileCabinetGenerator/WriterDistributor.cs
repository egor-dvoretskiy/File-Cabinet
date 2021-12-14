using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetGenerator.FormatWriters;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    public static class WriterDistributor
    {
        public static void WriteToCsv(StreamWriter writer, int recordAmount)
        {
            CsvWriter.WriteHeader(typeof(FileCabinetRecord), writer);

            for (int i = 0; i < recordAmount; i++)
            {
                FileCabinetRecord record = RecordGenerator.GetRecord();

                CsvWriter.Write(record, writer);
            }
        }

        public static void WriteToXml(StreamWriter writer, int recordAmount)
        {
            
        }
    }
}
