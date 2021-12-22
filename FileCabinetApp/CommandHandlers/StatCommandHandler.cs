using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for stat command.
    /// </summary>
    public class StatCommandHandler : CommandHandlerBase
    {
        private const string CommandName = "stat";

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
            Program.FileCabinetService.GetStat();
        }
    }
}
