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

        // first argument - record's id, second element - record's position in file.
        private Dictionary<int, int> dictRecordsPositionOrder = new Dictionary<int, int>();

        // first argument - record's param to search, second element - record's position in file.
        private Dictionary<string, List<int>> firstNameDictionary = new Dictionary<string, List<int>>();
        private Dictionary<string, List<int>> lastNameDictionary = new Dictionary<string, List<int>>();
        private Dictionary<DateTime, List<int>> dateOfBirthDictionary = new Dictionary<DateTime, List<int>>();

        private int recordsCount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFileSystemService"/> class.
        /// </summary>
        /// <param name="fileStream">File stream.</param>
        public FileCabinetFileSystemService(FileStream fileStream)
        {
            this.fileStream = fileStream;

            this.recordsCount = (int)(fileStream.Length / RecordSize);

            this.AssignRecordValuesToDictionaries();
        }

        /// <inheritdoc/>
        public int CreateRecord(FileCabinetRecord record)
        {
            try
            {
                this.fileStream.Seek(0, SeekOrigin.End);

                record.Id = ++this.recordsCount;

                if (!this.dictRecordsPositionOrder.ContainsKey(record.Id))
                {
                    this.dictRecordsPositionOrder.Add(record.Id, this.recordsCount - 1);
                }

                byte[] buffer = this.WriteRecordToFile(record);

                this.fileStream.Write(buffer, 0, buffer.Length);

                this.fileStream.Flush(true);

                this.AddIdToParamsDictionary(record.Id, this.recordsCount - 1, ref this.dictRecordsPositionOrder);
                this.AddNameToParamsDictionary(record.FirstName, this.recordsCount - 1, ref this.firstNameDictionary);
                this.AddNameToParamsDictionary(record.LastName, this.recordsCount - 1, ref this.lastNameDictionary);
                this.AddDateTimeToParamsDictionary(record.DateOfBirth, this.recordsCount - 1, ref this.dateOfBirthDictionary);

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
        public void EditRecord(int recordPosition, FileCabinetRecord record)
        {
            try
            {
                byte[] buffer = this.WriteRecordToFile(record);
                this.fileStream.Seek(recordPosition * RecordSize, SeekOrigin.Begin);

                this.fileStream.Write(buffer, 0, buffer.Length);
                this.fileStream.Flush(true);

                this.ClearDictionaries();
                this.AssignRecordValuesToDictionaries();
            }
            catch (ArgumentNullException anex)
            {
                Console.WriteLine(anex.Message);
            }
            catch (ArgumentException aex)
            {
                Console.WriteLine(aex.Message);
            }
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByBirthDate(string birthDate)
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            List<int> datePositions = this.GetDateTimeFromDictionary(birthDate, this.dateOfBirthDictionary);

            for (int i = 0; i < datePositions.Count; i++)
            {
                this.fileStream.Seek(datePositions[i] * RecordSize, SeekOrigin.Begin);
                byte[] bytes = new byte[RecordSize];
                this.fileStream.Read(bytes, 0, RecordSize);

                FileCabinetRecord record = this.ReadRecordFromFile(bytes);

                records.Add(record);
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            List<int> firstNamePositions = this.GetNameFromDictionary(firstName, this.firstNameDictionary);

            for (int i = 0; i < firstNamePositions.Count; i++)
            {
                this.fileStream.Seek(firstNamePositions[i] * RecordSize, SeekOrigin.Begin);
                byte[] bytes = new byte[RecordSize];
                this.fileStream.Read(bytes, 0, RecordSize);

                FileCabinetRecord record = this.ReadRecordFromFile(bytes);

                records.Add(record);
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            List<int> firstNamePositions = this.GetNameFromDictionary(lastName, this.lastNameDictionary);

            for (int i = 0; i < firstNamePositions.Count; i++)
            {
                this.fileStream.Seek(firstNamePositions[i] * RecordSize, SeekOrigin.Begin);
                byte[] bytes = new byte[RecordSize];
                this.fileStream.Read(bytes, 0, RecordSize);

                FileCabinetRecord record = this.ReadRecordFromFile(bytes);

                records.Add(record);
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public int GetRecordPosition(int id)
        {
            if (!this.dictRecordsPositionOrder.ContainsKey(id))
            {
                throw new ArgumentException($"#{id} record is not found.");
            }

            return this.dictRecordsPositionOrder[id];
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();

            try
            {
                this.fileStream.Seek(0, SeekOrigin.Begin);

                for (int i = 0; i < this.recordsCount; i++)
                {
                    byte[] bytes = new byte[RecordSize];
                    this.fileStream.Read(bytes, 0, RecordSize);

                    FileCabinetRecord record = this.ReadRecordFromFile(bytes);

                    records.Add(record);
                }
            }
            catch (ArgumentOutOfRangeException argumentOutOfRangeException)
            {
                Console.WriteLine(argumentOutOfRangeException.Message);
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

        private void ClearDictionaries()
        {
            this.dictRecordsPositionOrder.Clear();
            this.firstNameDictionary.Clear();
            this.lastNameDictionary.Clear();
            this.dateOfBirthDictionary.Clear();
        }

        private void AssignRecordValuesToDictionaries()
        {
            this.fileStream.Seek(0, SeekOrigin.Begin);

            for (int i = 0; i < this.recordsCount; i++)
            {
                byte[] bytes = new byte[RecordSize];
                this.fileStream.Read(bytes, 0, RecordSize);

                FileCabinetRecord record = this.ReadRecordFromFile(bytes);

                this.AddIdToParamsDictionary(record.Id, i, ref this.dictRecordsPositionOrder);
                this.AddNameToParamsDictionary(record.FirstName, i, ref this.firstNameDictionary);
                this.AddNameToParamsDictionary(record.LastName, i, ref this.lastNameDictionary);
                this.AddDateTimeToParamsDictionary(record.DateOfBirth, i, ref this.dateOfBirthDictionary);
            }
        }

        private void AddIdToParamsDictionary(int id, int position, ref Dictionary<int, int> valuePairs)
        {
            if (!valuePairs.ContainsKey(id))
            {
                valuePairs.Add(id, position);
            }
        }

        private void AddNameToParamsDictionary(string param, int value, ref Dictionary<string, List<int>> valuePairs)
        {
            if (!valuePairs.ContainsKey(param))
            {
                valuePairs.Add(param, new List<int>() { value });
            }
            else if (valuePairs.ContainsKey(param) && !valuePairs[param].Contains(value))
            {
                valuePairs[param].Add(value);
            }
        }

        private void AddDateTimeToParamsDictionary(DateTime param, int value, ref Dictionary<DateTime, List<int>> valuePairs)
        {
            if (!valuePairs.ContainsKey(param))
            {
                valuePairs.Add(param, new List<int>() { value });
            }
            else if (valuePairs.ContainsKey(param) && !valuePairs[param].Contains(value))
            {
                valuePairs[param].Add(value);
            }
        }

        private FileCabinetRecord ReadRecordFromFile(byte[] bytesRecord)
        {
            FileCabinetRecord record = new FileCabinetRecord();

            using (MemoryStream memoryStream = new MemoryStream(bytesRecord))
            using (BinaryReader reader = new BinaryReader(memoryStream))
            {
                _ = reader.ReadInt16();

                record.Id = reader.ReadInt32();

                byte[] firstNameBytes = reader.ReadBytes(MaxNameLength);
                string firstName = Encoding.ASCII.GetString(firstNameBytes, 0, MaxNameLength).TrimEnd('\0');
                record.FirstName = firstName;

                byte[] lastNameBytes = reader.ReadBytes(MaxNameLength);
                string lastName = Encoding.ASCII.GetString(lastNameBytes, 0, MaxNameLength).TrimEnd('\0');
                record.LastName = lastName;

                int year = reader.ReadInt32();
                int month = reader.ReadInt32();
                int day = reader.ReadInt32();
                record.DateOfBirth = new DateTime(year, month, day);

                record.PersonalRating = reader.ReadInt16();
                record.Debt = reader.ReadDecimal();
                record.Gender = reader.ReadChar();
            }

            return record;
        }

        private byte[] WriteRecordToFile(FileCabinetRecord record)
        {
            byte[] buffer = new byte[RecordSize];
            using (MemoryStream memoryStream = new MemoryStream(buffer))
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                writer.Seek(sizeof(short), SeekOrigin.Begin); // status reservation.

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

            return buffer;
        }

        private List<int> GetNameFromDictionary(string parametrName, Dictionary<string, List<int>> dictionary)
        {
            if (dictionary.ContainsKey(parametrName))
            {
                return dictionary[parametrName];
            }

            return new List<int>();
        }

        private List<int> GetDateTimeFromDictionary(string parametrName, Dictionary<DateTime, List<int>> dictionary)
        {
            bool isDateValid = DateTime.TryParse(parametrName, out DateTime birthDate);
            if (isDateValid && dictionary.ContainsKey(birthDate))
            {
                return dictionary[birthDate];
            }

            return new List<int>();
        }
    }
}
