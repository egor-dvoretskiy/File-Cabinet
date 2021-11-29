using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Records Processor Class.
    /// </summary>
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();
        private Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        /// <summary>
        /// Creates record and adds to main list.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <param name="lastName">Last name.</param>
        /// <param name="dateOfBirth">Birth date.</param>
        /// <param name="personalRating">Personal rating.</param>
        /// <param name="debtString">Debt.</param>
        /// <param name="genderString">Gender.</param>
        /// <returns>Record's id in list.</returns>
        public int CreateRecord(
            string firstName,
            string lastName,
            string dateOfBirth,
            string personalRating,
            string debtString,
            string genderString)
        {
            try
            {
                var parametersTuple = this.ValidateArguments(firstName, lastName, dateOfBirth, personalRating, debtString, genderString);

                var record = new FileCabinetRecord
                {
                    Id = this.list.Count + 1,
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = parametersTuple.Item1,
                    PersonalRating = parametersTuple.Item2,
                    Debt = parametersTuple.Item3,
                    Gender = parametersTuple.Item4,
                };

                this.list.Add(record);

                this.AddInformationToDictionary(firstName, ref this.firstNameDictionary, record);
                this.AddInformationToDictionary(lastName, ref this.lastNameDictionary, record);
                this.AddInformationToDictionary(dateOfBirth, ref this.dateOfBirthDictionary, record);

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

            Console.WriteLine("Please, try again:");

            return -1;
        }

        /// <summary>
        /// Edit record in list.
        /// </summary>
        /// <param name="id">Record's id in list.</param>
        /// <param name="firstName">First name.</param>
        /// <param name="lastName">Last name.</param>
        /// <param name="dateOfBirth">Birth date.</param>
        /// <param name="personalRating">Personal rating.</param>
        /// <param name="debtString">Debt.</param>
        /// <param name="genderString">Gender.</param>
        /// <exception cref="ArgumentException">id.</exception>
        public void EditRecord(
            int id,
            string firstName,
            string lastName,
            string dateOfBirth,
            string personalRating,
            string debtString,
            string genderString)
        {
            if (id == -1)
            {
                throw new ArgumentException($"{id}");
            }

            try
            {
                var parametersTuple = this.ValidateArguments(firstName, lastName, dateOfBirth, personalRating, debtString, genderString);

                this.list[id].FirstName = firstName;
                this.list[id].LastName = lastName;
                this.list[id].DateOfBirth = parametersTuple.Item1;
                this.list[id].PersonalRating = parametersTuple.Item2;
                this.list[id].Debt = parametersTuple.Item3;
                this.list[id].Gender = parametersTuple.Item4;

                this.EditInformationInDictionary(firstName, ref this.firstNameDictionary, this.list[id]);
                this.EditInformationInDictionary(lastName, ref this.lastNameDictionary, this.list[id]);
                this.EditInformationInDictionary(dateOfBirth, ref this.dateOfBirthDictionary, this.list[id]);
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
            if (dict.ContainsKey(parameterName))
            {
                for (int i = 0; i < dict[parameterName].Count; i++)
                {
                    if (dict[parameterName][i].Id == record.Id)
                    {
                        dict[parameterName][i] = record;
                        return;
                    }
                }
            }
        }

        private List<FileCabinetRecord> GetInformationFromDictionary(string parametrName, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            if (dictionary.ContainsKey(parametrName))
            {
                return dictionary[parametrName];
            }

            return new List<FileCabinetRecord>();
        }

        private Tuple<DateTime, short, decimal, char> ValidateArguments(
            string firstName,
            string lastName,
            string dateOfBirth,
            string personalRatingString,
            string debtString,
            string genderString)
        {
            this.NullCheckRecordValues(
                firstName,
                lastName,
                dateOfBirth,
                personalRatingString,
                debtString,
                genderString);

            var isBirthDateValid = DateTime.TryParse(dateOfBirth, out DateTime birthDate);
            var isPersonalRatingValid = short.TryParse(personalRatingString, out short personalRating);
            var isDebtValid = decimal.TryParse(debtString, out decimal debt);
            var isGenderValid = char.TryParse(genderString, out char gender);

            this.ValidateParsingRecordValues(
                isBirthDateValid,
                isPersonalRatingValid,
                isDebtValid,
                isGenderValid);

            this.ValidateRecordBySpecificRules(
                firstName,
                lastName,
                birthDate,
                personalRating,
                debt,
                gender);

            return new Tuple<DateTime, short, decimal, char>(birthDate, personalRating, debt, gender);
        }

        private void NullCheckRecordValues(
            string firstName,
            string lastName,
            string dateOfBirth,
            string personalRatingString,
            string debtString,
            string genderString)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            if (string.IsNullOrWhiteSpace(dateOfBirth))
            {
                throw new ArgumentNullException(nameof(dateOfBirth));
            }

            if (string.IsNullOrWhiteSpace(personalRatingString))
            {
                throw new ArgumentNullException(nameof(personalRatingString));
            }

            if (string.IsNullOrWhiteSpace(debtString))
            {
                throw new ArgumentNullException(nameof(debtString));
            }

            if (string.IsNullOrWhiteSpace(genderString))
            {
                throw new ArgumentNullException(nameof(genderString));
            }
        }

        private void ValidateParsingRecordValues(
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

        private void ValidateRecordBySpecificRules(
            string firstName,
            string lastName,
            DateTime birthDate,
            short personalRating,
            decimal debt,
            char gender)
        {
            if (firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException($"{nameof(firstName)}'s length is not in the interval [2; 60].");
            }

            if (lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException($"{nameof(lastName)}'s length is not in the interval [2; 60].");
            }

            if (personalRating < -12)
            {
                throw new ArgumentException($"{nameof(personalRating)} value lesser than -12.");
            }

            if (debt < 0)
            {
                throw new ArgumentException($"{nameof(debt)} value is less than zero.");
            }

            if (!char.IsLetter(gender))
            {
                throw new ArgumentException($"{nameof(gender)} is not a letter.");
            }

            DateTime leftDateLimit = new DateTime(1950, 1, 1);
            DateTime rightDateLimit = DateTime.Now;

            if (DateTime.Compare(birthDate, leftDateLimit) < 0 || DateTime.Compare(birthDate, rightDateLimit) > 0)
            {
                throw new ArgumentException($"{nameof(birthDate)} is not into the interval [{leftDateLimit:yyyy-MMM-dd}, {rightDateLimit:yyyy-MMM-dd}].");
            }
        }
    }
}
