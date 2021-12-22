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
    /// Handler for remove command.
    /// </summary>
    public class RemoveCommandHandler : CommandHandlerBase
    {
        private const string CommandName = "remove";
        private IFileCabinetService service = new FileCabinetMemoryService();

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommandHandler"/> class.
        /// </summary>
        /// <param name="service">File cabinet service.</param>
        public RemoveCommandHandler(IFileCabinetService service)
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
                this.Remove(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Remove(string parameters)
        {
            bool isParameterValid = int.TryParse(parameters, out int recordIdToRemove);

            if (!isParameterValid || recordIdToRemove < 1)
            {
                Console.WriteLine("Wrong parameters. Please, try again.");
                return;
            }

            this.service.RemoveRecordById(recordIdToRemove);
        }
    }
}
