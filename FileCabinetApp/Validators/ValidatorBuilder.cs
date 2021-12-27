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
            ValidatorSettings.LoadSpecifiedSettings(SettingsType.Default);

            return this.GetUniversalBuilder();
        }

        /// <summary>
        /// Creates settings for default validator.
        /// </summary>
        /// <returns>Composite validator.</returns>
        public IRecordValidator CreateCustom()
        {
            ValidatorSettings.LoadSpecifiedSettings(SettingsType.Custom);

            return this.GetUniversalBuilder();
        }

        private IRecordValidator GetUniversalBuilder()
        {
            return this
                .ValidateFirstName(ValidatorSettings.MinimalLengthFirstName, ValidatorSettings.MaximumLengthFirstName)
                .ValidateLastName(ValidatorSettings.MinimalLengthLastName, ValidatorSettings.MaximumLengthLastName)
                .ValidateBirthDate(ValidatorSettings.MinimalDate, ValidatorSettings.MaximumDate)
                .ValidateHeight(ValidatorSettings.MinimalPersonalRating)
                .ValidateSalary(ValidatorSettings.MinimalSalary)
                .ValidateGender(ValidatorSettings.GenderChars)
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
            this.validators.Add(new PersonalRatingValidator(minHeight));
            return this;
        }

        /// <summary>
        /// Validates date of birth.
        /// </summary>
        /// <param name="minSalary">Minimal possible weight of record.</param>
        /// <returns>Validator Builder.</returns>
        private ValidatorBuilder ValidateSalary(decimal minSalary)
        {
            this.validators.Add(new SalaryValidator(minSalary));
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
