﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Validators;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Records Processor Class.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private IRecordValidator recordValidator;

        private List<FileCabinetRecord> list = new ();

        private Dictionary<int, FileCabinetRecord> storedIdRecords = new Dictionary<int, FileCabinetRecord>();

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

                this.AddOrChangeInformationInIdDictionary(record.Id, ref this.storedIdRecords, record);

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
        /// Edit record in list.
        /// </summary>
        /// <param name="id">Record's id in list.</param>
        /// <param name="record">Input parameter object.</param>
        /// <exception cref="ArgumentException">id.</exception>
        public void EditRecord(int id, FileCabinetRecord record)
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

                this.AddOrChangeInformationInIdDictionary(record.Id, ref this.storedIdRecords, record);

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

        /// <summary>
        /// Method find record position in list by ID.
        /// </summary>
        /// <param name="id">Record's id.</param>
        public void CheckRecordPresence(int id)
        {
            if (!this.storedIdRecords.ContainsKey(id))
            {
                throw new ArgumentException($"#{id} record is not found.");
            }
        }

        /// <summary>
        /// Searches all matches by firstname parameter.
        /// </summary>
        /// <param name="firstName">Person's first name.</param>
        /// <returns>All records with the same firstname.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return new ReadOnlyCollection<FileCabinetRecord>(this.GetInformationFromDictionary(firstName, this.firstNameDictionary));
        }

        /// <summary>
        /// Searches all matches by lastName parameter.
        /// </summary>
        /// <param name="lastName">Person's last name.</param>
        /// <returns>All records with the same lastname.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            return new ReadOnlyCollection<FileCabinetRecord>(this.GetInformationFromDictionary(lastName, this.lastNameDictionary));
        }

        /// <summary>
        /// Searches all matches by birthDate parameter.
        /// </summary>
        /// <param name="birthDate">Person's date of birth.</param>
        /// <returns>All records with the same date of birth.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByBirthDate(string birthDate)
        {
            return new ReadOnlyCollection<FileCabinetRecord>(this.GetInformationFromDictionary(birthDate, this.dateOfBirthDictionary));
        }

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

                this.AddOrChangeInformationInIdDictionary(record.Id, ref this.storedIdRecords, record);

                this.EditInformationInDictionary(record.FirstName, ref this.firstNameDictionary, record);
                this.EditInformationInDictionary(record.LastName, ref this.lastNameDictionary, record);
                this.EditInformationInDictionary(record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), ref this.dateOfBirthDictionary, record);
            }

            this.LoadRecordsToList();
        }

        /// <summary>
        /// Removes record by id.
        /// </summary>
        /// <param name="id">Id to remove.</param>
        public void RemoveRecordById(int id)
        {
            try
            {
                int positionInList = this.list.FindIndex(x => x.Id == id);

                this.list.RemoveAt(positionInList);

                string firstName = this.storedIdRecords[id].FirstName;
                this.RemoveRecordFromDictionary(firstName, id, ref this.firstNameDictionary);

                string lastName = this.storedIdRecords[id].LastName;
                this.RemoveRecordFromDictionary(lastName, id, ref this.lastNameDictionary);

                string dateOfBirth = this.storedIdRecords[id].DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
                this.RemoveRecordFromDictionary(dateOfBirth, id, ref this.dateOfBirthDictionary);

                this.storedIdRecords.Remove(id);
            }
            catch (ArgumentException argumentException)
            {
                Console.WriteLine(argumentException.Message);
            }
        }

        /// <inheritdoc/>
        public void Purge()
        {
            Console.WriteLine("Wrong service, please switch memory mode to file system.");
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

        private void AddOrChangeInformationInIdDictionary(int parametrName, ref Dictionary<int, FileCabinetRecord> dict, FileCabinetRecord record)
        {
            if (!dict.ContainsKey(parametrName))
            {
                dict.Add(parametrName, record);
            }
            else
            {
                dict[parametrName] = record;
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

        private List<FileCabinetRecord> GetInformationFromDictionary(string parametrName, Dictionary<string, List<int>> dictionary)
        {
            if (dictionary.ContainsKey(parametrName))
            {
                List<FileCabinetRecord> listOfPositions = new List<FileCabinetRecord>();

                for (int i = 0; i < dictionary[parametrName].Count; i++)
                {
                    listOfPositions.Add(this.storedIdRecords[dictionary[parametrName][i]]);
                }

                return listOfPositions;
            }

            return new List<FileCabinetRecord>();
        }

        private void LoadRecordsToList()
        {
            this.list = new List<FileCabinetRecord>();

            foreach (var item in this.storedIdRecords.Values)
            {
                this.list.Add(item);
            }
        }
    }
}
