using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for purge command.
    /// </summary>
    public class PurgeCommandHandler : CommandHandlerBase
    {
        private const string CommandName = "purge";

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

            Program.FileCabinetService.Purge();
        }
    }
}
