using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for list command.
    /// </summary>
    public class ListCommandHandler : CommandHandlerBase
    {
        private const string CommandName = "list";

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
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
            var records = Program.FileCabinetService.GetRecords();

            if (records.Count == 0)
            {
                Console.WriteLine("List is empty.");
                return;
            }

            for (int i = 0; i < records.Count; i++)
            {
                this.PrintRecord(records[i]);
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
