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
        void CreateRecord(FileCabinetRecord record);

        /// <summary>
        /// Adds record with specific group of parameters.
        /// </summary>
        /// <param name="record">File cabinet record.</param>
        void InsertRecord(FileCabinetRecord record);

        /// <summary>
        /// Acquire record from storage by id.
        /// </summary>
        /// <param name="id">Record's id.</param>
        /// <returns>Returns record.</returns>
        FileCabinetRecord GetRecord(int id);

        /// <summary>
        /// Searches all matches by birthDate parameter.
        /// </summary>
        /// <param name="birthDate">Person's date of birth.</param>
        /// <returns>All records with the same date of birth.</returns>
        IEnumerable<FileCabinetRecord> FindByBirthDate(string birthDate);

        /// <summary>
        /// Searches all matches by firstname parameter.
        /// </summary>
        /// <param name="firstName">Person's first name.</param>
        /// <returns>All records with the same firstname.</returns>
        IEnumerable<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Searches all matches by lastName parameter.
        /// </summary>
        /// <param name="lastName">Person's last name.</param>
        /// <returns>All records with the same lastname.</returns>
        IEnumerable<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Searches all matches by personalRating parameter.
        /// </summary>
        /// <param name="personalRating">Person's personal rating.</param>
        /// <returns>All records with the same personal rating.</returns>
        IEnumerable<FileCabinetRecord> FindByPersonalRating(string personalRating);

        /// <summary>
        /// Searches all matches by salary parameter.
        /// </summary>
        /// <param name="salary">Person's salary.</param>
        /// <returns>All records with the same salary.</returns>
        IEnumerable<FileCabinetRecord> FindBySalary(string salary);

        /// <summary>
        /// Searches all matches by gender parameter.
        /// </summary>
        /// <param name="gender">Person's gender.</param>
        /// <returns>All records with the same gender.</returns>
        IEnumerable<FileCabinetRecord> FindByGender(string gender);

        /// <summary>
        /// Method checks record presence in list by ID.
        /// </summary>
        /// <param name="id">Record's id.</param>
        /// <returns>Boolean present in list.</returns>
        bool CheckRecordPresence(int id);

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
        /// Delete records using input list of ids.
        /// </summary>
        /// <param name="ids">List of record ids.</param>
        void Delete(List<int> ids);

        /// <summary>
        /// Update records using input list of records.
        /// </summary>
        /// <param name="records">Records to update.</param>
        void Update(List<FileCabinetRecord> records);

        /// <summary>
        /// Defragments the data file.
        /// </summary>
        void Purge();
    }
}