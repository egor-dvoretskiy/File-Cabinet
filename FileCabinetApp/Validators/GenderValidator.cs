using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Validates record's gender.
    /// </summary>
    public class GenderValidator : IRecordValidator
    {
        private readonly char[] genderChars = { 'M', 'F' };

        /// <summary>
        /// Initializes a new instance of the <see cref="GenderValidator"/> class.
        /// </summary>
        /// <param name="possibleGenderChars">All possible chars for gender value.</param>
        public GenderValidator(char[] possibleGenderChars)
        {
            this.genderChars = possibleGenderChars;
        }

        /// <inheritdoc/>
        public bool ValidateParameters(FileCabinetRecord parameters)
        {
            bool isValid = true;
            var input = parameters.Gender;

            if (!this.genderChars.Contains(input))
            {
                isValid = false; // throw new ArgumentException($"Gender possible chars dont contains input gender value ({input}).");
            }

            return isValid;
        }
    }
}
