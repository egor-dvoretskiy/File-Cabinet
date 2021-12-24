using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Validates first name.
    /// </summary>
    public class FirstNameValidator : IRecordValidator
    {
        private readonly int minLength = 2;
        private readonly int maxLength = 60;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
        /// </summary>
        /// <param name="min">Minimal length of first name.</param>
        /// <param name="max">Maximum length of first name.</param>
        public FirstNameValidator(int min, int max)
        {
            this.minLength = min;
            this.maxLength = max;
        }

        /// <inheritdoc/>
        public bool ValidateParameters(FileCabinetRecord parameters)
        {
            bool isValid = true;
            var input = parameters.FirstName;

            if (input.Length < this.minLength || input.Length > this.maxLength)
            {
                isValid = false; // throw new ArgumentOutOfRangeException("FirstName's length is not in the interval [2; 60].");
            }

            return isValid;
        }
    }
}
