using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Validator with specific rules.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        /// <inheritdoc/>
        public FileCabinetRecord ValidateParameters(RecordInputObject recordInputObject)
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
