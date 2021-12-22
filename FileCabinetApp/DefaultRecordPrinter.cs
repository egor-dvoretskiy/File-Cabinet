using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp
{
    /// <summary>
    /// Print records.
    /// </summary>
    public class DefaultRecordPrinter : IRecordPrinter
    {
        /// <summary>
        /// Print records in console.
        /// </summary>
        /// <param name="records">Data to print.</param>
        public void Print(IEnumerable<FileCabinetRecord> records)
        {
            foreach (FileCabinetRecord record in records)
            {
                Console.WriteLine(
                        $"#{record.Id}, " +
                        $"{record.FirstName}, " +
                        $"{record.LastName}, " +
                        $"{record.DateOfBirth:yyyy-MMM-dd}, " +
                        $"{record.PersonalRating}, " +
                        $"{record.Debt}, " +
                        $"{record.Gender}.");
            }
        }
    }
}
