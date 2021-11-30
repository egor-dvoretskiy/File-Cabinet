using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
                var validatedRecord = this.ValidateParameters(recordInputObject);

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
                var record = this.ValidateParameters(recordInputObject);

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
        /// Check if records has null values.
        /// </summary>
        /// <param name="recordInputObject">Record.</param>
        /// <exception cref="ArgumentNullException">One of record parameters.</exception>
        protected void NullCheckRecordValues(RecordInputObject recordInputObject)
        {
            if (string.IsNullOrWhiteSpace(recordInputObject.FirstName))
            {
                throw new ArgumentNullException(nameof(recordInputObject.FirstName));
            }

            if (string.IsNullOrWhiteSpace(recordInputObject.LastName))
            {
                throw new ArgumentNullException(nameof(recordInputObject.LastName));
            }

            if (string.IsNullOrWhiteSpace(recordInputObject.DateOfBirth))
            {
                throw new ArgumentNullException(nameof(recordInputObject.DateOfBirth));
            }

            if (string.IsNullOrWhiteSpace(recordInputObject.PersonalRating))
            {
                throw new ArgumentNullException(nameof(recordInputObject.PersonalRating));
            }

            if (string.IsNullOrWhiteSpace(recordInputObject.Debt))
            {
                throw new ArgumentNullException(nameof(recordInputObject.Debt));
            }

            if (string.IsNullOrWhiteSpace(recordInputObject.Gender))
            {
                throw new ArgumentNullException(nameof(recordInputObject.Debt));
            }
        }

        /// <summary>
        /// Checks if parsing parameters goes right.
        /// </summary>
        /// <param name="isBirthDateValid">Status of parsed birthDate.</param>
        /// <param name="isPersonalRatingValid">Status of parsed personal rating.</param>
        /// <param name="isDebtValid">Status of parsed debt.</param>
        /// <param name="isGenderValid">Status of parsed gender.</param>
        /// <exception cref="ArgumentException">One of status values.</exception>
        protected void ValidateParsingRecordValues(
            bool isBirthDateValid,
            bool isPersonalRatingValid,
            bool isDebtValid,
            bool isGenderValid)
        {
            if (!isBirthDateValid)
            {
                throw new ArgumentException($"Cannot parse _birthDate_.");
            }

            if (!isPersonalRatingValid)
            {
                throw new ArgumentException($"Cannot parse _personalRating_.");
            }

            if (!isDebtValid)
            {
                throw new ArgumentException($"Cannot parse _indebtness_.");
            }

            if (!isGenderValid)
            {
                throw new ArgumentException($"Cannot parse _gender_.");
            }
        }

        /// <summary>
        /// Validates record using specific rules.
        /// </summary>
        /// <param name="fileCabinetRecord">Record.</param>
        /// <exception cref="ArgumentException">One of record's parameters.</exception>
        protected void ValidateRecordBySpecificRules(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord.FirstName.Length < 2 || fileCabinetRecord.FirstName.Length > 60)
            {
                throw new ArgumentException($"{nameof(fileCabinetRecord.FirstName)}'s length is not in the interval [2; 60].");
            }

            if (fileCabinetRecord.LastName.Length < 2 || fileCabinetRecord.LastName.Length > 60)
            {
                throw new ArgumentException($"{nameof(fileCabinetRecord.LastName)}'s length is not in the interval [2; 60].");
            }

            DateTime leftDateLimit = new DateTime(1950, 1, 1);
            DateTime rightDateLimit = DateTime.Now;

            if (DateTime.Compare(fileCabinetRecord.DateOfBirth, leftDateLimit) < 0 || DateTime.Compare(fileCabinetRecord.DateOfBirth, rightDateLimit) > 0)
            {
                throw new ArgumentException($"{nameof(fileCabinetRecord.DateOfBirth)} is not into the interval [{leftDateLimit:yyyy-MMM-dd}, {rightDateLimit:yyyy-MMM-dd}].");
            }

            if (fileCabinetRecord.PersonalRating < -12)
            {
                throw new ArgumentException($"{nameof(fileCabinetRecord.PersonalRating)} value lesser than -12.");
            }

            if (fileCabinetRecord.Debt < 0)
            {
                throw new ArgumentException($"{nameof(fileCabinetRecord.Debt)} value is less than zero.");
            }

            if (!char.IsLetter(fileCabinetRecord.Gender))
            {
                throw new ArgumentException($"{nameof(fileCabinetRecord.Gender)} is not a letter.");
            }
        }

        /*/// <summary>
        /// Validates input parameters
        /// </summary>
        /// <param name="recordInputObject">Input parameters class.</param>
        /// <returns>Valid record.</returns>
        protected FileCabinetRecord ValidateParameters(RecordInputObject recordInputObject)
        {
            this.NullCheckRecordValues(recordInputObject);

            bool isBirthDateValid = DateTime.TryParse(recordInputObject.DateOfBirth, out DateTime birthDate);
            bool isPersonalRatingValid = short.TryParse(recordInputObject.PersonalRating, out short personalRating);
            bool isDebtValid = decimal.TryParse(recordInputObject.Debt, out decimal debt);
            bool isGenderValid = char.TryParse(recordInputObject.Gender, out char gender);

            this.ValidateParsingRecordValues(
                isBirthDateValid,
                isPersonalRatingValid,
                isDebtValid,
                isGenderValid);

            FileCabinetRecord record = new FileCabinetRecord()
            {
                FirstName = recordInputObject.FirstName,
                LastName = recordInputObject.LastName,
                DateOfBirth = birthDate,
                PersonalRating = personalRating,
                Debt = debt,
                Gender = gender,
            };

            this.ValidateRecordBySpecificRules(record);

            return record;
        }*/

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
