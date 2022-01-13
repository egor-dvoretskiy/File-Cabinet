using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Iterators;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Records Processor Class.
    /// </summary>
    public class FileCabinetMemoryService : FileCabinetDictionary, IFileCabinetService
    {
        private IRecordValidator recordValidator;

        private List<FileCabinetRecord> list = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="recordValidator">Validator for records.</param>
        public FileCabinetMemoryService(IRecordValidator recordValidator)
        {
            this.recordValidator = recordValidator;
        }

        /// <summary>
        /// Creates record and adds to main list.
        /// </summary>
        /// <param name="record">Input parameter object.</param>
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

                record.Id = this.GetUniqueId();

                this.list.Add(record);

                int position = this.list.Count - 1;

                this.AddRecordToDictionaries(record, position);

                Console.WriteLine($"Record #{record.Id} is created.");
            }
            catch (ArgumentNullException argumentNullException)
            {
                Console.WriteLine(argumentNullException.Message);
            }
            catch (ArgumentOutOfRangeException argumentOutOfRangeException)
            {
                Console.WriteLine(argumentOutOfRangeException.Message);
            }
            catch (ArgumentException argumentException)
            {
                Console.WriteLine(argumentException.Message);
            }
        }

        /// <summary>
        /// Method return all stored records.
        /// </summary>
        /// <returns>Stored records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return new ReadOnlyCollection<FileCabinetRecord>(this.list);
        }

        /// <summary>
        /// Method return records count.
        /// </summary>
        public void GetStat()
        {
            Console.WriteLine($"{this.list.Count} record(s).");
        }

        /// <inheritdoc/>
        public bool CheckRecordPresence(int id) => this.storedIdRecords.ContainsKey(id);

        /// <summary>
        /// Searches all matches by firstname parameter.
        /// </summary>
        /// <param name="firstName">Person's first name.</param>
        /// <returns>All records with the same firstname.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName) => this.Memoized(firstName, x =>
        {
            List<int> listIdRecordsPositions = new List<int>();

            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                var listFirstNameIds = this.firstNameDictionary[firstName];
                int id = 0;

                for (int i = 0; i < listFirstNameIds.Count; i++)
                {
                    id = listFirstNameIds[i];
                    listIdRecordsPositions.Add(this.storedIdRecords[id]);
                }
            }

            return new RecordMemoryEnumerable(this.list, listIdRecordsPositions);
        });

        /// <summary>
        /// Searches all matches by lastName parameter.
        /// </summary>
        /// <param name="lastName">Person's last name.</param>
        /// <returns>All records with the same lastname.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName) => this.Memoized(lastName, x =>
        {
            List<int> listIdRecordsPositions = new List<int>();

            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                var listLastNameIds = this.lastNameDictionary[lastName];
                int id = 0;

                for (int i = 0; i < listLastNameIds.Count; i++)
                {
                    id = listLastNameIds[i];
                    listIdRecordsPositions.Add(this.storedIdRecords[id]);
                }
            }

            return new RecordMemoryEnumerable(this.list, listIdRecordsPositions);
        });

        /// <summary>
        /// Searches all matches by birthDate parameter.
        /// </summary>
        /// <param name="birthDate">Person's date of birth.</param>
        /// <returns>All records with the same date of birth.</returns>
        public IEnumerable<FileCabinetRecord> FindByBirthDate(string birthDate) => this.Memoized(birthDate, x =>
        {
            List<int> listIdRecordsPositions = new List<int>();

            var isValidBirth = DateTime.TryParse(birthDate, out DateTime validBirthDate);

            if (isValidBirth && this.dateOfBirthDictionary.ContainsKey(validBirthDate))
            {
                var listBirthDatesIds = this.dateOfBirthDictionary[validBirthDate];
                int id = 0;

                for (int i = 0; i < listBirthDatesIds.Count; i++)
                {
                    id = listBirthDatesIds[i];
                    listIdRecordsPositions.Add(this.storedIdRecords[id]);
                }
            }

            return new RecordMemoryEnumerable(this.list, listIdRecordsPositions);
        });

        /// <summary>
        /// Searches all matches by personalRating parameter.
        /// </summary>
        /// <param name="personalRating">Person's personal rating.</param>
        /// <returns>All records with the same personal rating.</returns>
        public IEnumerable<FileCabinetRecord> FindByPersonalRating(string personalRating) => this.Memoized(personalRating, x =>
        {
            List<int> listIdRecordsPositions = new List<int>();

            var isValidPersonalRating = short.TryParse(personalRating, out short validPersonalRating);

            if (isValidPersonalRating && this.personalRatingDictionary.ContainsKey(validPersonalRating))
            {
                var listOfPersonalRating = this.personalRatingDictionary[validPersonalRating];
                int id = 0;

                for (int i = 0; i < listOfPersonalRating.Count; i++)
                {
                    id = listOfPersonalRating[i];
                    listIdRecordsPositions.Add(this.storedIdRecords[id]);
                }
            }

            return new RecordMemoryEnumerable(this.list, listIdRecordsPositions);
        });

        /// <summary>
        /// Searches all matches by salary parameter.
        /// </summary>
        /// <param name="salary">Person's salary.</param>
        /// <returns>All records with the same salary.</returns>
        public IEnumerable<FileCabinetRecord> FindBySalary(string salary) => this.Memoized(salary, x =>
        {
            List<int> listIdRecordsPositions = new List<int>();

            var isValidSalary = decimal.TryParse(salary, out decimal validSalary);

            if (isValidSalary && this.salaryDictionary.ContainsKey(validSalary))
            {
                var listSalaryIds = this.salaryDictionary[validSalary];
                int id = 0;

                for (int i = 0; i < listSalaryIds.Count; i++)
                {
                    id = listSalaryIds[i];
                    listIdRecordsPositions.Add(this.storedIdRecords[id]);
                }
            }

            return new RecordMemoryEnumerable(this.list, listIdRecordsPositions);
        });

        /// <summary>
        /// Searches all matches by gender parameter.
        /// </summary>
        /// <param name="gender">Person's gender.</param>
        /// <returns>All records with the same gender.</returns>
        public IEnumerable<FileCabinetRecord> FindByGender(string gender) => this.Memoized(gender, x =>
        {
            List<int> listIdRecordsPositions = new List<int>();

            var isValidGender = char.TryParse(gender, out char validGender);

            if (isValidGender && this.genderDictionary.ContainsKey(validGender))
            {
                var listBirthDatesIds = this.genderDictionary[validGender];
                int id = 0;

                for (int i = 0; i < listBirthDatesIds.Count; i++)
                {
                    id = listBirthDatesIds[i];
                    listIdRecordsPositions.Add(this.storedIdRecords[id]);
                }
            }

            return new RecordMemoryEnumerable(this.list, listIdRecordsPositions);
        });

        /// <summary>
        /// Makes snapshot of FileCabinetService.
        /// </summary>
        /// <returns>Snapshot of FileCabinetService.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list, this.recordValidator);
        }

        /// <summary>
        /// Restores record's data from file.
        /// </summary>
        /// <param name="fileCabinetServiceSnapshot">Holds snapshot of data.</param>
        public void Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot)
        {
            var unloadRecords = fileCabinetServiceSnapshot.Records.ToList();

            for (int i = 0; i < unloadRecords.Count; i++)
            {
                var record = unloadRecords[i];

                this.EditRecordInDictionaries(record, i);
            }

            this.list = unloadRecords;
        }

        /// <inheritdoc/>
        public void Delete(List<int> ids)
        {
            MemoizerForMemoryService.RefreshMemoizer();

            for (int i = 0; i < ids.Count; i++)
            {
                this.RemoveRecordById(ids[i]);
            }
        }

        /// <inheritdoc/>
        public void Purge()
        {
            Console.WriteLine("Wrong service, please switch memory mode to file system.");
        }

        /// <inheritdoc/>
        public void InsertRecord(FileCabinetRecord record)
        {
            MemoizerForMemoryService.RefreshMemoizer();

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

                this.list.Add(record);

                int position = this.list.Count - 1;

                this.AddRecordToDictionaries(record, position);

                Console.WriteLine($"Record was successfully inserted in memory.");
            }
            catch (ArgumentNullException argumentNullException)
            {
                Console.WriteLine(argumentNullException.Message);
            }
            catch (ArgumentOutOfRangeException argumentOutOfRangeException)
            {
                Console.WriteLine(argumentOutOfRangeException.Message);
            }
            catch (ArgumentException argumentException)
            {
                Console.WriteLine(argumentException.Message);
            }
        }

        /// <inheritdoc/>
        public void Update(List<FileCabinetRecord> records)
        {
            MemoizerForMemoryService.RefreshMemoizer();

            for (int i = 0; i < records.Count; i++)
            {
                this.EditRecord(records[i].Id, records[i]);
            }

            Console.WriteLine($"Records updating completed.");
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetRecord(int id)
        {
            // with preliminary check (checkpresenceid)
            return this.list[this.storedIdRecords[id]];
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
                int listIndex = this.storedIdRecords[id];

                FileCabinetRecord recordToDelete = this.list[listIndex];

                this.RemoveRecordFromDictionaries(id, listIndex, recordToDelete);

                this.list.RemoveAt(listIndex);

                this.UpdateIdPositioningDictionaryAccordingToRemoveRecord(listIndex);

                Console.WriteLine($"Record #{id} is deleted.");
            }
            catch (ArgumentException argumentException)
            {
                Console.WriteLine(argumentException.Message);
            }
        }

        private void EditRecord(int id, FileCabinetRecord record)
        {
            try
            {
                bool isValid = this.recordValidator.ValidateParameters(record);

                if (!isValid)
                {
                    Console.WriteLine($"Record validation failed.");
                    return;
                }

                int index = this.list.FindIndex(x => x.Id == id);

                this.list[index] = record;

                this.EditRecordInDictionaries(record, index);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Console.WriteLine(argumentNullException.Message);
            }
            catch (ArgumentOutOfRangeException argumentOutOfRangeException)
            {
                Console.WriteLine(argumentOutOfRangeException.Message);
            }
            catch (ArgumentException argumentException)
            {
                Console.WriteLine(argumentException.Message);
            }
        }

        private void UpdateIdPositioningDictionaryAccordingToRemoveRecord(int indexInList)
        {
            if (indexInList >= this.list.Count)
            {
                return;
            }

            for (int i = indexInList; i < this.list.Count; i++)
            {
                this.storedIdRecords[this.list[i].Id] -= 1;
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
    }
}
