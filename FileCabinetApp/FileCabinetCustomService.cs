using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Records processor class that inherits FileCabinetService with custom conditions.
    /// </summary>
    public class FileCabinetCustomService : FileCabinetService
    {
        /// <summary>
        /// Validates input parameters.
        /// </summary>
        /// <param name="recordInputObject">Input parameters class.</param>
        /// <returns>Valid record.</returns>
        protected override FileCabinetRecord ValidateParameters(RecordInputObject recordInputObject)
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

            this.CustomValidation(record);

            return record;
        }

        private void CustomValidation(FileCabinetRecord fileCabinetRecord)
        {
            if (!Regex.IsMatch(fileCabinetRecord.FirstName, @"^[a-zA-Z]+$"))
            {
                throw new ArgumentException($"{nameof(fileCabinetRecord.FirstName)} has symbols that aren't letter.");
            }

            if (!Regex.IsMatch(fileCabinetRecord.FirstName, @"^[a-zA-Z]+$"))
            {
                throw new ArgumentException($"{nameof(fileCabinetRecord.LastName)} has symbols that aren't letter.");
            }

            char[] availableGenderChars = new char[]
            {
                'M',
                'F',
                'T',
                'N',
                'O',
            };

            if (!availableGenderChars.Contains(fileCabinetRecord.Gender))
            {
                throw new ArgumentException($"{nameof(fileCabinetRecord.Gender)} has not defined gender.");
            }

            if (fileCabinetRecord.FirstName.Length < 2)
            {
                throw new ArgumentException($"{nameof(fileCabinetRecord.FirstName)}'s length is less than 2.");
            }

            if (fileCabinetRecord.LastName.Length < 2)
            {
                throw new ArgumentException($"{nameof(fileCabinetRecord.LastName)}'s length is less than 2.");
            }

            DateTime leftDateLimit = new DateTime(1650, 1, 1);
            DateTime rightDateLimit = DateTime.Now;

            if (DateTime.Compare(fileCabinetRecord.DateOfBirth, leftDateLimit) < 0 || DateTime.Compare(fileCabinetRecord.DateOfBirth, rightDateLimit) > 0)
            {
                throw new ArgumentException($"{nameof(fileCabinetRecord.DateOfBirth)} is not into the interval [{leftDateLimit:yyyy-MMM-dd}, {rightDateLimit:yyyy-MMM-dd}].");
            }

            if (fileCabinetRecord.PersonalRating < 0)
            {
                throw new ArgumentException($"{nameof(fileCabinetRecord.PersonalRating)} value lesser than 0.");
            }

            if (fileCabinetRecord.Debt < 0)
            {
                throw new ArgumentException($"{nameof(fileCabinetRecord.Debt)} value is less than zero.");
            }
        }
    }
}
