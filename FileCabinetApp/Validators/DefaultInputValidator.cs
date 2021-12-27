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
    public class DefaultInputValidator : IRecordInputValidator
    {
        /// <summary>
        /// Validates firstName value.
        /// </summary>
        /// <param name="input">FirstName.</param>
        /// <returns>Tuple values (isValid, errorMessage).</returns>
        public Tuple<bool, string> FirstNameValidator(string input)
        {
            bool isValid = true;
            string errorMessage = string.Empty;

            if (input.Length < ValidatorSettings.MinimalLengthFirstName || input.Length > ValidatorSettings.MaximumLengthFirstName)
            {
                isValid = false;
                errorMessage = $"FirstName's length is not in the interval [{ValidatorSettings.MinimalLengthFirstName}; {ValidatorSettings.MaximumLengthFirstName}].";
            }

            return new Tuple<bool, string>(isValid, errorMessage);
        }

        /// <summary>
        /// Validates lastName value.
        /// </summary>
        /// <param name="input">LastName.</param>
        /// <returns>Tuple values (isValid, errorMessage).</returns>
        public Tuple<bool, string> LastNameValidator(string input)
        {
            bool isValid = true;
            string errorMessage = string.Empty;

            if (input.Length < ValidatorSettings.MinimalLengthLastName || input.Length > ValidatorSettings.MaximumLengthLastName)
            {
                isValid = false;
                errorMessage = $"LastName's length is not in the interval [{ValidatorSettings.MinimalLengthLastName}; {ValidatorSettings.MaximumLengthLastName}].";
            }

            return new Tuple<bool, string>(isValid, errorMessage);
        }

        /// <summary>
        /// Validates birthDate value.
        /// </summary>
        /// <param name="input">Birth Date.</param>
        /// <returns>Tuple values (isValid, errorMessage).</returns>
        public Tuple<bool, string> DateOfBirthValidator(DateTime input)
        {
            bool isValid = true;
            string errorMessage = string.Empty;

            if (DateTime.Compare(input, ValidatorSettings.MinimalDate) < 0 || DateTime.Compare(input, ValidatorSettings.MaximumDate) > 0)
            {
                isValid = false;
                errorMessage = $"BirthDate is not into the interval [{ValidatorSettings.MinimalDate:yyyy-MMM-dd}, {ValidatorSettings.MaximumDate:yyyy-MMM-dd}].";
            }

            return new Tuple<bool, string>(isValid, errorMessage);
        }

        /// <summary>
        /// Validates personalRating value.
        /// </summary>
        /// <param name="input">personalRating.</param>
        /// <returns>Tuple values (isValid, errorMessage).</returns>
        public Tuple<bool, string> PersonalRatingValidator(short input)
        {
            bool isValid = true;
            string errorMessage = string.Empty;

            if (input < ValidatorSettings.MinimalPersonalRating)
            {
                isValid = false;
                errorMessage = $"Personal rating value is less than {ValidatorSettings.MinimalSalary}.";
            }

            return new Tuple<bool, string>(isValid, errorMessage);
        }

        /// <summary>
        /// Validates debt value.
        /// </summary>
        /// <param name="input">debt.</param>
        /// <returns>Tuple values (isValid, errorMessage).</returns>
        public Tuple<bool, string> SalaryValidator(decimal input)
        {
            bool isValid = true;
            string errorMessage = string.Empty;

            if (input < ValidatorSettings.MinimalSalary)
            {
                isValid = false;
                errorMessage = $"Salary value is less than {ValidatorSettings.MinimalSalary}.";
            }

            return new Tuple<bool, string>(isValid, errorMessage);
        }

        /// <summary>
        /// Validates gender value.
        /// </summary>
        /// <param name="input">gender.</param>
        /// <returns>Tuple values (isValid, errorMessage).</returns>
        public Tuple<bool, string> GenderValidator(char input)
        {
            bool isValid = true;
            string errorMessage = string.Empty;

            if (!char.IsLetter(input))
            {
                isValid = false;
                errorMessage = $"Gender value is not a letter.";
            }

            return new Tuple<bool, string>(isValid, errorMessage);
        }
    }
}
