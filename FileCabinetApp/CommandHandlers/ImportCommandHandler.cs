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
    /// Handler for import command.
    /// </summary>
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "import";

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">File cabinet service.</param>
        public ImportCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

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
                var snapshot = this.service.MakeSnapshot();

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

                this.service.Restore(snapshot);
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
