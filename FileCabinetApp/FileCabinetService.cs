using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();

        public int CreateRecord(string firstName, string lastName, string dateOfBirth, string jailTimesString, string moneyAccountString, string genderString)
        {
            var parametersTuple = this.ValidateArguments(firstName, lastName, dateOfBirth, jailTimesString, moneyAccountString, genderString);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = parametersTuple.Item1,
                TimesInJail = parametersTuple.Item2,
                MoneyAccount = parametersTuple.Item3,
                Gender = parametersTuple.Item4,
            };

            this.list.Add(record);

            return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        public int GetStat()
        {
            return this.list.Count;
        }

        private Tuple<DateTime, short, decimal, char> ValidateArguments(string firstName, string lastName, string dateOfBirth, string jailTimesString, string moneyAccountString, string genderString)
        {
            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(dateOfBirth) ||
                string.IsNullOrWhiteSpace(jailTimesString) ||
                string.IsNullOrWhiteSpace(moneyAccountString) ||
                string.IsNullOrWhiteSpace(genderString))
            {
                throw new ArgumentNullException(firstName);
            }

            var birthDateValid = DateTime.TryParse(dateOfBirth, out DateTime birthDate);
            var jailTimesValid = short.TryParse(jailTimesString, out short jailTimes);
            var moneyAccountValid = decimal.TryParse(moneyAccountString, out decimal moneyAccount);
            var genderValid = char.TryParse(genderString, out char gender);

            if (!birthDateValid ||
                !jailTimesValid ||
                !moneyAccountValid ||
                !genderValid)
            {
                throw new ArgumentException("First block of Argument Check.");
            }

            var firstDate = new DateTime(1950, 1, 1);
            var lastDate = DateTime.Now;

            if (firstName.Length < 2 || firstName.Length > 60 ||
                lastName.Length < 2 || lastName.Length > 60 ||
                jailTimes < 0 ||
                moneyAccount < 0 ||
                !char.IsLetter(gender) ||
                DateTime.Compare(birthDate, firstDate) < 0 || DateTime.Compare(birthDate, lastDate) > 0)
            {
                throw new ArgumentException("Second block of Argument Check.");
            }

            return new Tuple<DateTime, short, decimal, char>(birthDate, jailTimes, moneyAccount, gender);
        }
    }
}
