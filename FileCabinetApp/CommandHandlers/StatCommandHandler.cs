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
    /// Handler for stat command.
    /// </summary>
    public class StatCommandHandler : CommandHandlerBase
    {
        private const string CommandName = "stat";
        private IFileCabinetService service = new FileCabinetMemoryService();

        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="service">File cabinet service.</param>
        public StatCommandHandler(IFileCabinetService service)
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
                this.Stat(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Stat(string parameters)
        {
            this.service.GetStat();
        }
    }
}
