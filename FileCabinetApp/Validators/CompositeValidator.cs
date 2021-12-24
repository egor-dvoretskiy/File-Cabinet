using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Container in 'composite pattern'.
    /// </summary>
    public class CompositeValidator : IRecordValidator
    {
        private List<IRecordValidator> validators = new List<IRecordValidator>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">List of validators.</param>
        public CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            this.validators = validators.ToList();
        }

        /// <inheritdoc/>
        public bool ValidateParameters(FileCabinetRecord parameters)
        {
            bool isValid = true;

            foreach (var validator in this.validators)
            {
                isValid &= validator.ValidateParameters(parameters);
            }

            return isValid;
        }
    }
}
