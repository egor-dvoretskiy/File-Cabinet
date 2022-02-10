using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Database enumerator for records.
    /// </summary>
    internal class RecordDatabaseEnumerator : IEnumerator<FileCabinetRecord>
    {
        private int position = -1;
        private List<FileCabinetRecord> records;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordDatabaseEnumerator"/> class.
        /// </summary>
        /// <param name="records">Stored records in database.</param>
        internal RecordDatabaseEnumerator(List<FileCabinetRecord> records)
        {
            this.records = records;
        }

        /// <inheritdoc/>
        public FileCabinetRecord Current
        {
            get
            {
                if (this.position == -1 || this.position >= this.records.Count)
                {
                    throw new InvalidOperationException();
                }

                return this.records[this.position];
            }
        }

        /// <inheritdoc/>
        object IEnumerator.Current => throw new NotImplementedException();

        /// <inheritdoc/>
        public void Dispose()
        {
        }

        /// <inheritdoc/>
        public bool MoveNext()
        {
            if (this.position < this.records.Count - 1)
            {
                this.position++;

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public void Reset()
        {
            this.position = -1;
        }
    }
}
