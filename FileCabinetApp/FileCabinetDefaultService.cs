using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Records processor class that inherits FileCabinetService with custom conditions.
    /// </summary>
    internal class FileCabinetDefaultService : FileCabinetService
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

            this.ValidateRecordBySpecificRules(record);

            return record;
        }
    }
}
