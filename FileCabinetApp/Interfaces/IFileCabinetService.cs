﻿using System.Collections.ObjectModel;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Records Processor interface.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Creates record and adds to main list.
        /// </summary>
        /// <param name="record">Input parameter object.</param>
        /// <returns>Record's id in list.</returns>
        int CreateRecord(FileCabinetRecord record);

        /// <summary>
        /// Edit record in list.
        /// </summary>
        /// <param name="id">Record's id in list.</param>
        /// <param name="record">Input parameter object.</param>
        /// <exception cref="ArgumentException">id.</exception>
        void EditRecord(int id, FileCabinetRecord record);

        /// <summary>
        /// Searches all matches by birthDate parameter.
        /// </summary>
        /// <param name="birthDate">Person's date of birth.</param>
        /// <returns>All records with the same date of birth.</returns>
        IRecordIterator FindByBirthDate(string birthDate);

        /// <summary>
        /// Searches all matches by firstname parameter.
        /// </summary>
        /// <param name="firstName">Person's first name.</param>
        /// <returns>All records with the same firstname.</returns>
        IRecordIterator FindByFirstName(string firstName);

        /// <summary>
        /// Searches all matches by lastName parameter.
        /// </summary>
        /// <param name="lastName">Person's last name.</param>
        /// <returns>All records with the same lastname.</returns>
        IRecordIterator FindByLastName(string lastName);

        /// <summary>
        /// Method checks record presence in list by ID.
        /// </summary>
        /// <param name="id">Record's id.</param>
        void CheckRecordPresence(int id);

        /// <summary>
        /// Method return all stored records.
        /// </summary>
        /// <returns>Stored records.</returns>
        ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Method return records count.
        /// </summary>
        void GetStat();

        /// <summary>
        /// Makes snapshot of FileCabinetService.
        /// </summary>
        /// <returns>Snapshot of FileCabinetService.</returns>
        FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Restores data from file.
        /// </summary>
        /// <param name="fileCabinetServiceSnapshot">Snapshot.</param>
        void Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot);

        /// <summary>
        /// Removes record from container.
        /// </summary>
        /// <param name="id">Record's id.</param>
        void RemoveRecordById(int id);

        /// <summary>
        /// Defragments the data file.
        /// </summary>
        void Purge();
    }
}