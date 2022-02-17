using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileCabinetApp.Interfaces;
using FileCabinetApp.ServiceTools;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for create command.
    /// </summary>
    public class CreateCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "create";
        private readonly IRecordInputValidator inputValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">File cabinet service.</param>
        /// <param name="inputValidator">Input validator.</param>
        public CreateCommandHandler(IFileCabinetService service, IRecordInputValidator inputValidator)
            : base(service)
        {
            this.inputValidator = inputValidator;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            this.ResetSimilarCommandsHandler(); // first command in chain.
            this.AssignToSimilarCommands(CommandName, appCommandRequest);

            string command = appCommandRequest.Command;
            string parameters = appCommandRequest.Parameters;

            if (command.Equals(CommandName, StringComparison.OrdinalIgnoreCase))
            {
                this.Create(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Create(string parameters)
        {
            Console.Write("First name: ");
            var firstName = this.ReadInput(InputConverter.StringConverter, this.inputValidator.FirstNameValidator);

            Console.Write("Last name: ");
            var lastName = this.ReadInput(InputConverter.StringConverter, this.inputValidator.LastNameValidator);

            Console.Write("Date of birth (month.day.year): ");
            var birthDate = this.ReadInput(InputConverter.BirthDateConverter, this.inputValidator.DateOfBirthValidator);

            Console.Write("Personal rating: ");
            var personalRating = this.ReadInput(InputConverter.PersonalRatingConverter, this.inputValidator.PersonalRatingValidator);

            Console.Write("Salary: ");
            var salary = this.ReadInput(InputConverter.SalaryConverter, this.inputValidator.SalaryValidator);

            Console.Write("Gender: ");
            var gender = this.ReadInput(InputConverter.GenderConverter, this.inputValidator.GenderValidator);

            FileCabinetRecord record = new FileCabinetRecord()
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = birthDate,
                PersonalRating = personalRating,
                Salary = salary,
                Gender = gender,
            };

            this.service.CreateRecord(record);
        }

        /// <summary>
        /// Reads input string.
        /// </summary>
        /// <typeparam name="T">Depends on input.</typeparam>
        /// <param name="converter">Converter depends on input.</param>
        /// <param name="validator">Input validator.</param>
        /// <returns>Type depends on input.</returns>
        private T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();

                if (input is null)
                {
                    Console.WriteLine($"Incorrect input. Please, try again.");
                    continue;
                }

                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }
    }
}
