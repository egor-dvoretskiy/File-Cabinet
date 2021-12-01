using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// Records processor class that inherits FileCabinetService with custom conditions.
    /// </summary>
    public class FileCabinetCustomService : FileCabinetService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetCustomService"/> class.
        /// </summary>
        /// <param name="validator">IRecordValidator.</param>
        public FileCabinetCustomService(IRecordValidator validator)
            : base(validator)
        {
        }

        /// <summary>
        /// Validates input parameters.
        /// </summary>
        /// <param name="recordInputObject">Input parameters class.</param>
        /// <returns>Valid record.</returns>
        protected override FileCabinetRecord ValidateParameters(RecordInputObject recordInputObject)
        {
            return new FileCabinetRecord();
        }

        /// <summary>
        /// Creates default validator.
        /// </summary>
        /// <returns>Interface that realises validate parameters.</returns>
        protected override IRecordValidator CreateValidator() => new CustomValidator();
    }
}
