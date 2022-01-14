using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.FormatWriters;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for help command.
    /// </summary>
    public class HelpCommandHandler : CommandHandlerBase
    {
        private const string CommandName = "help";

        private const string CommandHelpDescription =
            "prints the help screen.\n\t\tExample: help 'command'";

        private const string CommandExitDescription =
            "exits the application.\n\t\tExample(There are no additional parameters): exit";

        private const string CommandStatDescription =
            "display amount of records.\n\t\tExample(There are no additional parameters): stat";

        private const string CommandCreateDescription =
            "create record by manual input data.\n\t\tExample(There are no additional parameters): create";

        private const string CommandExportDescription =
            "export data list to specific format.\n\t\tExample: export csv records.csv\n\t\tcsv - file format.\n\t\trecords.csv - path to file.";

        private const string CommandImportDescription =
            "import data list from file.\n\t\tExample: import csv records.csv\n\t\tcsv - file format.\n\t\trecords.csv - path to file.";

        private const string CommandPurgeDescription =
            "defragments the data file.\n\t\tExample(There are no additional parameters): purge";

        private const string CommandInsertDescription =
            "add record to storage with specified fields.\n\t\tExample: insert (id, firstname, lastname, dateofbirth) values ('1', 'John', 'Doe', '5/18/1986')\n\t\tThe order in different brackets should matching.";

        private const string CommandUpdateDescription =
            "update the record with specified fields.\n\t\tExample: update set firstname = 'John', lastname = 'Doe' , dateofbirth = '5/18/1986' where id = '1' and firstname=John or lastname=Jigurda\n\t\tset ... - editable parameters in found records.\n\t\twhere - conditions to get neccessary records";

        private const string CommandDeleteDescription =
            "delete records by specified fields.\n\t\tExample: delete where id = '1'\n\t\twhere - conditions to get neccessary records.";

        private const string CommandSelectDescription =
            "select and show records by specified conditions.\n\t\tExample: select id, firstname, lastname where firstname = 'John' and lastname = 'Doe'\n\t\twhere - conditions to get neccessary records.\n\t\tid, firstname, lastname - display info.";

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
            new string[] { "help", CommandHelpDescription, "The 'help' command prints the help screen." },
            new string[] { "exit", CommandExitDescription, "The 'exit' command exits the application." },
            new string[] { "stat", CommandStatDescription, "The 'stat' command displays record statistics." },
            new string[] { "create", CommandCreateDescription, "The 'create' command creates user data." },
            new string[] { "export", CommandExportDescription, "The 'export' command converts data list to specific format." },
            new string[] { "import", CommandImportDescription, "The 'import' command converts file data to filesystem." },
            new string[] { "purge", CommandPurgeDescription, "The 'purge' command defragments the data file." },
            new string[] { "insert", CommandInsertDescription, "The 'insert' command adds record to storage with specified fields." },
            new string[] { "update", CommandUpdateDescription, "The 'update' command updates the record with specified fields." },
            new string[] { "delete", CommandDeleteDescription, "The 'delete' command deletes records by specified fields." },
            new string[] { "select", CommandSelectDescription, "The 'select' command pick records by specified conditions." },
        };

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            this.AssignToSimilarCommands(CommandName, appCommandRequest);

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

            this.PrintAvailableParameters();
            this.PrintAvailableFormats();
            Console.WriteLine();
        }

        private void PrintAvailableParameters()
        {
            var parameters = ReflectedRecordParams.GetPropertiesNameString(typeof(FileCabinetRecord));

            Console.WriteLine();
            Console.WriteLine($"Available parameters: {parameters}");
        }

        private void PrintAvailableFormats()
        {
            Console.WriteLine();
            Console.WriteLine($"Available file formats: xml, csv.");
        }
    }
}
