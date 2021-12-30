using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Class contains validation settings.
    /// </summary>
    internal static class ValidatorSettings
    {
        /// <summary>
        /// Gets minimal possible id value.
        /// </summary>
        /// <value>
        /// Minimal possible length of first name.
        /// </value>
        public static int MinimalId { get; private set; } = 1;

        /// <summary>
        /// Gets minimal possible length of first name.
        /// </summary>
        /// <value>
        /// Minimal possible length of first name.
        /// </value>
        public static int MinimalLengthFirstName { get; private set; } = 2;

        /// <summary>
        /// Gets maximum possible length of first name.
        /// </summary>
        /// <value>
        /// Maximum possible length of first name.
        /// </value>
        public static int MaximumLengthFirstName { get; private set; } = 60;

        /// <summary>
        /// Gets minimal possible length of last name.
        /// </summary>
        /// <value>
        /// Minimal possible length of last name.
        /// </value>
        public static int MinimalLengthLastName { get; private set; } = 2;

        /// <summary>
        /// Gets maximum possible length of last name.
        /// </summary>
        /// <value>
        /// Maximum possible length of last name.
        /// </value>
        public static int MaximumLengthLastName { get; private set; } = 60;

        /// <summary>
        /// Gets minimal possible birth date.
        /// </summary>
        /// <value>
        /// Minimal possible birth date.
        /// </value>
        public static DateTime MinimalDate { get; private set; } = new DateTime(1950, 1, 1);

        /// <summary>
        /// Gets maximum possible birth date.
        /// </summary>
        /// <value>
        /// Maximum possible birth date.
        /// </value>
        public static DateTime MaximumDate { get; private set; } = DateTime.Now;

        /// <summary>
        /// Gets minimal possible personal rating.
        /// </summary>
        /// <value>
        /// Minimal possible personal rating.
        /// </value>
        public static short MinimalPersonalRating { get; private set; } = -10000;

        /// <summary>
        /// Gets minimal possible salary.
        /// </summary>
        /// <value>
        /// Minimal possible salary.
        /// </value>
        public static decimal MinimalSalary { get; private set; } = 0;

        /// <summary>
        /// Gets existing genders.
        /// </summary>
        /// <value>
        /// Existing genders.
        /// </value>
        public static char[] GenderChars { get; private set; } = new char[]
        {
                'M',
                'F',
        };

        /// <summary>
        /// Sets custom settings from json file.
        /// </summary>
        /// <param name="settingsType">Type of settings.</param>
        public static void LoadSpecifiedSettings(SettingsType settingsType)
        {
            var settingTypeString = settingsType.ToString().ToLower();
            IConfiguration config = ValidatorSettings.GetConfigurationBuilder();
            ValidatorSettings.AssignSpecifiedSettingsFromFile(settingTypeString, config);
        }

        private static IConfiguration GetConfigurationBuilder()
        {
            IConfiguration config = new ConfigurationBuilder()
                  .AddJsonFile("Validators/validation-rules.json", true, true)
                  .Build();

            return config;
        }

        private static void AssignSpecifiedSettingsFromFile(string settingsType, IConfiguration config)
        {
            MinimalLengthFirstName = config.GetValue<int>($"{settingsType}:firstName:min");
            MaximumLengthFirstName = config.GetValue<int>($"{settingsType}:firstName:max");
            MinimalLengthLastName = config.GetValue<int>($"{settingsType}:lastName:min");
            MaximumLengthLastName = config.GetValue<int>($"{settingsType}:lastName:max");
            MinimalDate = config.GetValue<DateTime>($"{settingsType}:dateOfBirth:from");
            MaximumDate = config.GetValue<DateTime>($"{settingsType}:dateOfBirth:to");
            MinimalPersonalRating = config.GetValue<short>($"{settingsType}:personalRating:min");
            MinimalSalary = config.GetValue<decimal>($"{settingsType}:salary:min");
            GenderChars = config.GetValue<string>($"{settingsType}:genderChars:chars").ToCharArray();
        }
    }
}
