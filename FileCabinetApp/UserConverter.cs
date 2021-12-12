﻿using System;
using System.Text.RegularExpressions;

namespace FileCabinetApp
{
    /// <summary>
    /// Contains various converters.
    /// </summary>
    public static class UserConverter
    {
        /// <summary>
        /// Converts input string to string.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Tuple values (isValid, errorMessage, resultOfConverting).</returns>
        public static Tuple<bool, string, string> StringConverter(string input)
        {
            bool isConvertingSuccessful = true;
            string errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                isConvertingSuccessful = false;
                errorMessage = "Input string Is Null Or White Space";
            }

            return new Tuple<bool, string, string>(isConvertingSuccessful, errorMessage, input);
        }

        /// <summary>
        /// Converts input string to DateTime.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Tuple values (isValid, errorMessage, resultOfConverting).</returns>
        public static Tuple<bool, string, DateTime> BirthDateConverter(string input)
        {
            bool isConvertingSuccessful = true;
            string errorMessage = string.Empty;
            DateTime birthDate = DateTime.Now;

            if (string.IsNullOrWhiteSpace(input))
            {
                isConvertingSuccessful = false;
                errorMessage = "BirthDate Is Null Or White Space";
            }
            else
            {
                isConvertingSuccessful = DateTime.TryParse(input, out birthDate);

                if (!isConvertingSuccessful)
                {
                    errorMessage = "Cannot parse BirthDate(DateTime)";
                }
            }

            return new Tuple<bool, string, DateTime>(isConvertingSuccessful, errorMessage, birthDate);
        }

        /// <summary>
        /// Converts input string to short.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Tuple values (isValid, errorMessage, resultOfConverting).</returns>
        public static Tuple<bool, string, short> PersonalRatingConverter(string input)
        {
            bool isConvertingSuccessful = true;
            string errorMessage = string.Empty;
            short personalRating = 0;

            if (string.IsNullOrWhiteSpace(input))
            {
                isConvertingSuccessful = false;
                errorMessage = "personalRating Is Null Or White Space";
            }
            else
            {
                isConvertingSuccessful = short.TryParse(input, out personalRating);
                if (!isConvertingSuccessful)
                {
                    errorMessage = "Cannot parse personalRating(short)";
                }
            }

            return new Tuple<bool, string, short>(isConvertingSuccessful, errorMessage, personalRating);
        }

        /// <summary>
        /// Converts input string to decimal.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Tuple values (isValid, errorMessage, resultOfConverting).</returns>
        public static Tuple<bool, string, decimal> DebtConverter(string input)
        {
            bool isConvertingSuccessful = true;
            string errorMessage = string.Empty;
            decimal debt = 0;

            if (string.IsNullOrWhiteSpace(input))
            {
                isConvertingSuccessful = false;
                errorMessage = "debt Is Null Or White Space";
            }
            else
            {
                isConvertingSuccessful = decimal.TryParse(input, out debt);
                if (!isConvertingSuccessful)
                {
                    errorMessage = "Cannot parse debt(decimal)";
                }
            }

            return new Tuple<bool, string, decimal>(isConvertingSuccessful, errorMessage, debt);
        }

        /// <summary>
        /// Converts input string to char.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Tuple values (isValid, errorMessage, resultOfConverting).</returns>
        public static Tuple<bool, string, char> GenderConverter(string input)
        {
            bool isConvertingSuccessful = true;
            string errorMessage = string.Empty;
            char gender = char.MinValue;

            if (string.IsNullOrWhiteSpace(input))
            {
                isConvertingSuccessful = false;
                errorMessage = "gender Is Null Or White Space";
            }
            else
            {
                isConvertingSuccessful = char.TryParse(input, out gender);

                if (!isConvertingSuccessful)
                {
                    errorMessage = "Cannot parse gender(char)";
                }
            }

            return new Tuple<bool, string, char>(isConvertingSuccessful, errorMessage, gender);
        }
    }
}