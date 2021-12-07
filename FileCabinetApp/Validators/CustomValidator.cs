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
        /// <summary>
        /// Validates firstName value.
        /// </summary>
        /// <param name="input">FirstName.</param>
        /// <returns>Tuple values (isValid, errorMessage).</returns>
        public Tuple<bool, string> FirstNameValidator(string input)
        {
            bool isValid = true;
            string errorMessage = string.Empty;

            if (input.Length < 2)
            {
                isValid = false;
                errorMessage = "FirstName's length is less than 2.";
            }
            else if (!Regex.IsMatch(input, @"^[a-zA-Z]+$"))
            {
                isValid = false;
                errorMessage = "FirstName has symbols that aren't letter.";
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

            if (input.Length < 2)
            {
                isValid = false;
                errorMessage = "LastName's length is less than 2.";
            }
            else if (!Regex.IsMatch(input, @"^[a-zA-Z]+$"))
            {
                isValid = false;
                errorMessage = "LastName has symbols that aren't letter.";
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

            DateTime leftDateLimit = new DateTime(1650, 1, 1);
            DateTime rightDateLimit = DateTime.Now;

            if (DateTime.Compare(input, leftDateLimit) < 0 || DateTime.Compare(input, rightDateLimit) > 0)
            {
                isValid = false;
                errorMessage = $"BirthDate is not into the interval [{leftDateLimit:yyyy-MMM-dd}, {rightDateLimit:yyyy-MMM-dd}].";
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

            if (input < 0)
            {
                errorMessage = $"PersonalRating value lesser than zero.";
            }

            return new Tuple<bool, string>(isValid, errorMessage);
        }

        /// <summary>
        /// Validates debt value.
        /// </summary>
        /// <param name="input">debt.</param>
        /// <returns>Tuple values (isValid, errorMessage).</returns>
        public Tuple<bool, string> DebtValidator(decimal input)
        {
            bool isValid = true;
            string errorMessage = string.Empty;

            if (input < 0)
            {
                errorMessage = $"Debt value is less than zero.";
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

            char[] availableGenderChars = new char[]
            {
                'M',
                'F',
                'T',
                'N',
                'O',
            };

            if (!availableGenderChars.Contains(input))
            {
                errorMessage = $"The world doesnt know about entered genter at this moment.";
            }

            return new Tuple<bool, string>(isValid, errorMessage);
        }
    }
}
