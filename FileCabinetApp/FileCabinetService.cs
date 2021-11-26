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
            string jailTimesString,
            string moneyAccountString,
            string genderString)
        {
            try
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
            string jailTimesString,
            string moneyAccountString,
            string genderString)
        {
            if (id == -1)
            {
                throw new ArgumentException($"{id}");
            }

            try
            {
                var parametersTuple = this.ValidateArguments(firstName, lastName, dateOfBirth, jailTimesString, moneyAccountString, genderString);

                this.list[id].FirstName = firstName;
                this.list[id].LastName = lastName;
                this.list[id].DateOfBirth = parametersTuple.Item1;
                this.list[id].TimesInJail = parametersTuple.Item2;
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

        private Tuple<DateTime, short, decimal, char> ValidateArguments(string firstName, string lastName, string dateOfBirth, string jailTimesString, string moneyAccountString, string genderString)
        {
            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(dateOfBirth) ||
                string.IsNullOrWhiteSpace(jailTimesString) ||
                string.IsNullOrWhiteSpace(moneyAccountString) ||
                string.IsNullOrWhiteSpace(genderString))
            {
                throw new ArgumentNullException("Something has null reference");
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
