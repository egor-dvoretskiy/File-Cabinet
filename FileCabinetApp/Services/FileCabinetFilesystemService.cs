using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Iterators;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// File system service.
    /// </summary>
    public class FileCabinetFileSystemService : IFileCabinetService
    {
        /// <summary>
        /// According to maximum length of name: 60 * sizeof(char).
        /// </summary>
        public const byte MaxNameLength = 120;

        /// <summary>
        /// Summary size of single record in file.
        /// </summary>
        public const int RecordSize = sizeof(short) + sizeof(int) + (MaxNameLength * 2) + (sizeof(int) * 3) + sizeof(short) + sizeof(decimal) + sizeof(char);

        private FileStream fileStream;
        private IRecordValidator recordValidator;

        // first argument - record's id, second element - record's position in file.
        private Dictionary<int, int> dictRecordsPositionOrder = new Dictionary<int, int>();

        // first argument - record's param to search, second element - list of id's.
        private Dictionary<string, List<int>> firstNameDictionary = new Dictionary<string, List<int>>();
        private Dictionary<string, List<int>> lastNameDictionary = new Dictionary<string, List<int>>();
        private Dictionary<DateTime, List<int>> dateOfBirthDictionary = new Dictionary<DateTime, List<int>>();

        private int recordsCount = 0;
        private int recordMarkedAsDeletedCount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFileSystemService"/> class.
        /// </summary>
        /// <param name="fileStream">File stream.</param>
        /// <param name="recordValidator">Validator for record.</param>
        public FileCabinetFileSystemService(FileStream fileStream, IRecordValidator recordValidator)
        {
            this.fileStream = fileStream;
            this.recordValidator = recordValidator;

            this.recordsCount = (int)(fileStream.Length / RecordSize);

            this.AssignRecordValuesToDictionaries();
        }

        /// <summary>
        /// Reads fixed size buffer.
        /// </summary>
        /// <param name="bytesRecord">Acquired from file buffer.</param>
        /// <returns>Returns read record and isDeleted flag.</returns>
        public static Tuple<FileCabinetRecord, short> ReadRecordFromBuffer(byte[] bytesRecord)
        {
            short isRecordDeleted = 0;
            FileCabinetRecord record = new FileCabinetRecord();

            using (MemoryStream memoryStream = new MemoryStream(bytesRecord))
            using (BinaryReader reader = new BinaryReader(memoryStream))
            {
                isRecordDeleted = reader.ReadInt16();

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
                record.Salary = reader.ReadDecimal();
                record.Gender = reader.ReadChar();
            }

            return new Tuple<FileCabinetRecord, short>(record, isRecordDeleted);
        }

        /// <inheritdoc/>
        public int CreateRecord(FileCabinetRecord record)
        {
            try
            {
                bool isValid = this.recordValidator.ValidateParameters(record);

                if (!isValid)
                {
                    throw new ArgumentException("Record you want to create is not valid. Please try again!");
                }

                this.fileStream.Seek(0, SeekOrigin.End);

                record.Id = this.GetUniqueId();

                byte[] buffer = this.WriteRecordToBuffer(record);

                this.fileStream.Write(buffer, 0, buffer.Length);
                this.fileStream.Flush(true);

                this.AddIdToParamsDictionary(record.Id, this.recordsCount, ref this.dictRecordsPositionOrder);
                this.AddNameToParamsDictionary(record.FirstName, record.Id, ref this.firstNameDictionary);
                this.AddNameToParamsDictionary(record.LastName, record.Id, ref this.lastNameDictionary);
                this.AddDateTimeToParamsDictionary(record.DateOfBirth, record.Id, ref this.dateOfBirthDictionary);

                this.UpdateRecordCount();

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
            try
            {
                bool isValid = this.recordValidator.ValidateParameters(record);

                if (!isValid)
                {
                    throw new ArgumentException("Record you want to edit is not valid. Please try again!");
                }

                int recordPosition = this.dictRecordsPositionOrder[id];

                byte[] buffer = this.WriteRecordToBuffer(record);
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
        public IEnumerable<FileCabinetRecord> FindByBirthDate(string birthDate)
        {
            List<int> listIdRecordsPositions = this.GetDateTimeFromDictionary(birthDate, this.dateOfBirthDictionary);

            return new RecordFilesystemEnumerable(this.fileStream, listIdRecordsPositions);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            List<int> listIdRecordsPositions = this.GetNameFromDictionary(firstName, this.firstNameDictionary);

            return new RecordFilesystemEnumerable(this.fileStream, listIdRecordsPositions);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            List<int> listIdRecordsPositions = this.GetNameFromDictionary(lastName, this.lastNameDictionary);

            return new RecordFilesystemEnumerable(this.fileStream, listIdRecordsPositions);
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetRecord(int id)
        {
            this.fileStream.Seek(this.dictRecordsPositionOrder[id] * RecordSize, SeekOrigin.Begin);

            byte[] bytes = new byte[RecordSize];
            this.fileStream.Read(bytes, 0, RecordSize);

            var tupleReadFromFile = ReadRecordFromBuffer(bytes);

            FileCabinetRecord record = tupleReadFromFile.Item1;

            return record;
        }

        /// <inheritdoc/>
        public bool CheckRecordPresence(int id)
        {
            bool listIdPresent = true;

            if (!this.dictRecordsPositionOrder.ContainsKey(id))
            {
                listIdPresent = false;
            }

            return listIdPresent;
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

                    var tupleReadFromFile = ReadRecordFromBuffer(bytes);

                    if (tupleReadFromFile.Item2 == 1)
                    {
                        continue;
                    }

                    FileCabinetRecord record = tupleReadFromFile.Item1;

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
        public void GetStat()
        {
            Console.WriteLine($"{this.recordsCount} record(s). {this.recordMarkedAsDeletedCount} record(s) marked as deleted.");
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.GetRecords().ToList(), this.recordValidator);
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot)
        {
            var unloadRecords = fileCabinetServiceSnapshot.Records.ToList();

            for (int i = 0; i < unloadRecords.Count; i++)
            {
                var record = unloadRecords[i];

                byte[] buffer = this.WriteRecordToBuffer(record);

                if (this.dictRecordsPositionOrder.ContainsKey(record.Id))
                {
                    int positionOfExistingId = this.dictRecordsPositionOrder[record.Id] * RecordSize;

                    this.fileStream.Seek(positionOfExistingId, SeekOrigin.Begin);
                }
                else
                {
                    this.dictRecordsPositionOrder.Add(record.Id, this.recordsCount);
                    this.AddNameToParamsDictionary(record.FirstName, record.Id, ref this.firstNameDictionary);
                    this.AddNameToParamsDictionary(record.LastName, record.Id, ref this.lastNameDictionary);
                    this.AddDateTimeToParamsDictionary(record.DateOfBirth, record.Id, ref this.dateOfBirthDictionary);

                    this.fileStream.Seek(0, SeekOrigin.End);
                }

                this.fileStream.Write(buffer, 0, buffer.Length);
                this.fileStream.Flush(true);

                this.UpdateRecordCount();
            }
        }

        /// <inheritdoc/>
        public void RemoveRecordById(int id)
        {
            if (!this.dictRecordsPositionOrder.ContainsKey(id))
            {
                Console.WriteLine($"There is no record #{id}.");
                return;
            }

            try
            {
                short deleteByte = 1;

                byte[] buffer = new byte[sizeof(short)];
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                using (BinaryWriter writer = new BinaryWriter(memoryStream))
                {
                    writer.Write(deleteByte);
                }

                int recordPosition = this.dictRecordsPositionOrder[id];

                this.fileStream.Seek(recordPosition * RecordSize, SeekOrigin.Begin);

                this.fileStream.Write(buffer, 0, buffer.Length);
                this.fileStream.Flush(true);

                this.UpdateRecordCount();
                this.ClearDictionaries();
                this.AssignRecordValuesToDictionaries();

                Console.WriteLine($"Record #{id} is deleted.");
            }
            catch (ArgumentNullException anex)
            {
                Console.WriteLine(anex.Message);
            }
            catch (ArgumentException aex)
            {
                Console.WriteLine(aex.Message);
            }
            catch (KeyNotFoundException keyNotFoundException)
            {
                Console.WriteLine(keyNotFoundException.Message);
            }
        }

        /// <inheritdoc/>
        public void Delete(List<int> ids)
        {
            for (int i = 0; i < ids.Count; i++)
            {
                this.RemoveRecordById(ids[i]);
            }
        }

        /// <inheritdoc/>
        public void Purge()
        {
            try
            {
                int currentPosition = 0;
                int previousDeletedCount = 0;

                for (int i = 0; i < this.recordsCount; i++)
                {
                    this.fileStream.Seek(i * RecordSize, SeekOrigin.Begin);

                    byte[] bytes = new byte[RecordSize];
                    this.fileStream.Read(bytes, 0, RecordSize);

                    var tupleReadFromFile = ReadRecordFromBuffer(bytes);

                    if (tupleReadFromFile.Item2 == 1)
                    {
                        previousDeletedCount++;
                    }
                    else if ((tupleReadFromFile.Item2 == 0 && previousDeletedCount > 0) || currentPosition != i)
                    {
                        byte[] buffer = this.WriteRecordToBuffer(tupleReadFromFile.Item1);

                        this.fileStream.Seek(currentPosition * RecordSize, SeekOrigin.Begin);

                        this.fileStream.Write(buffer, 0, buffer.Length);

                        previousDeletedCount--;
                        currentPosition++;
                    }
                    else
                    {
                        currentPosition++;
                    }
                }

                this.fileStream.Flush(true);
                this.fileStream.SetLength(currentPosition * RecordSize);

                Console.WriteLine($"Data file processing is completed: {this.recordMarkedAsDeletedCount} of {this.recordsCount} records were purged.");

                this.UpdateRecordCount();
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
        public void InsertRecord(FileCabinetRecord record)
        {
            try
            {
                bool isValid = this.recordValidator.ValidateParameters(record);

                if (!isValid)
                {
                    throw new ArgumentException("Record you want to add is not valid. Please try again!");
                }

                if (this.dictRecordsPositionOrder.ContainsKey(record.Id))
                {
                    throw new ArgumentException($"Memory is already has a record #{record.Id}.");
                }

                this.fileStream.Seek(0, SeekOrigin.End);

                byte[] buffer = this.WriteRecordToBuffer(record);

                this.fileStream.Write(buffer, 0, buffer.Length);
                this.fileStream.Flush(true);

                this.AddIdToParamsDictionary(record.Id, this.recordsCount, ref this.dictRecordsPositionOrder);
                this.AddNameToParamsDictionary(record.FirstName, record.Id, ref this.firstNameDictionary);
                this.AddNameToParamsDictionary(record.LastName, record.Id, ref this.lastNameDictionary);
                this.AddDateTimeToParamsDictionary(record.DateOfBirth, record.Id, ref this.dateOfBirthDictionary);

                this.UpdateRecordCount();
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
        public void Update(List<FileCabinetRecord> records)
        {
            for (int i = 0; i < records.Count; i++)
            {
                this.EditRecord(records[i].Id, records[i]);
            }

            Console.WriteLine($"Record's updating completed.");
        }

        private int GetUniqueId()
        {
            int id = 1;

            while (this.dictRecordsPositionOrder.ContainsKey(id))
            {
                id++;
            }

            return id;
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
            this.recordMarkedAsDeletedCount = 0;
            this.fileStream.Seek(0, SeekOrigin.Begin);

            for (int i = 0; i < this.recordsCount; i++)
            {
                byte[] bytes = new byte[RecordSize];
                this.fileStream.Read(bytes, 0, RecordSize);

                var tupleReadFromFile = ReadRecordFromBuffer(bytes);

                if (tupleReadFromFile.Item2 == 1)
                {
                    this.recordMarkedAsDeletedCount++;
                    continue;
                }

                FileCabinetRecord record = tupleReadFromFile.Item1;

                this.AddIdToParamsDictionary(record.Id, i, ref this.dictRecordsPositionOrder);
                this.AddNameToParamsDictionary(record.FirstName, record.Id, ref this.firstNameDictionary);
                this.AddNameToParamsDictionary(record.LastName, record.Id, ref this.lastNameDictionary);
                this.AddDateTimeToParamsDictionary(record.DateOfBirth, record.Id, ref this.dateOfBirthDictionary);
            }
        }

        private void AddIdToParamsDictionary(int id, int position, ref Dictionary<int, int> valuePairs)
        {
            if (!valuePairs.ContainsKey(id))
            {
                valuePairs.Add(id, position);
            }
        }

        private void AddNameToParamsDictionary(string param, int id, ref Dictionary<string, List<int>> valuePairs)
        {
            if (!valuePairs.ContainsKey(param))
            {
                valuePairs.Add(param, new List<int>() { id });
            }
            else if (valuePairs.ContainsKey(param) && !valuePairs[param].Contains(id))
            {
                valuePairs[param].Add(id);
            }
        }

        private void AddDateTimeToParamsDictionary(DateTime param, int id, ref Dictionary<DateTime, List<int>> valuePairs)
        {
            if (!valuePairs.ContainsKey(param))
            {
                valuePairs.Add(param, new List<int>() { id });
            }
            else if (valuePairs.ContainsKey(param) && !valuePairs[param].Contains(id))
            {
                valuePairs[param].Add(id);
            }
        }

        private byte[] WriteRecordToBuffer(FileCabinetRecord record)
        {
            byte[] buffer = new byte[RecordSize];
            using (MemoryStream memoryStream = new MemoryStream(buffer))
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                short isRecordDeleted = 0;
                writer.Write(isRecordDeleted);

                writer.Write(record.Id);

                var firstNameBytes = this.PrepareStringToWrite(record.FirstName);
                writer.Write(firstNameBytes);
                memoryStream.Seek(MaxNameLength - firstNameBytes.Length, SeekOrigin.Current);

                var lastNameBytes = this.PrepareStringToWrite(record.LastName);
                writer.Write(lastNameBytes);
                memoryStream.Seek(MaxNameLength - lastNameBytes.Length, SeekOrigin.Current);

                int year = record.DateOfBirth.Year;
                writer.Write(year);

                int month = record.DateOfBirth.Month;
                writer.Write(month);

                int day = record.DateOfBirth.Day;
                writer.Write(day);

                writer.Write(record.PersonalRating);
                writer.Write(record.Salary);
                writer.Write(record.Gender);
            }

            return buffer;
        }

        private List<int> GetNameFromDictionary(string parametrName, Dictionary<string, List<int>> dictionary)
        {
            if (dictionary.ContainsKey(parametrName))
            {
                // List of id's, but need positions.
                List<int> listOfPositions = new List<int>();

                for (int i = 0; i < dictionary[parametrName].Count; i++)
                {
                    var position = this.dictRecordsPositionOrder[dictionary[parametrName][i]] * RecordSize;
                    listOfPositions.Add(position);
                }

                return listOfPositions;
            }

            return new List<int>();
        }

        private List<int> GetDateTimeFromDictionary(string parametrName, Dictionary<DateTime, List<int>> dictionary)
        {
            bool isDateValid = DateTime.TryParse(parametrName, out DateTime birthDate);
            if (isDateValid && dictionary.ContainsKey(birthDate))
            {
                // List of id's, but need positions.
                List<int> listOfPositions = new List<int>();

                for (int i = 0; i < dictionary[birthDate].Count; i++)
                {
                    var position = this.dictRecordsPositionOrder[dictionary[birthDate][i]] * RecordSize;
                    listOfPositions.Add(position);
                }

                return listOfPositions;
            }

            return new List<int>();
        }

        private void UpdateRecordCount()
        {
            this.recordsCount = (int)this.fileStream.Length / RecordSize;
        }
    }
}
