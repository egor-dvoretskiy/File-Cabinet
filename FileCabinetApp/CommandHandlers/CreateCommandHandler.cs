using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for create command.
    /// </summary>
    public class CreateCommandHandler : CommandHandlerBase
    {
        private const string CommandName = "create";
        private IFileCabinetService service = new FileCabinetMemoryService();

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">File cabinet service.</param>
        public CreateCommandHandler(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
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
            var firstName = Program.ReadInput(InputConverter.StringConverter, Program.Validator.FirstNameValidator);

            Console.Write("Last name: ");
            var lastName = Program.ReadInput(InputConverter.StringConverter, Program.Validator.LastNameValidator);

            Console.Write("Date of birth (month/day/year): ");
            var birthDate = Program.ReadInput(InputConverter.BirthDateConverter, Program.Validator.DateOfBirthValidator);

            Console.Write("Personal rating: ");
            var personalRating = Program.ReadInput(InputConverter.PersonalRatingConverter, Program.Validator.PersonalRatingValidator);

            Console.Write("Debt: ");
            var debt = Program.ReadInput(InputConverter.DebtConverter, Program.Validator.DebtValidator);

            Console.Write("Gender: ");
            var gender = Program.ReadInput(InputConverter.GenderConverter, Program.Validator.GenderValidator);

            FileCabinetRecord record = new FileCabinetRecord()
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = birthDate,
                PersonalRating = personalRating,
                Debt = debt,
                Gender = gender,
            };

            int id = this.service.CreateRecord(record);

            Console.WriteLine($"Record #{id} is created.");
        }
    }
}
