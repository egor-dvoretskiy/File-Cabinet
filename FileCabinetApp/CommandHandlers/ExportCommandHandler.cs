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
    /// Handler for export command.
    /// </summary>
    public class ExportCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "export";

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">File cabinet service.</param>
        public ExportCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            this.AssignToSimilarCommands(CommandName, appCommandRequest);

            string command = appCommandRequest.Command;
            string parameters = appCommandRequest.Parameters;

            if (command.Equals(CommandName, StringComparison.OrdinalIgnoreCase))
            {
                this.Export(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Export(string parameters)
        {
            var splitedParams = parameters.Split(' ');

            if (splitedParams.Length != 2)
            {
                Console.WriteLine("Wrong command. Please, try again.");
                return;
            }

            string exportFormat = splitedParams[0];
            string pathToFile = splitedParams[1];
            string fileName = pathToFile.Split('\\').Last();

            if (!Program.AvailableFormatsToExportImport.Contains(exportFormat))
            {
                Console.WriteLine("Wrong format. Please, try again.");
                return;
            }

            if (File.Exists(pathToFile))
            {
                Console.Write($"File is exist - rewrite {pathToFile}? [Y/n] ");
                string keyAgreement = this.ReadKeyAgreement();
                if (keyAgreement == "n")
                {
                    return;
                }
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(pathToFile))
                {
                    var snapshot = this.service.MakeSnapshot();

                    switch (exportFormat)
                    {
                        case "csv":
                            snapshot.SaveToCsv(writer);
                            break;
                        case "xml":
                            snapshot.SaveToXml(writer);
                            break;
                        case "database":
                            // snapshot.SaveToDatabase();
                            break;
                        default:
                            Console.WriteLine("There is no such format to export.");
                            break;
                    }

                    Console.WriteLine($"All records are exported to file {fileName}.");
                }
            }
            catch (DirectoryNotFoundException directoryNotFoundException)
            {
                _ = directoryNotFoundException;
                Console.WriteLine($"Export failed: can't open file {pathToFile}.");
            }
        }

        private string ReadKeyAgreement()
        {
            do
            {
                var key = Console.ReadKey()
                            .KeyChar
                            .ToString()
                            .ToLower();

                if (key.Length != 1 || !(new string[2] { "y", "n" }).Contains(key))
                {
                    Console.Write("\nWrong key input. Please, try again with [Y/N]: ");
                    continue;
                }
                else
                {
                    Console.WriteLine();
                    return key;
                }
            }
            while (true);
        }
    }
}
