using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Validates record's height.
    /// </summary>
    public class HeightValidator : IRecordValidator
    {
        private readonly short minimalHeight = 0; // in cm

        /// <summary>
        /// Initializes a new instance of the <see cref="HeightValidator"/> class.
        /// </summary>
        /// <param name="min">Minimal possible height.</param>
        public HeightValidator(short min)
        {
            this.minimalHeight = min;
        }

        /// <inheritdoc/>
        public void ValidateParameters(FileCabinetRecord parameters)
        {
            var input = parameters.PersonalRating;

            if (input < this.minimalHeight)
            {
                throw new ArgumentOutOfRangeException($"Height is less than minimal value ({this.minimalHeight}).");
            }
        }
    }
}
