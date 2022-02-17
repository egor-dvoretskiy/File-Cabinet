using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.ServiceTools
{
    /// <summary>
    /// Compare fields by specific rules.
    /// </summary>
    public static class CustomComparer
    {
        /// <summary>
        /// Compares dates.
        /// </summary>
        /// <param name="d1">Date 1 to compare.</param>
        /// <param name="d2">Date 2 to compare.</param>
        /// <returns>Returns result of comparing two dates.</returns>
        public static bool IsEqualDatesUpToDays(DateTime d1, DateTime d2)
        {
            return d1.Year == d2.Year && d1.Month == d2.Month && d1.Day == d2.Day;
        }
    }
}
