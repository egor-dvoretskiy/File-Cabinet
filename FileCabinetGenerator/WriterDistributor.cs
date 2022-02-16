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
            FileCabinetRecord[] records = new FileCabinetRecord[recordAmount];

            for (int i = 0; i < recordAmount; i++)
            {
                records[i] = RecordGenerator.GetRecord();
            }

            XmlWriter.Write(writer, records);
        }

        public static void WriteToSQLDatabase(int recordAmount)
        {
            FileCabinetRecord[] records = new FileCabinetRecord[recordAmount];

            for (int i = 0; i < recordAmount; i++)
            {
                records[i] = RecordGenerator.GetRecord();
            }

            SQLDatabaseWriter.Write(records);
        }

        public static void WriteToNoSQLDatabase(int recordAmount)
        {
            FileCabinetRecord[] records = new FileCabinetRecord[recordAmount];

            for (int i = 0; i < recordAmount; i++)
            {
                records[i] = RecordGenerator.GetRecord();
            }

            NoSQLDatabaseWriter.Write(records);
        }
    }
}
