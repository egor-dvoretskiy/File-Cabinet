using System.Collections.ObjectModel;

namespace FileCabinetApp
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
        int GetPositionInListRecordsById(int id);

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
    }
}