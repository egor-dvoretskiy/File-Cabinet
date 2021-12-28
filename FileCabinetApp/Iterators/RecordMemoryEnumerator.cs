using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Memory enumerator for records.
    /// </summary>
    internal class RecordMemoryEnumerator : IEnumerator<FileCabinetRecord>
    {
        private List<FileCabinetRecord> records;
        private int position = -1;
        private List<int> listPositionsId;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordMemoryEnumerator"/> class.
        /// </summary>
        /// <param name="records">List of records.</param>
        /// <param name="listPositionsId">List of positions required recprds in records list.</param>
        public RecordMemoryEnumerator(List<FileCabinetRecord> records, List<int> listPositionsId)
        {
            this.records = records;
            this.listPositionsId = listPositionsId;
        }

        /// <summary>
        /// Gets current record.
        /// </summary>
        /// <value>
        /// Holds current record.
        /// </value>
        public FileCabinetRecord Current
        {
            get
            {
                if (this.position == -1 || this.position >= this.listPositionsId.Count)
                {
                    throw new InvalidOperationException();
                }

                int positionInList = this.listPositionsId[this.position];

                return this.records[positionInList];
            }
        }

        /// <inheritdoc/>
        object IEnumerator.Current => throw new NotImplementedException();

        /// <summary>
        /// Disposes class.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Sets the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>Possibility to move far.</returns>
        public bool MoveNext()
        {
            if (this.position < this.listPositionsId.Count - 1)
            {
                this.position++;

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the enumerator to its initial position.
        /// </summary>
        public void Reset()
        {
            this.position = -1;
        }
    }
}
