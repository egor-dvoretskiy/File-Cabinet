using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();
        private Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        public int CreateRecord(
            string firstName,
            string lastName,
            string dateOfBirth,
            string personalRating,
            string moneyAccountString,
            string genderString)
        {
            try
            {
                var parametersTuple = this.ValidateArguments(firstName, lastName, dateOfBirth, personalRating, moneyAccountString, genderString);

                var record = new FileCabinetRecord
                {
                    Id = this.list.Count + 1,
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = parametersTuple.Item1,
                    PersonalRating = parametersTuple.Item2,
                    MoneyAccount = parametersTuple.Item3,
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

        public void EditRecord(
            int id,
            string firstName,
            string lastName,
            string dateOfBirth,
            string personalRatingString,
            string moneyAccountString,
            string genderString)
        {
            if (id == -1)
            {
                throw new ArgumentException($"{id}");
            }

            try
            {
                var parametersTuple = this.ValidateArguments(firstName, lastName, dateOfBirth, personalRatingString, moneyAccountString, genderString);

                this.list[id].FirstName = firstName;
                this.list[id].LastName = lastName;
                this.list[id].DateOfBirth = parametersTuple.Item1;
                this.list[id].PersonalRating = parametersTuple.Item2;
                this.list[id].MoneyAccount = parametersTuple.Item3;
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

        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        public int GetStat()
        {
            return this.list.Count;
        }

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
            string moneyAccountString,
            string genderString)
        {
            this.NullCheckRecordValues(
                firstName,
                lastName,
                dateOfBirth,
                personalRatingString,
                moneyAccountString,
                genderString);

            var isBirthDateValid = DateTime.TryParse(dateOfBirth, out DateTime birthDate);
            var isPersonalRatingValid = short.TryParse(personalRatingString, out short personalRating);
            var isAmountOfMoneyValid = decimal.TryParse(moneyAccountString, out decimal amountOfMoney);
            var isGenderValid = char.TryParse(genderString, out char gender);

            this.ValidateParsingRecordValues(
                isBirthDateValid,
                isPersonalRatingValid,
                isAmountOfMoneyValid,
                isGenderValid);

            this.ValidateRecordBySpecificRules(
                firstName,
                lastName,
                birthDate,
                personalRating,
                amountOfMoney,
                gender);

            return new Tuple<DateTime, short, decimal, char>(birthDate, personalRating, amountOfMoney, gender);
        }

        private void NullCheckRecordValues(
            string firstName,
            string lastName,
            string dateOfBirth,
            string personalRatingString,
            string moneyAccountString,
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

            if (string.IsNullOrWhiteSpace(moneyAccountString))
            {
                throw new ArgumentNullException(nameof(moneyAccountString));
            }

            if (string.IsNullOrWhiteSpace(genderString))
            {
                throw new ArgumentNullException(nameof(genderString));
            }
        }

        private void ValidateParsingRecordValues(
            bool isBirthDateValid,
            bool isPersonalRatingValid,
            bool isAmountOfMoneyValid,
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

            if (!isAmountOfMoneyValid)
            {
                throw new ArgumentException($"Cannot parse _amountOfMoney_.");
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
            decimal amountOfMoney,
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

            if (amountOfMoney < 0)
            {
                throw new ArgumentException($"{nameof(amountOfMoney)} value is less than zero.");
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
