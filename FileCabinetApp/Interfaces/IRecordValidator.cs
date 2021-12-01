using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Interface that hold validate parameters functional.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validates input parameters.
        /// </summary>
        /// <param name="recordInputObject">Input parameters class.</param>
        /// <returns>Valid record.</returns>
        public FileCabinetRecord ValidateParameters(RecordInputObject recordInputObject);
    }
}
