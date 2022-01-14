using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileCabinetApp.ConditionWords;
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

                byte[] buffer = ServiceBufferCommunicator.WriteRecordToBuffer(record, RecordSize, MaxNameLength);

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

            var tupleReadFromFile = ServiceBufferCommunicator.ReadRecordFromBuffer(bytes, MaxNameLength);

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

                    var tupleReadFromFile = ServiceBufferCommunicator.ReadRecordFromBuffer(bytes, MaxNameLength);

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

                byte[] buffer = ServiceBufferCommunicator.WriteRecordToBuffer(record, RecordSize, MaxNameLength);

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

                    var tupleReadFromFile = ServiceBufferCommunicator.ReadRecordFromBuffer(bytes, MaxNameLength);

                    if (tupleReadFromFile.Item2 == 1)
                    {
                        previousDeletedCount++;
                    }
                    else if ((tupleReadFromFile.Item2 == 0 && previousDeletedCount > 0) || currentPosition != i)
                    {
                        byte[] buffer = ServiceBufferCommunicator.WriteRecordToBuffer(tupleReadFromFile.Item1, RecordSize, MaxNameLength);

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

                byte[] buffer = ServiceBufferCommunicator.WriteRecordToBuffer(record, RecordSize, MaxNameLength);

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

        /// <inheritdoc/>
        public List<FileCabinetRecord> Select(string phrase, string memoizingKey, IRecordInputValidator inputValidator)
        {
            ConditionWhere where = new ConditionWhere(this, inputValidator);
            var records = where.GetFilteredRecords(phrase);

            return records;
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

                byte[] buffer = ServiceBufferCommunicator.WriteRecordToBuffer(record, RecordSize, MaxNameLength);
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

        private void AssignRecordValuesToDictionaries()
        {
            this.recordMarkedAsDeletedCount = 0;
            this.fileStream.Seek(0, SeekOrigin.Begin);

            for (int i = 0; i < this.recordsCount; i++)
            {
                byte[] bytes = new byte[RecordSize];
                this.fileStream.Read(bytes, 0, RecordSize);

                var tupleReadFromFile = ServiceBufferCommunicator.ReadRecordFromBuffer(bytes, MaxNameLength);

                if (tupleReadFromFile.Item2 == 1)
                {
                    this.recordMarkedAsDeletedCount++;
                    continue;
                }

                FileCabinetRecord record = tupleReadFromFile.Item1;

                this.AddRecordToDictionaries(record, i);
            }
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
