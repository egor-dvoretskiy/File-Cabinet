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
    /// Handler for list command.
    /// </summary>
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "list";
        private Action<IEnumerable<FileCabinetRecord>> recordPrinter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="service">File cabinet service.</param>
        /// <param name="recordPrinter">Records printer.</param>
        public ListCommandHandler(IFileCabinetService service, Action<IEnumerable<FileCabinetRecord>> recordPrinter)
            : base(service)
        {
            this.recordPrinter = recordPrinter;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            this.AssignToSimilarCommands(CommandName, appCommandRequest);

            string command = appCommandRequest.Command;
            string parameters = appCommandRequest.Parameters;

            if (command.Equals(CommandName, StringComparison.OrdinalIgnoreCase))
            {
                this.List(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void List(string parameters)
        {
            var records = this.service.GetRecords();

            if (records.Count == 0)
            {
                Console.WriteLine("List is empty.");
                return;
            }

            this.recordPrinter(records);
        }
    }
}
