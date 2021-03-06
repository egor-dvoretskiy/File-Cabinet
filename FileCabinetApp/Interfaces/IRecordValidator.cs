using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Record validator's methods.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validate parameters.
        /// </summary>
        /// <param name="parameters">Record parameters.</param>
        /// <returns>Return validation result as boolean value.</returns>
        bool ValidateParameters(FileCabinetRecord parameters);
    }
}
