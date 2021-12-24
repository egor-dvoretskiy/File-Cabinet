using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Interface that hold validate parameters functional.
    /// </summary>
    public interface IRecordInputValidator
    {
        /// <summary>
        /// Validates firstName value.
        /// </summary>
        /// <param name="input">FirstName.</param>
        /// <returns>Tuple values (isValid, errorMessage).</returns>
        public Tuple<bool, string> FirstNameValidator(string input);

        /// <summary>
        /// Validates lastName value.
        /// </summary>
        /// <param name="input">LastName.</param>
        /// <returns>Tuple values (isValid, errorMessage).</returns>
        public Tuple<bool, string> LastNameValidator(string input);

        /// <summary>
        /// Validates birthDate value.
        /// </summary>
        /// <param name="input">Birth Date.</param>
        /// <returns>Tuple values (isValid, errorMessage).</returns>
        public Tuple<bool, string> DateOfBirthValidator(DateTime input);

        /// <summary>
        /// Validates personalRating value.
        /// </summary>
        /// <param name="input">personalRating.</param>
        /// <returns>Tuple values (isValid, errorMessage).</returns>
        public Tuple<bool, string> PersonalRatingValidator(short input);

        /// <summary>
        /// Validates debt value.
        /// </summary>
        /// <param name="input">debt.</param>
        /// <returns>Tuple values (isValid, errorMessage).</returns>
        public Tuple<bool, string> SalaryValidator(decimal input);

        /// <summary>
        /// Validates gender value.
        /// </summary>
        /// <param name="input">gender.</param>
        /// <returns>Tuple values (isValid, errorMessage).</returns>
        public Tuple<bool, string> GenderValidator(char input);
    }
}
