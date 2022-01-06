﻿using System;
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
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private IRecordValidator recordValidator;

        private List<FileCabinetRecord> list = new ();

        /// <summary>
        /// 1st arg - record's id, 2nd arg - position in list.
        /// </summary>
        private Dictionary<int, int> storedIdRecords = new Dictionary<int, int>();

        private Dictionary<string, List<int>> firstNameDictionary = new Dictionary<string, List<int>>();
        private Dictionary<string, List<int>> lastNameDictionary = new Dictionary<string, List<int>>();
        private Dictionary<string, List<int>> dateOfBirthDictionary = new Dictionary<string, List<int>>();

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
        /// <returns>Record's id in list.</returns>
        public int CreateRecord(FileCabinetRecord record)
        {
            try
            {
                bool isValid = this.recordValidator.ValidateParameters(record);

                if (!isValid)
                {
                    return -1;
                }

                record.Id = this.GetUniqueId();

                this.list.Add(record);

                int position = this.list.Count - 1;

                this.AddOrChangeInformationInIdDictionary(record.Id, position, ref this.storedIdRecords, record);

                this.AddInformationToDictionary(record.FirstName, ref this.firstNameDictionary, record);
                this.AddInformationToDictionary(record.LastName, ref this.lastNameDictionary, record);
                this.AddInformationToDictionary(record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), ref this.dateOfBirthDictionary, record);

                return record.Id;
            }
            catch (ArgumentNullException anex)
            {
                Console.WriteLine(anex.Message);
            }
            catch (ArgumentOutOfRangeException aorex)
            {
                Console.WriteLine(aorex.Message);
            }
            catch (ArgumentException aex)
            {
                Console.WriteLine(aex.Message);
            }

            return -1;
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
        public bool CheckRecordPresence(int id)
        {
            bool listIdPresent = true;

            if (!this.storedIdRecords.ContainsKey(id))
            {
                listIdPresent = false;
            }

            return listIdPresent;
        }

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
            birthDate = validBirthDate.ToString("yyyy-MMM-dd");

            if (isValidBirth && this.dateOfBirthDictionary.ContainsKey(birthDate))
            {
                var listBirthDatesIds = this.dateOfBirthDictionary[birthDate];
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

                this.AddOrChangeInformationInIdDictionary(record.Id, i, ref this.storedIdRecords, record);

                this.EditInformationInDictionary(record.FirstName, ref this.firstNameDictionary, record);
                this.EditInformationInDictionary(record.LastName, ref this.lastNameDictionary, record);
                this.EditInformationInDictionary(record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), ref this.dateOfBirthDictionary, record);
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

                this.AddOrChangeInformationInIdDictionary(record.Id, position, ref this.storedIdRecords, record);

                this.AddInformationToDictionary(record.FirstName, ref this.firstNameDictionary, record);
                this.AddInformationToDictionary(record.LastName, ref this.lastNameDictionary, record);
                this.AddInformationToDictionary(record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), ref this.dateOfBirthDictionary, record);

                Console.WriteLine($"Record was successfully inserted in memory.");
            }
            catch (ArgumentNullException anex)
            {
                Console.WriteLine(anex.Message);
            }
            catch (ArgumentOutOfRangeException aorex)
            {
                Console.WriteLine(aorex.Message);
            }
            catch (ArgumentException aex)
            {
                Console.WriteLine(aex.Message);
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

            Console.WriteLine($"Record's updating completed.");
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetRecord(int id)
        {
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
                string firstName = this.list[this.storedIdRecords[id]].FirstName;
                this.RemoveRecordFromDictionary(firstName, id, ref this.firstNameDictionary);

                string lastName = this.list[this.storedIdRecords[id]].LastName;
                this.RemoveRecordFromDictionary(lastName, id, ref this.lastNameDictionary);

                string dateOfBirth = this.list[this.storedIdRecords[id]].DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
                this.RemoveRecordFromDictionary(dateOfBirth, id, ref this.dateOfBirthDictionary);

                int indexInList = this.storedIdRecords[id];

                this.storedIdRecords.Remove(id);

                int positionInList = this.list.FindIndex(x => x.Id == id);
                this.list.RemoveAt(positionInList);

                this.UpdateIdDictionaryAccordingToRemoveRecord(indexInList);

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
                    return;
                }

                int index = this.list.FindIndex(x => x.Id == id);

                this.list[index] = record;

                this.AddOrChangeInformationInIdDictionary(record.Id, index, ref this.storedIdRecords, record);

                this.EditInformationInDictionary(record.FirstName, ref this.firstNameDictionary, record);
                this.EditInformationInDictionary(record.LastName, ref this.lastNameDictionary, record);
                this.EditInformationInDictionary(record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), ref this.dateOfBirthDictionary, record);
            }
            catch (ArgumentNullException anex)
            {
                Console.WriteLine(anex.Message);
            }
            catch (ArgumentOutOfRangeException aorex)
            {
                Console.WriteLine(aorex.Message);
            }
            catch (ArgumentException aex)
            {
                Console.WriteLine(aex.Message);
            }
        }

        private void UpdateIdDictionaryAccordingToRemoveRecord(int indexInList)
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

        private void AddInformationToDictionary(string parametrName, ref Dictionary<string, List<int>> dict, FileCabinetRecord record)
        {
            if (!dict.ContainsKey(parametrName))
            {
                dict.Add(parametrName, new List<int>() { record.Id });
            }
            else
            {
                dict[parametrName].Add(record.Id);
            }
        }

        private void AddOrChangeInformationInIdDictionary(int id, int position, ref Dictionary<int, int> dict, FileCabinetRecord record)
        {
            if (!dict.ContainsKey(id))
            {
                dict.Add(id, position);
            }
            else
            {
                dict[id] = position;
            }
        }

        private void EditInformationInDictionary(string parameterName, ref Dictionary<string, List<int>> dict, FileCabinetRecord record)
        {
            foreach (var element in dict)
            {
                int index = element.Value.IndexOf(record.Id);

                if (index != -1)
                {
                    element.Value.RemoveAt(index);

                    if (element.Value.Count == 0)
                    {
                        dict.Remove(element.Key);
                    }

                    break;
                }
            }

            this.AddInformationToDictionary(parameterName, ref dict, record);
        }

        private void RemoveRecordFromDictionary(string parameterName, int id, ref Dictionary<string, List<int>> dict)
        {
            dict[parameterName].Remove(id);
            if (dict[parameterName].Count == 0)
            {
                dict.Remove(parameterName);
            }
        }
    }
}
