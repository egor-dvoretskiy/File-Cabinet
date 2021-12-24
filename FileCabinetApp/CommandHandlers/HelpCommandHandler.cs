using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for help command.
    /// </summary>
    public class HelpCommandHandler : CommandHandlerBase
    {
        private const string CommandName = "help";

        /// <summary>
        /// Command help index.
        /// </summary>
        private const int CommandHelpIndex = 0;

        /// <summary>
        /// Description help index.
        /// </summary>
        private const int DescriptionHelpIndex = 1;

        /// <summary>
        /// Explanation Help index.
        /// </summary>
        private const int ExplanationHelpIndex = 2;

        /// <summary>
        /// Messages displays after entered help command.
        /// </summary>
        private readonly string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "display record statistics", "The 'stat' command displays record statistics." },
            new string[] { "create", "create user data", "The 'create' command creates user data." },
            new string[] { "list", "display stored records", "The 'list' command displays stored records." },
            new string[] { "edit", "edit stored user data", "The 'edit' command edits stored user data." },
            new string[] { "find", "find stored user data by specific field", "The 'find' command searches for stored user data by specific field." },
            new string[] { "export", "export data list to specific format", "The 'export' command converts data list to specific format." },
            new string[] { "import", "import data list from file", "The 'import' command converts file data to filesystem." },
            new string[] { "remove", "remove specific record by id", "The 'remove' command remove specific record by id." },
            new string[] { "purge", "defragments the data file", "The 'purge' command defragments the data file." },
        };

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            string command = appCommandRequest.Command;
            string parameters = appCommandRequest.Parameters;

            if (command.Equals(CommandName, StringComparison.OrdinalIgnoreCase))
            {
                this.PrintHelp(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(this.helpMessages, 0, this.helpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(this.helpMessages[index][ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in this.helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }
    }
}
