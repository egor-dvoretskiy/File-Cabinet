using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Class contains custom validation settings.
    /// </summary>
    internal static class ValidatorSettingsCustom
    {
        /// <summary>
         /// Minimal possible length of first name.
         /// </summary>
        public static readonly int MinimalLengthFirstName = 2;

        /// <summary>
        /// Maximum possible length of first name.
        /// </summary>
        public static readonly int MaximumLengthFirstName = 60;

        /// <summary>
        /// Minimal possible length of last name.
        /// </summary>
        public static readonly int MinimalLengthLastName = 2;

        /// <summary>
        /// Maximum possible length of last name.
        /// </summary>
        public static readonly int MaximumLengthLastName = 60;

        /// <summary>
        /// Minimal possible birth date.
        /// </summary>
        public static readonly DateTime MinimalDate = DateTime.MinValue;

        /// <summary>
        /// Maximum possible birth date.
        /// </summary>
        public static readonly DateTime MaximumDate = DateTime.Now;

        /// <summary>
        /// Minimal possible personal rating.
        /// </summary>
        public static readonly short MinimalPersonalRating = -1250;

        /// <summary>
        /// Minimal possible salary.
        /// </summary>
        public static readonly decimal MinimalSalary = 100000;

        /// <summary>
        /// Existing genders.
        /// </summary>
        public static readonly char[] GenderChars = new char[]
        {
                'M',
                'F',
                'T',
                'N',
                'O',
        };
    }
}
