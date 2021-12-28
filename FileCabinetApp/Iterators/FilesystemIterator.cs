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
    /// Iterator for <see cref="FileCabinetFileSystemService"/> class.
    /// </summary>
    public class FilesystemIterator : IRecordIterator
    {
        private int position = 0;
        private FileStream fileStream;
        private List<int> listPositionsId = new List<int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemIterator"/> class.
        /// </summary>
        /// <param name="fileStream">Using for reading data from file.</param>
        /// <param name="listPositionsId">List of record's positions in file.</param>
        public FilesystemIterator(FileStream fileStream, List<int> listPositionsId)
        {
            this.fileStream = fileStream;
            this.listPositionsId = listPositionsId;
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetNext()
        {
            this.fileStream.Seek(this.listPositionsId[this.position++], SeekOrigin.Begin);
            byte[] bytes = new byte[FileCabinetFileSystemService.RecordSize];
            this.fileStream.Read(bytes, 0, FileCabinetFileSystemService.RecordSize);

            var tupleReadFromFile = FileCabinetFileSystemService.ReadRecordFromBuffer(bytes);

            FileCabinetRecord record = tupleReadFromFile.Item1;

            return record;
        }

        /// <inheritdoc/>
        public bool HasMore()
        {
            bool hasMore = this.position < this.listPositionsId.Count;
            return hasMore;
        }
    }
}
