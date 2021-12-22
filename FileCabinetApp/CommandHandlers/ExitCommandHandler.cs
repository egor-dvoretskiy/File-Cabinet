using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for exit command.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        private const string CommandName = "exit";

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            string command = appCommandRequest.Command;
            string parameters = appCommandRequest.Parameters;

            if (command.Equals(CommandName, StringComparison.OrdinalIgnoreCase))
            {
                this.Exit(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            Program.IsRunning = false;
        }
    }
}
