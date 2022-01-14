using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Class, that contains record parameters.
    /// </summary>
    public class FileCabinetRecord
    {
        /// <summary>
        /// Gets or sets unique idetifier of record.
        /// </summary>
        /// <value>
        /// Unique idetifier of record.
        /// </value>
        public int Id { get; set; }

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
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets person's personal rating.
        /// </summary>
        /// <value>
        /// Person's personal rating.
        /// </value>
        public short PersonalRating { get; set; }

        /// <summary>
        /// Gets or sets person's salary.
        /// </summary>
        /// <value>
        /// Person's amount of debt.
        /// </value>
        public decimal Salary { get; set; }

        /// <summary>
        /// Gets or sets person's gender.
        /// </summary>
        /// <value>
        /// Person's gender.
        /// </value>
        public char Gender { get; set; }
    }
}
