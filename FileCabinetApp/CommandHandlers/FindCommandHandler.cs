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
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "find";
        private Action<IEnumerable<FileCabinetRecord>> recordPrinter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="service">File cabinet service.</param>
        /// <param name="recordPrinter">Print records.</param>
        public FindCommandHandler(IFileCabinetService service, Action<IEnumerable<FileCabinetRecord>> recordPrinter)
            : base(service)
        {
            this.recordPrinter = recordPrinter;
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
                    var iteratorFirstName = this.service.FindByFirstName(parameterToFind);
                    foundDataContainer = this.GetRecordsArray(iteratorFirstName);
                    break;
                case "lastname":
                    var iteratorLastName = this.service.FindByLastName(parameterToFind);
                    foundDataContainer = this.GetRecordsArray(iteratorLastName);
                    break;
                case "dateofbirth":
                    var iteratorBirthDate = this.service.FindByBirthDate(parameterToFind);
                    foundDataContainer = this.GetRecordsArray(iteratorBirthDate);
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

            this.recordPrinter(foundDataContainer);
        }

        private FileCabinetRecord[] GetRecordsArray(IEnumerable<FileCabinetRecord> recordIterator)
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();

            foreach (var record in recordIterator)
            {
                records.Add(record);
            }

            return records.ToArray();
        }
    }
}
