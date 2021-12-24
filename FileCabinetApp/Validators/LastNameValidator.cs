using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Validates last name.
    /// </summary>
    public class LastNameValidator : IRecordValidator
    {
        private readonly int minLength = 2;
        private readonly int maxLength = 60;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameValidator"/> class.
        /// </summary>
        /// <param name="min">Minimal length of first name.</param>
        /// <param name="max">Maximum length of first name.</param>
        public LastNameValidator(int min, int max)
        {
            this.minLength = min;
            this.maxLength = max;
        }

        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecord parameters)
        {
            var input = parameters.LastName;

            if (input.Length < this.minLength || input.Length > this.maxLength)
            {
                throw new ArgumentOutOfRangeException("LastName's length is not in the interval [2; 60].");
            }
        }
    }
}
