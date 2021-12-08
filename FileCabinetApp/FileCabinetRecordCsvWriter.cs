using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetRecordCsvWriter
    {
        private readonly TextWriter writer;

        public FileCabinetRecordCsvWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        public void Write(FileCabinetRecord record)
        {
            this.writer.WriteLine(WriterAssistant.GetPropertiesValuesString(record));
        }

        public void WriteHeader(Type type)
        {
            this.writer.WriteLine(WriterAssistant.GetPropertiesNameString(type));
        }
    }
}
