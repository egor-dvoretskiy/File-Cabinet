using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Validates date of birth.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator
    {
        private readonly DateTime minimalDate = new DateTime(1950, 1, 1);
        private readonly DateTime maximumDate = DateTime.Now;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="from">Minimal acceptable to valid date.</param>
        /// <param name="to">Maximum acceptable to valid date.</param>
        public DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.minimalDate = from;
            this.maximumDate = to;
        }

        /// <inheritdoc/>
        public bool ValidateParameters(FileCabinetRecord parameters)
        {
            bool isValid = true;
            var input = parameters.DateOfBirth;

            if (DateTime.Compare(input, this.minimalDate) < 0 || DateTime.Compare(input, this.maximumDate) > 0)
            {
                isValid = false; // throw new ArgumentOutOfRangeException($"Birth date is not into the interval [{this.minimalDate:yyyy-MMM-dd}, {this.maximumDate:yyyy-MMM-dd}].");
            }

            return isValid;
        }
    }
}
