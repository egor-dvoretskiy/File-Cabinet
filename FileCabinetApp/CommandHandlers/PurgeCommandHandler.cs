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
    /// Handler for purge command.
    /// </summary>
    public class PurgeCommandHandler : CommandHandlerBase
    {
        private const string CommandName = "purge";
        private IFileCabinetService service = new FileCabinetMemoryService();

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="service">File cabinet service.</param>
        public PurgeCommandHandler(IFileCabinetService service)
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
                this.Purge(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Purge(string parameters)
        {
            if (parameters != string.Empty)
            {
                Console.WriteLine("Wrong parameters. Please, try again.");
                return;
            }

            this.service.Purge();
        }
    }
}
