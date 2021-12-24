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
    internal class WeightValidator : IRecordValidator
    {
        private readonly decimal minimalWeight = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightValidator"/> class.
        /// </summary>
        /// <param name="min">Minimal value for record's weight.</param>
        public WeightValidator(decimal min)
        {
            this.minimalWeight = min;
        }

        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecord parameters)
        {
            var input = parameters.Debt;

            if (input < this.minimalWeight)
            {
                throw new ArgumentOutOfRangeException($"Weight is less than minimal value ({this.minimalWeight}).");
            }
        }
    }
}
