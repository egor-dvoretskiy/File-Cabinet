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
    public class CreateCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "create";

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">File cabinet service.</param>
        public CreateCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
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
            var firstName = Program.ReadInput(InputConverter.StringConverter, Program.InputValidator.FirstNameValidator);

            Console.Write("Last name: ");
            var lastName = Program.ReadInput(InputConverter.StringConverter, Program.InputValidator.LastNameValidator);

            Console.Write("Date of birth (month/day/year): ");
            var birthDate = Program.ReadInput(InputConverter.BirthDateConverter, Program.InputValidator.DateOfBirthValidator);

            Console.Write("Personal rating: ");
            var personalRating = Program.ReadInput(InputConverter.PersonalRatingConverter, Program.InputValidator.PersonalRatingValidator);

            Console.Write("Debt: ");
            var debt = Program.ReadInput(InputConverter.SalaryConverter, Program.InputValidator.SalaryValidator);

            Console.Write("Gender: ");
            var gender = Program.ReadInput(InputConverter.GenderConverter, Program.InputValidator.GenderValidator);

            FileCabinetRecord record = new FileCabinetRecord()
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = birthDate,
                PersonalRating = personalRating,
                Salary = debt,
                Gender = gender,
            };

            int id = this.service.CreateRecord(record);

            Console.WriteLine($"Record #{id} is created.");
        }
    }
}
