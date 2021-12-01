using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Default Validator.
    /// </summary>
    public class DefaultValidator : IRecordValidator
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

            this.ValidateRecordBySpecificRules(record);

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
    }
}
