using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Services;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Filesystem enumerator for records.
    /// </summary>
    internal class RecordFilesystemEnumerator : IEnumerator<FileCabinetRecord>
    {
        private int position = -1;
        private FileStream fileStream;
        private List<int> listPositionsId = new List<int>();
        private FileCabinetRecord record;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordFilesystemEnumerator"/> class.
        /// </summary>
        /// <param name="fileStream">File stream.</param>
        /// <param name="listPositionsId">List of positions required recprds in records list.</param>
        public RecordFilesystemEnumerator(FileStream fileStream, List<int> listPositionsId)
        {
            this.fileStream = fileStream;
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
                if (this.position == -1 || this.position >= this.listPositionsId.Count || this.record is null)
                {
                    throw new InvalidOperationException();
                }

                return this.record;
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

                this.fileStream.Seek(this.listPositionsId[this.position], SeekOrigin.Begin);
                byte[] bytes = new byte[FileCabinetFileSystemService.RecordSize];
                this.fileStream.Read(bytes, 0, FileCabinetFileSystemService.RecordSize);

                var tupleReadFromFile = FileCabinetFileSystemService.ReadRecordFromBuffer(bytes);

                this.record = tupleReadFromFile.Item1;

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
