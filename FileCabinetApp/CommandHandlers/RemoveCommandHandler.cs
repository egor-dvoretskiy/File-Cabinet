using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for remove command.
    /// </summary>
    public class RemoveCommandHandler : CommandHandlerBase
    {
        private const string CommandName = "remove";

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

            Program.FileCabinetService.RemoveRecordById(recordIdToRemove);
        }
    }
}
