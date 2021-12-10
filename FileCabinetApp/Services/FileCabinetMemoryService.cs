using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// Records Processor Class.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();

        private Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        public FileCabinetMemoryService()
        {
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
                record.Id = this.list.Count + 1;

                this.list.Add(record);

                this.AddInformationToDictionary(record.FirstName, ref this.firstNameDictionary, record);
                this.AddInformationToDictionary(record.LastName, ref this.lastNameDictionary, record);
                this.AddInformationToDictionary(record.DateOfBirth.ToString("yyyy-MMM-dd"), ref this.dateOfBirthDictionary, record);

                return record.Id;
            }
            catch (ArgumentNullException anex)
            {
                Console.WriteLine(anex.Message);
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
        public void EditRecord(
            int id,
            FileCabinetRecord record)
        {
            if (id == -1)
            {
                throw new ArgumentException($"{id}");
            }

            try
            {
                record.Id = this.list[id].Id;

                this.list[id] = record;

                this.EditInformationInDictionary(record.FirstName, ref this.firstNameDictionary, record);
                this.EditInformationInDictionary(record.LastName, ref this.lastNameDictionary, record);
                this.EditInformationInDictionary(record.DateOfBirth.ToString("yyyy-MMM-dd"), ref this.dateOfBirthDictionary, record);
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
        /// <returns>Amount of records.</returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <summary>
        /// Method find record position in list by ID.
        /// </summary>
        /// <param name="id">Record's id.</param>
        /// <returns>Record's position in list.</returns>
        public int GetPositionInListRecordsById(int id)
        {
            int leftBorder = 0;
            int rightBorder = this.list.Count;
            int index = -1;
            while (leftBorder < rightBorder)
            {
                int middle = (leftBorder + rightBorder) / 2;
                int checkId = this.list[middle].Id;
                if (checkId > id)
                {
                    rightBorder = middle - 1;
                }
                else if (checkId < id)
                {
                    leftBorder = middle + 1;
                }
                else
                {
                    index = middle;
                    break;
                }
            }

            return index;
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
            return new FileCabinetServiceSnapshot(this.list);
        }

        private void AddInformationToDictionary(string parametrName, ref Dictionary<string, List<FileCabinetRecord>> dict, FileCabinetRecord record)
        {
            if (!dict.ContainsKey(parametrName))
            {
                dict.Add(parametrName, new List<FileCabinetRecord>() { record });
            }
            else
            {
                dict[parametrName].Add(record);
            }
        }

        private void EditInformationInDictionary(string parameterName, ref Dictionary<string, List<FileCabinetRecord>> dict, FileCabinetRecord record)
        {
            foreach (var element in dict)
            {
                int index = element.Value.FindIndex(0, element.Value.Count, i => i.Id == record.Id);

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

        private List<FileCabinetRecord> GetInformationFromDictionary(string parametrName, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            if (dictionary.ContainsKey(parametrName))
            {
                return dictionary[parametrName];
            }

            return new List<FileCabinetRecord>();
        }
    }
}
