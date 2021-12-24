using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Validates record's weight.
    /// </summary>
    internal class SalaryValidator : IRecordValidator
    {
        private readonly decimal minimalSalary = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalaryValidator"/> class.
        /// </summary>
        /// <param name="min">Minimal value for record's weight.</param>
        public SalaryValidator(decimal min)
        {
            this.minimalSalary = min;
        }

        /// <inheritdoc/>
        public bool ValidateParameters(FileCabinetRecord parameters)
        {
            bool isValid = true;
            var input = parameters.Salary;

            if (input < this.minimalSalary)
            {
                isValid = false; // throw new ArgumentOutOfRangeException($"Salary is less than minimal value ({this.minimalSalary}).");
            }

            return isValid;
        }
    }
}
