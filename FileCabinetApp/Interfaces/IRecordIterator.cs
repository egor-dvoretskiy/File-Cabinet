using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Applied to 'iterator' pattern.
    /// </summary>
    public interface IRecordIterator
    {
        /// <summary>
        /// Moves iterator to next element.
        /// </summary>
        /// <returns>Returns next record.</returns>
        FileCabinetRecord GetNext();

        /// <summary>
        /// Checks if there are more records.
        /// </summary>
        /// <returns>Returns availability of records.</returns>
        bool HasMore();
    }
}
