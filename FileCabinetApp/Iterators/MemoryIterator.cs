using System;
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
    public class MemoryIterator : IRecordIterator
    {
        private List<FileCabinetRecord> records;
        private int position = 0;
        private List<int> listPositionsId = new List<int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryIterator"/> class.
        /// </summary>
        /// <param name="records">List of records.</param>
        /// <param name="listPositionsId">Stored parameter's id.</param>
        public MemoryIterator(List<FileCabinetRecord> records, List<int> listPositionsId)
        {
            this.records = records;
            this.listPositionsId = listPositionsId;
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetNext()
        {
            int positionInList = this.listPositionsId[this.position++];

            return this.records[positionInList];
        }

        /// <inheritdoc/>
        public bool HasMore()
        {
            bool hasMore = this.position < this.listPositionsId.Count;
            return hasMore;
        }
    }
}
