using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Class used as builer in 'builder fluent interface'.
    /// </summary>
    public class ValidatorBuilder
    {
        private List<IRecordValidator> validators = new List<IRecordValidator>();

        /// <summary>
        /// Creates settings for default validator.
        /// </summary>
        /// <returns>Composite validator.</returns>
        public IRecordValidator CreateDefault()
        {
            int minimalLengthFirstName = 2;
            int maximumLengthFirstName = 60;

            int minimalLengthLastName = 2;
            int maximumLengthLastName = 60;

            DateTime minimalDate = new DateTime(1950, 1, 1);
            DateTime maximumDate = DateTime.Now;

            short minimalHeight = 0;

            decimal minimalWeight = 0;

            char[] genderChars = new char[]
            {
                'M',
                'F',
            };

            return this
                .ValidateFirstName(minimalLengthFirstName, maximumLengthFirstName)
                .ValidateLastName(minimalLengthLastName, maximumLengthLastName)
                .ValidateBirthDate(minimalDate, maximumDate)
                .ValidateHeight(minimalHeight)
                .ValidateWeight(minimalWeight)
                .ValidateGender(genderChars)
                .Create();
        }

        /// <summary>
        /// Creates settings for default validator.
        /// </summary>
        /// <returns>Composite validator.</returns>
        public IRecordValidator CreateCustom()
        {
            int minimalLengthFirstName = 2;
            int maximumLengthFirstName = 60;

            int minimalLengthLastName = 2;
            int maximumLengthLastName = 60;

            DateTime minimalDate = DateTime.MinValue;
            DateTime maximumDate = DateTime.Now;

            short minimalHeight = 130;

            decimal minimalWeight = 15;

            char[] genderChars = new char[]
            {
                'M',
                'F',
                'T',
                'N',
                'O',
            };

            return this
                .ValidateFirstName(minimalLengthFirstName, maximumLengthFirstName)
                .ValidateLastName(minimalLengthLastName, maximumLengthLastName)
                .ValidateBirthDate(minimalDate, maximumDate)
                .ValidateHeight(minimalHeight)
                .ValidateWeight(minimalWeight)
                .ValidateGender(genderChars)
                .Create();
        }

        /// <summary>
        /// Validates first name.
        /// </summary>
        /// <param name="min">Minimal length of first name.</param>
        /// <param name="max">Maximum length of first name.</param>
        /// <returns>Validator Builder.</returns>
        private ValidatorBuilder ValidateFirstName(int min, int max)
        {
            this.validators.Add(new FirstNameValidator(min, max));
            return this;
        }

        /// <summary>
        /// Validates last name.
        /// </summary>
        /// <param name="min">Minimal length of last name.</param>
        /// <param name="max">Maximum length of last name.</param>
        /// <returns>Validator Builder.</returns>
        private ValidatorBuilder ValidateLastName(int min, int max)
        {
            this.validators.Add(new LastNameValidator(min, max));
            return this;
        }

        /// <summary>
        /// Validates date of birth.
        /// </summary>
        /// <param name="min">Minimal acceptable to valid date.</param>
        /// <param name="max">Maximum acceptable to valid date.</param>
        /// <returns>Validator Builder.</returns>
        private ValidatorBuilder ValidateBirthDate(DateTime min, DateTime max)
        {
            this.validators.Add(new DateOfBirthValidator(min, max));
            return this;
        }

        /// <summary>
        /// Validates date of birth.
        /// </summary>
        /// <param name="minHeight">Minimal possible height of record.</param>
        /// <returns>Validator Builder.</returns>
        private ValidatorBuilder ValidateHeight(short minHeight)
        {
            this.validators.Add(new HeightValidator(minHeight));
            return this;
        }

        /// <summary>
        /// Validates date of birth.
        /// </summary>
        /// <param name="minWeight">Minimal possible weight of record.</param>
        /// <returns>Validator Builder.</returns>
        private ValidatorBuilder ValidateWeight(decimal minWeight)
        {
            this.validators.Add(new WeightValidator(minWeight));
            return this;
        }

        /// <summary>
        /// Validates date of birth.
        /// </summary>
        /// <param name="possibleGenderChars">Possible to use gender chars.</param>
        /// <returns>Validator Builder.</returns>
        private ValidatorBuilder ValidateGender(char[] possibleGenderChars)
        {
            this.validators.Add(new GenderValidator(possibleGenderChars));
            return this;
        }

        /// <summary>
        /// Creates validator.
        /// </summary>
        /// <returns>Specified validator.</returns>
        private IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}
