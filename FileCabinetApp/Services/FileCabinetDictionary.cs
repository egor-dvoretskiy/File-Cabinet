using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable SA1401 // Fields should be private

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Class that holds dictionaries methods.
    /// </summary>
    public abstract class FileCabinetDictionary
    {
        /// <summary>
        /// 1st arg - record's id, 2nd arg - position in list/file.
        /// </summary>
        protected Dictionary<int, int> storedIdRecords = new Dictionary<int, int>();

        /// <summary>
        /// Dictionary that holds firstName parameter refers to list of IDs.
        /// 1st arg - firstName, 2nd - list of records id that matches parameter.
        /// </summary>
        protected Dictionary<string, List<int>> firstNameDictionary = new Dictionary<string, List<int>>();

        /// <summary>
        /// Dictionary that holds lastName parameter refers to list of IDs.
        /// 1st arg - lastName, 2nd - list of records id that matches parameter.
        /// </summary>
        protected Dictionary<string, List<int>> lastNameDictionary = new Dictionary<string, List<int>>();

        /// <summary>
        /// Dictionary that holds birthDate parameter refers to list of IDs.
        /// 1st arg - birthDate, 2nd - list of records id that matches parameter.
        /// </summary>
        protected Dictionary<DateTime, List<int>> dateOfBirthDictionary = new Dictionary<DateTime, List<int>>();

        /// <summary>
        /// Dictionary that holds personalRating parameter refers to list of IDs.
        /// 1st arg - personalRating, 2nd - list of records id that matches parameter.
        /// </summary>
        protected Dictionary<short, List<int>> personalRatingDictionary = new Dictionary<short, List<int>>();

        /// <summary>
        /// Dictionary that holds salary parameter refers to list of IDs.
        /// 1st arg - salary, 2nd - list of records id that matches parameter.
        /// </summary>
        protected Dictionary<decimal, List<int>> salaryDictionary = new Dictionary<decimal, List<int>>();

        /// <summary>
        /// Dictionary that holds gender parameter refers to list of IDs.
        /// 1st arg - gender, 2nd - list of records id that matches parameter.
        /// </summary>
        protected Dictionary<char, List<int>> genderDictionary = new Dictionary<char, List<int>>();

        /// <summary>
        /// Adds record to dictionaries, that simplify record position determining.
        /// </summary>
        /// <param name="record">File cabinet record to add.</param>
        /// <param name="position">Current position of record in container.</param>
        protected void AddRecordToDictionaries(FileCabinetRecord record, int position)
        {
            this.AddOrChangeInformationInIdDictionary(record.Id, position, ref this.storedIdRecords);

            this.AddRecordReferenceToDictionary<string>(record.FirstName, record.Id, ref this.firstNameDictionary);
            this.AddRecordReferenceToDictionary<string>(record.LastName, record.Id, ref this.lastNameDictionary);
            this.AddRecordReferenceToDictionary<DateTime>(record.DateOfBirth, record.Id, ref this.dateOfBirthDictionary);
            this.AddRecordReferenceToDictionary<short>(record.PersonalRating, record.Id, ref this.personalRatingDictionary);
            this.AddRecordReferenceToDictionary<decimal>(record.Salary, record.Id, ref this.salaryDictionary);
            this.AddRecordReferenceToDictionary<char>(record.Gender, record.Id, ref this.genderDictionary);
        }

        /// <summary>
        /// Edit record in dictionaries, that simplify record position determining.
        /// </summary>
        /// <param name="record">File cabinet record to edit.</param>
        /// <param name="position">Current position of record in container.</param>
        protected void EditRecordInDictionaries(FileCabinetRecord record, int position)
        {
            this.AddOrChangeInformationInIdDictionary(record.Id, position, ref this.storedIdRecords);

            this.EditRecordReferencesInDictionary<string>(record.FirstName, record.Id, ref this.firstNameDictionary);
            this.EditRecordReferencesInDictionary<string>(record.LastName, record.Id, ref this.lastNameDictionary);
            this.EditRecordReferencesInDictionary<DateTime>(record.DateOfBirth, record.Id, ref this.dateOfBirthDictionary);
            this.EditRecordReferencesInDictionary<short>(record.PersonalRating, record.Id, ref this.personalRatingDictionary);
            this.EditRecordReferencesInDictionary<decimal>(record.Salary, record.Id, ref this.salaryDictionary);
            this.EditRecordReferencesInDictionary<char>(record.Gender, record.Id, ref this.genderDictionary);
        }

        /// <summary>
        /// Removes record from dictionaries, that simplify record position determining.
        /// </summary>
        /// <param name="id">Record's id to remove.</param>
        /// <param name="listIndex">Current position of record in container.</param>
        /// <param name="recordToDelete">Set of parameters to remove.</param>
        protected void RemoveRecordFromDictionaries(int id, int listIndex, FileCabinetRecord recordToDelete)
        {
            this.RemoveRecordFromDictionary<string>(recordToDelete.FirstName, recordToDelete.Id, ref this.firstNameDictionary);
            this.RemoveRecordFromDictionary<string>(recordToDelete.LastName, recordToDelete.Id, ref this.lastNameDictionary);
            this.RemoveRecordFromDictionary<DateTime>(recordToDelete.DateOfBirth, recordToDelete.Id, ref this.dateOfBirthDictionary);
            this.RemoveRecordFromDictionary<short>(recordToDelete.PersonalRating, recordToDelete.Id, ref this.personalRatingDictionary);
            this.RemoveRecordFromDictionary<decimal>(recordToDelete.Salary, recordToDelete.Id, ref this.salaryDictionary);
            this.RemoveRecordFromDictionary<char>(recordToDelete.Gender, recordToDelete.Id, ref this.genderDictionary);

            this.storedIdRecords.Remove(id);
        }

        /// <summary>
        /// Clears all dictionaries.
        /// </summary>
        protected void ClearDictionaries()
        {
            this.storedIdRecords.Clear();
            this.firstNameDictionary.Clear();
            this.lastNameDictionary.Clear();
            this.dateOfBirthDictionary.Clear();
            this.personalRatingDictionary.Clear();
            this.salaryDictionary.Clear();
            this.genderDictionary.Clear();
        }

        private void AddOrChangeInformationInIdDictionary(int id, int position, ref Dictionary<int, int> dict)
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

        private void AddRecordReferenceToDictionary<T>(T parameter, int recordId, ref Dictionary<T, List<int>> dict)
            where T : notnull
        {
            if (!dict.ContainsKey(parameter))
            {
                dict.Add(parameter, new List<int>() { recordId });
            }
            else
            {
                dict[parameter].Add(recordId);
            }
        }

        private void EditRecordReferencesInDictionary<T>(T parameter, int recordId, ref Dictionary<T, List<int>> dict)
            where T : notnull
        {
            foreach (var element in dict)
            {
                int index = element.Value.IndexOf(recordId);

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

            this.AddRecordReferenceToDictionary<T>(parameter, recordId, ref dict);
        }

        private void RemoveRecordFromDictionary<T>(T parameter, int recordId, ref Dictionary<T, List<int>> dict)
            where T : notnull
        {
            dict[parameter].Remove(recordId);

            if (dict[parameter].Count == 0)
            {
                dict.Remove(parameter);
            }
        }
    }
}
