using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for edit command.
    /// </summary>
    public class EditCommandHandler : CommandHandlerBase
    {
        private const string CommandName = "edit";

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
                int recordPosition = Program.FileCabinetService.GetRecordPosition(id);

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
                    Id = id,
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = birthDate,
                    PersonalRating = personalRating,
                    Debt = debt,
                    Gender = gender,
                };

                Program.FileCabinetService.EditRecord(recordPosition, record);

                Console.WriteLine($"Record #{id} is edited.");
            }
            catch (ArgumentException aex)
            {
                Console.WriteLine(aex.Message);
            }
        }
    }
}
