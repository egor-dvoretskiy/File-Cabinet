using System.Collections.ObjectModel;

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
        ReadOnlyCollection<FileCabinetRecord> FindByBirthDate(string birthDate);

        /// <summary>
        /// Searches all matches by firstname parameter.
        /// </summary>
        /// <param name="firstName">Person's first name.</param>
        /// <returns>All records with the same firstname.</returns>
        ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Searches all matches by lastName parameter.
        /// </summary>
        /// <param name="lastName">Person's last name.</param>
        /// <returns>All records with the same lastname.</returns>
        ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Method find record position in list by ID.
        /// </summary>
        /// <param name="id">Record's id.</param>
        /// <returns>Record's position in list.</returns>
        int GetRecordPosition(int id);

        /// <summary>
        /// Method return all stored records.
        /// </summary>
        /// <returns>Stored records.</returns>
        ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Method return records count.
        /// </summary>
        /// <returns>Amount of records.</returns>
        int GetStat();

        /// <summary>
        /// Makes snapshot of FileCabinetService.
        /// </summary>
        /// <param name="recordValidator">Validator for importing files.</param>
        /// <returns>Snapshot of FileCabinetService.</returns>
        FileCabinetServiceSnapshot MakeSnapshot(IRecordValidator recordValidator);

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