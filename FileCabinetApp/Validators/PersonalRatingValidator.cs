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
    public class PersonalRatingValidator : IRecordValidator
    {
        private readonly short minimalPersonalRating = 0; // in cm

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonalRatingValidator"/> class.
        /// </summary>
        /// <param name="min">Minimal possible height.</param>
        public PersonalRatingValidator(short min)
        {
            this.minimalPersonalRating = min;
        }

        /// <inheritdoc/>
        public bool ValidateParameters(FileCabinetRecord parameters)
        {
            bool isValid = true;
            var input = parameters.PersonalRating;

            if (input < this.minimalPersonalRating)
            {
                isValid = false; // throw new ArgumentOutOfRangeException($"Pesonal rating is less than minimal value ({this.minimalPersonalRating}).");
            }

            return isValid;
        }
    }
}
