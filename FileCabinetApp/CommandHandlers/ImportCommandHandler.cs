using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for import command.
    /// </summary>
    public class ImportCommandHandler : CommandHandlerBase
    {
        private const string CommandName = "import";

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            string command = appCommandRequest.Command;
            string parameters = appCommandRequest.Parameters;

            if (command.Equals(CommandName, StringComparison.OrdinalIgnoreCase))
            {
                this.Import(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Import(string parameters)
        {
            var splitedParams = parameters.Split(' ');

            if (splitedParams.Length != 2)
            {
                Console.WriteLine("Wrong command. Please, try again.");
                return;
            }

            string exportFormat = splitedParams[0];
            string pathToFile = splitedParams[1];

            if (!Program.AvailableFormatsToExportImport.Contains(exportFormat))
            {
                Console.WriteLine("Wrong format. Please, try again.");
                return;
            }

            try
            {
                var snapshot = Program.FileCabinetService.MakeSnapshot(Program.Validator);

                using (StreamReader reader = new StreamReader(pathToFile))
                {
                    switch (exportFormat)
                    {
                        case "csv":
                            snapshot.LoadFromCsv(reader);
                            break;
                        case "xml":
                            snapshot.LoadFromXml(reader);
                            break;
                        default:
                            Console.WriteLine("There is no such format to export.");
                            break;
                    }
                }

                Program.FileCabinetService.Restore(snapshot);
            }
            catch (DirectoryNotFoundException directoryNotFoundException)
            {
                Console.WriteLine($"Import failed: {directoryNotFoundException.Message}.");
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                Console.WriteLine($"Import failed: {fileNotFoundException.Message}.");
            }
            catch (ArgumentException argumentException)
            {
                Console.WriteLine(argumentException.Message);
            }
        }
    }
}
