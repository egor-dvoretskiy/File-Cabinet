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
    /// Handler for edit command.
    /// </summary>
    public class EditCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "edit";

        /// <summary>
        /// Initializes a new instance of the <see cref="EditCommandHandler"/> class.
        /// </summary>
        /// <param name="service">File cabinet service.</param>
        public EditCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            string command = appCommandRequest.Command;
            string parameters = appCommandRequest.Parameters;

            if (command.Equals(CommandName, StringComparison.OrdinalIgnoreCase))
            {
                this.Edit(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Edit(string parameters)
        {
            bool isIdValid = int.TryParse(parameters, out int id);
            if (!isIdValid)
            {
                Console.WriteLine("Wrong id value.");
                return;
            }

            try
            {
                int recordPosition = this.service.GetRecordPosition(id);

                Console.Write("First name: ");
                var firstName = Program.ReadInput(InputConverter.StringConverter, Program.InputValidator.FirstNameValidator);

                Console.Write("Last name: ");
                var lastName = Program.ReadInput(InputConverter.StringConverter, Program.InputValidator.LastNameValidator);

                Console.Write("Date of birth (month/day/year): ");
                var birthDate = Program.ReadInput(InputConverter.BirthDateConverter, Program.InputValidator.DateOfBirthValidator);

                Console.Write("Personal rating: ");
                var personalRating = Program.ReadInput(InputConverter.PersonalRatingConverter, Program.InputValidator.PersonalRatingValidator);

                Console.Write("Debt: ");
                var debt = Program.ReadInput(InputConverter.DebtConverter, Program.InputValidator.DebtValidator);

                Console.Write("Gender: ");
                var gender = Program.ReadInput(InputConverter.GenderConverter, Program.InputValidator.GenderValidator);

                FileCabinetRecord record = new FileCabinetRecord()
                {
                    Id = id,
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = birthDate,
                    PersonalRating = personalRating,
                    Debt = debt,
                    Gender = gender,
                };

                this.service.EditRecord(recordPosition, record);

                Console.WriteLine($"Record #{id} is edited.");
            }
            catch (ArgumentException aex)
            {
                Console.WriteLine(aex.Message);
            }
        }
    }
}
