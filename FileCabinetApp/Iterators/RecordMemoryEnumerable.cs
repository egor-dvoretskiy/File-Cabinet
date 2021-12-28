using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Services;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Iterator for <see cref="FileCabinetMemoryService"/> class.
    /// </summary>
    public class RecordMemoryEnumerable : IEnumerable<FileCabinetRecord>
    {
        private List<FileCabinetRecord> records;
        private List<int> listPositionsId = new List<int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordMemoryEnumerable"/> class.
        /// </summary>
        /// <param name="records">List of records.</param>
        /// <param name="listPositionsId">Stored parameter's id.</param>
        public RecordMemoryEnumerable(List<FileCabinetRecord> records, List<int> listPositionsId)
        {
            this.records = records;
            this.listPositionsId = listPositionsId;
        }

        /// <inheritdoc/>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            var recordEnumerator = new RecordMemoryEnumerator(this.records, this.listPositionsId);

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
