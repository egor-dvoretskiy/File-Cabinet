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
    /// Iterator for <see cref="FileCabinetFileSystemService"/> class.
    /// </summary>
    public class RecordFilesystemEnumerable : IEnumerable<FileCabinetRecord>
    {
        private FileStream fileStream;
        private List<int> listPositionsId = new List<int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordFilesystemEnumerable"/> class.
        /// </summary>
        /// <param name="fileStream">Using for reading data from file.</param>
        /// <param name="listPositionsId">List of record's positions in file.</param>
        public RecordFilesystemEnumerable(FileStream fileStream, List<int> listPositionsId)
        {
            this.fileStream = fileStream;
            this.listPositionsId = listPositionsId;
        }

        /// <inheritdoc/>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            return new RecordFilesystemEnumerator(this.fileStream, this.listPositionsId);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
