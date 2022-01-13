using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileCabinetApp.Interfaces;
using FileCabinetApp.Iterators;
using FileCabinetApp.ServiceTools;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// File system service.
    /// </summary>
    public class FileCabinetFileSystemService : FileCabinetDictionary, IFileCabinetService
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
        public void CreateRecord(FileCabinetRecord record)
        {
            try
            {
                bool isValid = this.recordValidator.ValidateParameters(record);

                if (!isValid)
                {
                    Console.WriteLine($"Record validation failed.");
                    return;
                }

                this.fileStream.Seek(0, SeekOrigin.End);

                record.Id = this.GetUniqueId();

                byte[] buffer = this.WriteRecordToBuffer(record);

                this.fileStream.Write(buffer, 0, buffer.Length);
                this.fileStream.Flush(true);

                this.AddRecordToDictionaries(record, this.recordsCount);

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
        public IEnumerable<FileCabinetRecord> FindByBirthDate(string birthDate)
        {
            List<int> listIdRecordsPositions = this.GetListOfDateTimeFromDictionary(birthDate, this.dateOfBirthDictionary);

            return new RecordFilesystemEnumerable(this.fileStream, listIdRecordsPositions);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            List<int> listIdRecordsPositions = this.GetListOfNameFromDictionary(firstName, this.firstNameDictionary);

            return new RecordFilesystemEnumerable(this.fileStream, listIdRecordsPositions);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            List<int> listIdRecordsPositions = this.GetListOfNameFromDictionary(lastName, this.lastNameDictionary);

            return new RecordFilesystemEnumerable(this.fileStream, listIdRecordsPositions);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByPersonalRating(string personalRating)
        {
            List<int> listIdRecordsPositions = this.GetListOfPersonalRatingFromDictionary(personalRating, this.personalRatingDictionary);

            return new RecordFilesystemEnumerable(this.fileStream, listIdRecordsPositions);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindBySalary(string salary)
        {
            List<int> listIdRecordsPositions = this.GetListOfSalaryFromDictionary(salary, this.salaryDictionary);

            return new RecordFilesystemEnumerable(this.fileStream, listIdRecordsPositions);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByGender(string gender)
        {
            List<int> listIdRecordsPositions = this.GetListOfGenderFromDictionary(gender, this.genderDictionary);

            return new RecordFilesystemEnumerable(this.fileStream, listIdRecordsPositions);
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetRecord(int id)
        {
            this.fileStream.Seek(this.storedIdRecords[id] * RecordSize, SeekOrigin.Begin);

            byte[] bytes = new byte[RecordSize];
            this.fileStream.Read(bytes, 0, RecordSize);

            var tupleReadFromFile = ReadRecordFromBuffer(bytes);

            FileCabinetRecord record = tupleReadFromFile.Item1;

            return record;
        }

        /// <inheritdoc/>
        public bool CheckRecordPresence(int id) => this.storedIdRecords.ContainsKey(id);

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

                if (this.storedIdRecords.ContainsKey(record.Id))
                {
                    int positionOfExistingId = this.storedIdRecords[record.Id] * RecordSize;

                    this.fileStream.Seek(positionOfExistingId, SeekOrigin.Begin);
                }
                else
                {
                    this.AddRecordToDictionaries(record, this.recordsCount);

                    this.fileStream.Seek(0, SeekOrigin.End);
                }

                this.fileStream.Write(buffer, 0, buffer.Length);
                this.fileStream.Flush(true);

                this.UpdateRecordCount();
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

                if (this.storedIdRecords.ContainsKey(record.Id))
                {
                    throw new ArgumentException($"Memory is already has a record #{record.Id}.");
                }

                this.fileStream.Seek(0, SeekOrigin.End);

                byte[] buffer = this.WriteRecordToBuffer(record);

                this.fileStream.Write(buffer, 0, buffer.Length);
                this.fileStream.Flush(true);

                this.AddRecordToDictionaries(record, this.recordsCount);
                this.UpdateRecordCount();
            }
            catch (ArgumentNullException argumentNullException)
            {
                Console.WriteLine(argumentNullException.Message);
            }
            catch (ArgumentException argumentException)
            {
                Console.WriteLine(argumentException.Message);
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

        private void EditRecord(int id, FileCabinetRecord record)
        {
            try
            {
                bool isValid = this.recordValidator.ValidateParameters(record);

                if (!isValid)
                {
                    throw new ArgumentException("Record you want to edit is not valid. Please try again!");
                }

                int recordPosition = this.storedIdRecords[id];

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

        private void RemoveRecordById(int id)
        {
            if (!this.storedIdRecords.ContainsKey(id))
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

                int recordPosition = this.storedIdRecords[id];

                this.fileStream.Seek(recordPosition * RecordSize, SeekOrigin.Begin);

                this.fileStream.Write(buffer, 0, buffer.Length);
                this.fileStream.Flush(true);

                this.UpdateRecordCount();
                this.ClearDictionaries();
                this.AssignRecordValuesToDictionaries();

                Console.WriteLine($"Record #{id} is deleted.");
            }
            catch (ArgumentNullException argumentNullException)
            {
                Console.WriteLine(argumentNullException.Message);
            }
            catch (ArgumentException argumentException)
            {
                Console.WriteLine(argumentException.Message);
            }
            catch (KeyNotFoundException keyNotFoundException)
            {
                Console.WriteLine(keyNotFoundException.Message);
            }
        }

        private int GetUniqueId()
        {
            int id = 1;

            while (this.storedIdRecords.ContainsKey(id))
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
            this.storedIdRecords.Clear();
            this.firstNameDictionary.Clear();
            this.lastNameDictionary.Clear();
            this.dateOfBirthDictionary.Clear();
            this.personalRatingDictionary.Clear();
            this.salaryDictionary.Clear();
            this.genderDictionary.Clear();
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

                this.AddRecordToDictionaries(record, i);
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

        private List<int> GetListOfNameFromDictionary(string parametrName, Dictionary<string, List<int>> dictionary)
        {
            if (dictionary.ContainsKey(parametrName))
            {
                // List of id's, but need positions.
                List<int> listOfPositions = new List<int>();

                for (int i = 0; i < dictionary[parametrName].Count; i++)
                {
                    var position = this.storedIdRecords[dictionary[parametrName][i]] * RecordSize;
                    listOfPositions.Add(position);
                }

                return listOfPositions;
            }

            return new List<int>();
        }

        private List<int> GetListOfDateTimeFromDictionary(string parametrName, Dictionary<DateTime, List<int>> dictionary)
        {
            bool isDateValid = DateTime.TryParse(parametrName, out DateTime birthDate);
            if (isDateValid && dictionary.ContainsKey(birthDate))
            {
                // List of id's, but need positions.
                List<int> listOfPositions = new List<int>();

                for (int i = 0; i < dictionary[birthDate].Count; i++)
                {
                    var position = this.storedIdRecords[dictionary[birthDate][i]] * RecordSize;
                    listOfPositions.Add(position);
                }

                return listOfPositions;
            }

            return new List<int>();
        }

        private List<int> GetListOfPersonalRatingFromDictionary(string parametrName, Dictionary<short, List<int>> dictionary)
        {
            bool isPersonalRatingValid = short.TryParse(parametrName, out short personalRating);
            if (isPersonalRatingValid && dictionary.ContainsKey(personalRating))
            {
                // List of id's, but need positions.
                List<int> listOfPositions = new List<int>();

                for (int i = 0; i < dictionary[personalRating].Count; i++)
                {
                    var position = this.storedIdRecords[dictionary[personalRating][i]] * RecordSize;
                    listOfPositions.Add(position);
                }

                return listOfPositions;
            }

            return new List<int>();
        }

        private List<int> GetListOfSalaryFromDictionary(string parametrName, Dictionary<decimal, List<int>> dictionary)
        {
            bool isSalaryValid = decimal.TryParse(parametrName, out decimal salary);
            if (isSalaryValid && dictionary.ContainsKey(salary))
            {
                // List of id's, but need positions.
                List<int> listOfPositions = new List<int>();

                for (int i = 0; i < dictionary[salary].Count; i++)
                {
                    var position = this.storedIdRecords[dictionary[salary][i]] * RecordSize;
                    listOfPositions.Add(position);
                }

                return listOfPositions;
            }

            return new List<int>();
        }

        private List<int> GetListOfGenderFromDictionary(string parametrName, Dictionary<char, List<int>> dictionary)
        {
            bool isGenderValid = char.TryParse(parametrName, out char gender);
            if (isGenderValid && dictionary.ContainsKey(gender))
            {
                // List of id's, but need positions.
                List<int> listOfPositions = new List<int>();

                for (int i = 0; i < dictionary[gender].Count; i++)
                {
                    var position = this.storedIdRecords[dictionary[gender][i]] * RecordSize;
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
