using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// Records Processor Abstract Class.
    /// </summary>
    public abstract class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();
        private Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        /// <summary>
        /// Creates record and adds to main list.
        /// </summary>
        /// <param name="recordInputObject">Input parameter object.</param>
        /// <returns>Record's id in list.</returns>
        public int CreateRecord(RecordInputObject recordInputObject)
        {
            try
            {
                var validatedRecord = this.CreateValidator().ValidateParameters(recordInputObject);

                validatedRecord.Id = this.list.Count + 1;

                this.list.Add(validatedRecord);

                this.AddInformationToDictionary(recordInputObject.FirstName, ref this.firstNameDictionary, validatedRecord);
                this.AddInformationToDictionary(recordInputObject.LastName, ref this.lastNameDictionary, validatedRecord);
                this.AddInformationToDictionary(recordInputObject.DateOfBirth, ref this.dateOfBirthDictionary, validatedRecord);

                return validatedRecord.Id;
            }
            catch (ArgumentNullException anex)
            {
                Console.WriteLine(anex.Message);
            }
            catch (ArgumentException aex)
            {
                Console.WriteLine(aex.Message);
            }

            Console.WriteLine("Please, try again:");

            return -1;
        }

        /// <summary>
        /// Edit record in list.
        /// </summary>
        /// <param name="id">Record's id in list.</param>
        /// <param name="recordInputObject">Input parameter object.</param>
        /// <exception cref="ArgumentException">id.</exception>
        public void EditRecord(
            int id,
            RecordInputObject recordInputObject)
        {
            if (id == -1)
            {
                throw new ArgumentException($"{id}");
            }

            try
            {
                var record = this.CreateValidator().ValidateParameters(recordInputObject);

                record.Id = this.list[id].Id;

                this.list[id] = record;

                this.EditInformationInDictionary(recordInputObject.FirstName, ref this.firstNameDictionary, record);
                this.EditInformationInDictionary(recordInputObject.LastName, ref this.lastNameDictionary, record);
                this.EditInformationInDictionary(recordInputObject.DateOfBirth, ref this.dateOfBirthDictionary, record);
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
        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
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
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            /*List<FileCabinetRecord> tempLst = new ();
            int index = 0;

            while (index != -1)
            {
                index = this.list.FindIndex(index, this.list.Count - index, i => string.Equals(i.FirstName, firstName, StringComparison.InvariantCultureIgnoreCase));
                if (index != -1)
                {
                    tempLst.Add(this.list[index++]);
                }
            }*/

            return this.GetInformationFromDictionary(firstName, this.firstNameDictionary).ToArray();
        }

        /// <summary>
        /// Searches all matches by lastName parameter.
        /// </summary>
        /// <param name="lastName">Person's last name.</param>
        /// <returns>All records with the same lastname.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            /*List<FileCabinetRecord> tempLst = new();
            int index = 0;

            while (index != -1)
            {
                index = this.list.FindIndex(index, this.list.Count - index, i => string.Equals(i.LastName, lastName, StringComparison.InvariantCultureIgnoreCase));
                if (index != -1)
                {
                    tempLst.Add(this.list[index++]);
                }
            }*/

            return this.GetInformationFromDictionary(lastName, this.lastNameDictionary).ToArray();
        }

        /// <summary>
        /// Searches all matches by birthDate parameter.
        /// </summary>
        /// <param name="birthDate">Person's date of birth.</param>
        /// <returns>All records with the same date of birth.</returns>
        public FileCabinetRecord[] FindByBirthDate(string birthDate)
        {
            /*List<FileCabinetRecord> tempLst = new ();

            bool isBirthDateVaild = DateTime.TryParse(birthDate, out DateTime birthDateTime);

            int index = 0;

            while (index != -1)
            {
                index = this.list.FindIndex(index, this.list.Count - index, i => DateTime.Compare(i.DateOfBirth, birthDateTime) == 0);
                if (index != -1)
                {
                    tempLst.Add(this.list[index++]);
                }
            }*/

            return this.GetInformationFromDictionary(birthDate, this.dateOfBirthDictionary).ToArray();
        }

        /// <summary>
        /// Validates input parameters.
        /// </summary>
        /// <param name="recordInputObject">Input parameters class.</param>
        /// <returns>Valid record.</returns>
        protected abstract FileCabinetRecord ValidateParameters(RecordInputObject recordInputObject);

        /// <summary>
        /// Abstract class for creating various validators.
        /// </summary>
        /// <returns>IRecordValidator.</returns>
        protected abstract IRecordValidator CreateValidator();

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
