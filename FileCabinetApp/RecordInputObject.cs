using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Contains input parameter object for create and edit methods.
    /// </summary>
    public class RecordInputObject
    {
        /// <summary>
        /// Gets or sets person's first name.
        /// </summary>
        /// <value>
        /// Person's first name.
        /// </value>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets person's last name.
        /// </summary>
        /// <value>
        /// Person's last name.
        /// </value>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets person's birth date.
        /// </summary>
        /// <value>
        /// Person's birth date.
        /// </value>
        public string DateOfBirth { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets person's personal rating.
        /// </summary>
        /// <value>
        /// Person's personal rating.
        /// </value>
        public string PersonalRating { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets person's amount of debt.
        /// </summary>
        /// <value>
        /// Person's amount of debt.
        /// </value>
        public string Debt { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets person's gender.
        /// </summary>
        /// <value>
        /// Person's gender.
        /// </value>
        public string Gender { get; set; } = string.Empty;
    }
}
