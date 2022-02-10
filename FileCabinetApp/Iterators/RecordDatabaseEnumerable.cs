using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Services;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Iterator for <see cref="FileCabinetDatabaseService"/> class.
    /// </summary>
    internal class RecordDatabaseEnumerable : IEnumerable<FileCabinetRecord>
    {
        private List<FileCabinetRecord> records = new List<FileCabinetRecord>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordDatabaseEnumerable"/> class.
        /// </summary>
        /// <param name="records">Stored records in database.</param>
        internal RecordDatabaseEnumerable(List<FileCabinetRecord> records)
        {
            this.records = records;
        }

        /// <inheritdoc/>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            var recordEnumerator = new RecordDatabaseEnumerator(this.records);

            while (recordEnumerator.MoveNext())
            {
                yield return recordEnumerator.Current;
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
