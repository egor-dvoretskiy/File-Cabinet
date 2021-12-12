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
    public class FileCabinetFileSystemService : IFileCabinetService
    {
        private const byte MaxNameLength = 120;
        private const int RecordSize = sizeof(short) + sizeof(int) + (MaxNameLength * 2) + (sizeof(int) * 3) + sizeof(short) + sizeof(decimal) + sizeof(char);

        private FileStream fileStream;

        private Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        private int recordsCount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFileSystemService"/> class.
        /// </summary>
        /// <param name="fileStream">File stream.</param>
        public FileCabinetFileSystemService(FileStream fileStream)
        {
            fileStream.Seek(0, SeekOrigin.End);
            this.fileStream = fileStream;

            this.recordsCount = (int)(fileStream.Length / RecordSize);
        }

        /// <inheritdoc/>
        public int CreateRecord(FileCabinetRecord record)
        {
            try
            {
                this.fileStream.Seek(0, SeekOrigin.End);

                byte[] buffer = new byte[RecordSize];
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                using (BinaryWriter writer = new BinaryWriter(memoryStream))
                {
                    writer.Seek(sizeof(short), SeekOrigin.Begin); // status reservation.

                    record.Id = ++this.recordsCount;
                    writer.Write(record.Id);

                    var firstNameBytes = this.PrepareStringToWrite(record.FirstName);
                    writer.Write(firstNameBytes);
                    memoryStream.Seek(MaxNameLength - firstNameBytes.Length, SeekOrigin.Current);

                    var lastNameBytes = this.PrepareStringToWrite(record.LastName);
                    writer.Write(lastNameBytes);
                    memoryStream.Seek(MaxNameLength - lastNameBytes.Length, SeekOrigin.Current);

                    writer.Write(record.DateOfBirth.Year);
                    writer.Write(record.DateOfBirth.Month);
                    writer.Write(record.DateOfBirth.Day);
                    writer.Write(record.PersonalRating);
                    writer.Write(record.Debt);
                    writer.Write(record.Gender);
                }

                this.fileStream.Write(buffer, 0, buffer.Length);

                this.fileStream.Flush(true);

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
            this.fileStream.Seek(0, SeekOrigin.Begin);

            List<FileCabinetRecord> records = new List<FileCabinetRecord>();

            for (int i = 0; i < this.recordsCount; i++)
            {
                byte[] bytes = new byte[RecordSize];
                this.fileStream.Read(bytes, 0, RecordSize);

                FileCabinetRecord record = new FileCabinetRecord();

                using (MemoryStream memoryStream = new MemoryStream(bytes))
                using (BinaryReader reader = new BinaryReader(memoryStream))
                {
                    _ = reader.ReadInt16();

                    record.Id = reader.ReadInt32();

                    byte[] firstNameBytes = reader.ReadBytes(MaxNameLength);
                    string firstName = Encoding.ASCII.GetString(firstNameBytes, 0, MaxNameLength);
                    record.FirstName = firstName;

                    byte[] lastNameBytes = reader.ReadBytes(MaxNameLength);
                    string lastName = Encoding.ASCII.GetString(lastNameBytes, 0, MaxNameLength);
                    record.LastName = lastName;

                    int year = reader.ReadInt32();
                    int month = reader.ReadInt32();
                    int day = reader.ReadInt32();
                    record.DateOfBirth = new DateTime(year, month, day);

                    record.PersonalRating = reader.ReadInt16();
                    record.Debt = reader.ReadDecimal();
                    record.Gender = reader.ReadChar();

                    records.Add(record);
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            return this.recordsCount;
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }

        private byte[] PrepareStringToWrite(string stringToWrite)
        {
            var nameBytes = Encoding.ASCII.GetBytes(stringToWrite);
            var nameBuffer = new byte[MaxNameLength];
            int nameLength = nameBytes.Length;
            if (nameLength > MaxNameLength)
            {
                nameLength = MaxNameLength;
            }

            Array.Copy(nameBytes, 0, nameBuffer, 0, nameLength);

            return nameBytes;
        }
    }
}
