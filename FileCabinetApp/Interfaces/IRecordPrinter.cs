using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Responsible for print records.
    /// </summary>
    public interface IRecordPrinter
    {
        /// <summary>
        /// Print records.
        /// </summary>
        /// <param name="records">Data to print.</param>
        void Print(IEnumerable<FileCabinetRecord> records);
    }
}
