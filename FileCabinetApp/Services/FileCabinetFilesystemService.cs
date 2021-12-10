using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// File system service.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int RecordSize = 277; // bytes

        private FileStream fileStream;

        private Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        private int recordsCount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">File stream.</param>
        public FileCabinetFilesystemService(FileStream fileStream)
        {
            this.fileStream = fileStream;
        }

        /// <inheritdoc/>
        public int CreateRecord(FileCabinetRecord record)
        {
            try
            {
                record.Id = ++this.recordsCount;

                using (BinaryWriter writer = new BinaryWriter(this.fileStream))
                {
                    writer.Seek(2, SeekOrigin.Begin);
                    writer.Write(record.Id);

                    writer.Write(record.FirstName.ToCharArray());
                    writer.Seek(120 - record.FirstName.Length, SeekOrigin.Current);

                    writer.Write(record.LastName.ToCharArray());
                    writer.Seek(120 - record.LastName.Length, SeekOrigin.Current);

                    writer.Write(record.DateOfBirth.Year);
                    writer.Write(record.DateOfBirth.Month);
                    writer.Write(record.DateOfBirth.Day);
                    writer.Write(record.PersonalRating);
                    writer.Write(record.Debt);
                    writer.Write(record.Gender);
                }

                return record.Id;
            }
            catch (ArgumentNullException anex)
            {
                Console.WriteLine(anex.Message);
            }
            catch (ArgumentException aex)
            {
                Console.WriteLine(aex.Message);
            }

            return -1;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, FileCabinetRecord record)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByBirthDate(string birthDate)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int GetPositionInListRecordsById(int id)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }
    }
}
