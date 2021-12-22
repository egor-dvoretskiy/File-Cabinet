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
    /// Handler for find command.
    /// </summary>
    public class FindCommandHandler : CommandHandlerBase
    {
        private const string CommandName = "find";
        private IFileCabinetService service = new FileCabinetMemoryService();

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="service">File cabinet service.</param>
        public FindCommandHandler(IFileCabinetService service)
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
                this.Find(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Find(string parameters)
        {
            var paramsFindContainer = parameters.Split(' ');

            if (paramsFindContainer.Length < 2)
            {
                Console.WriteLine("Not enough parameters. Try again.");
                return;
            }

            FileCabinetRecord[] foundDataContainer = Array.Empty<FileCabinetRecord>();
            string parameterToFind = paramsFindContainer[1];

            switch (paramsFindContainer[0])
            {
                case "firstname":
                    foundDataContainer = this.service.FindByFirstName(parameterToFind).ToArray();
                    break;
                case "lastname":
                    foundDataContainer = this.service.FindByLastName(parameterToFind).ToArray();
                    break;
                case "dateofbirth":
                    foundDataContainer = this.service.FindByBirthDate(parameterToFind).ToArray();
                    break;
                default:
                    Console.WriteLine("There is no such parameter.");
                    break;
            }

            if (foundDataContainer.Length == 0)
            {
                Console.WriteLine("Nothing found.");
                return;
            }

            for (int i = 0; i < foundDataContainer.Length; i++)
            {
                this.PrintRecord(foundDataContainer[i]);
            }
        }

        private void PrintRecord(FileCabinetRecord record)
        {
            Console.WriteLine(
                    $"#{record.Id}, " +
                    $"{record.FirstName}, " +
                    $"{record.LastName}, " +
                    $"{record.DateOfBirth:yyyy-MMM-dd}, " +
                    $"{record.PersonalRating}, " +
                    $"{record.Debt}, " +
                    $"{record.Gender}.");
        }
    }
}
