using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Holds the current command.
    /// </summary>
    public class CommandHandler : CommandHandlerBase
    {
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
        private static readonly string[][] HelpMessages = new string[][]
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

        private static readonly string[] AvailableFormatsToExport = new string[]
        {
            "csv",
            "xml",
        };

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("export", Export),
            new Tuple<string, Action<string>>("import", Import),
            new Tuple<string, Action<string>>("remove", Remove),
            new Tuple<string, Action<string>>("purge", Purge),
        };

        /// <summary>
        /// Holds command request.
        /// </summary>
        /// <param name="appCommandRequest">Command request.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            string command = appCommandRequest.Command;

            if (string.IsNullOrEmpty(command))
            {
                Console.WriteLine(Program.HintMessage);
                return;
            }

            var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
            if (index >= 0)
            {
                var parameters = appCommandRequest.Parameters;
                commands[index].Item2(parameters);
            }
            else
            {
                PrintMissedCommandInfo(command);
            }
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void Create(string parameters)
        {
            Console.Write("First name: ");
            var firstName = Program.ReadInput(InputConverter.StringConverter, Program.Validator.FirstNameValidator);

            Console.Write("Last name: ");
            var lastName = Program.ReadInput(InputConverter.StringConverter, Program.Validator.LastNameValidator);

            Console.Write("Date of birth (month/day/year): ");
            var birthDate = Program.ReadInput(InputConverter.BirthDateConverter, Program.Validator.DateOfBirthValidator);

            Console.Write("Personal rating: ");
            var personalRating = Program.ReadInput(InputConverter.PersonalRatingConverter, Program.Validator.PersonalRatingValidator);

            Console.Write("Debt: ");
            var debt = Program.ReadInput(InputConverter.DebtConverter, Program.Validator.DebtValidator);

            Console.Write("Gender: ");
            var gender = Program.ReadInput(InputConverter.GenderConverter, Program.Validator.GenderValidator);

            FileCabinetRecord record = new FileCabinetRecord()
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = birthDate,
                PersonalRating = personalRating,
                Debt = debt,
                Gender = gender,
            };

            int id = Program.FileCabinetService.CreateRecord(record);

            Console.WriteLine($"Record #{id} is created.");
        }

        private static void Find(string parameters)
        {
            var paramsFindContainer = parameters.Split(' ');

            if (paramsFindContainer.Length < 2)
            {
                Console.WriteLine("Not enough parameters. Try again.");
                return;
            }

            FileCabinetRecord[] foundDataContainer = Array.Empty<FileCabinetRecord>();
            string parameterToFind = paramsFindContainer[1];

            switch (paramsFindContainer[0])
            {
                case "firstname":
                    foundDataContainer = Program.FileCabinetService.FindByFirstName(parameterToFind).ToArray();
                    break;
                case "lastname":
                    foundDataContainer = Program.FileCabinetService.FindByLastName(parameterToFind).ToArray();
                    break;
                case "dateofbirth":
                    foundDataContainer = Program.FileCabinetService.FindByBirthDate(parameterToFind).ToArray();
                    break;
                default:
                    Console.WriteLine("There is no such parameter.");
                    break;
            }

            if (foundDataContainer.Length == 0)
            {
                Console.WriteLine("Nothing found.");
                return;
            }

            for (int i = 0; i < foundDataContainer.Length; i++)
            {
                PrintRecord(foundDataContainer[i]);
            }
        }

        private static void Edit(string parameters)
        {
            bool isIdValid = int.TryParse(parameters, out int id);
            if (!isIdValid)
            {
                Console.WriteLine("Wrong id value.");
                return;
            }

            try
            {
                int recordPosition = Program.FileCabinetService.GetRecordPosition(id);

                Console.Write("First name: ");
                var firstName = Program.ReadInput(InputConverter.StringConverter, Program.Validator.FirstNameValidator);

                Console.Write("Last name: ");
                var lastName = Program.ReadInput(InputConverter.StringConverter, Program.Validator.LastNameValidator);

                Console.Write("Date of birth (month/day/year): ");
                var birthDate = Program.ReadInput(InputConverter.BirthDateConverter, Program.Validator.DateOfBirthValidator);

                Console.Write("Personal rating: ");
                var personalRating = Program.ReadInput(InputConverter.PersonalRatingConverter, Program.Validator.PersonalRatingValidator);

                Console.Write("Debt: ");
                var debt = Program.ReadInput(InputConverter.DebtConverter, Program.Validator.DebtValidator);

                Console.Write("Gender: ");
                var gender = Program.ReadInput(InputConverter.GenderConverter, Program.Validator.GenderValidator);

                FileCabinetRecord record = new FileCabinetRecord()
                {
                    Id = id,
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = birthDate,
                    PersonalRating = personalRating,
                    Debt = debt,
                    Gender = gender,
                };

                Program.FileCabinetService.EditRecord(recordPosition, record);

                Console.WriteLine($"Record #{id} is edited.");
            }
            catch (ArgumentException aex)
            {
                Console.WriteLine(aex.Message);
            }
        }

        private static void List(string parameters)
        {
            var records = Program.FileCabinetService.GetRecords();

            if (records.Count == 0)
            {
                Console.WriteLine("List is empty.");
                return;
            }

            for (int i = 0; i < records.Count; i++)
            {
                PrintRecord(records[i]);
            }
        }

        private static void PrintRecord(FileCabinetRecord record)
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

        private static void Export(string parameters)
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

            if (!AvailableFormatsToExport.Contains(exportFormat))
            {
                Console.WriteLine("Wrong format. Please, try again.");
                return;
            }

            if (File.Exists(pathToFile))
            {
                Console.Write($"File is exist - rewrite {pathToFile}? [Y/n] ");
                string keyAgreement = ReadKeyAgreement();
                if (keyAgreement == "n")
                {
                    return;
                }
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(pathToFile))
                {
                    var snapshot = Program.FileCabinetService.MakeSnapshot(Program.Validator);

                    switch (exportFormat)
                    {
                        case "csv":
                            snapshot.SaveToCsv(writer);
                            break;
                        case "xml":
                            snapshot.SaveToXml(writer);
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

        private static string ReadKeyAgreement()
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

        private static void Import(string parameters)
        {
            var splitedParams = parameters.Split(' ');

            if (splitedParams.Length != 2)
            {
                Console.WriteLine("Wrong command. Please, try again.");
                return;
            }

            string exportFormat = splitedParams[0];
            string pathToFile = splitedParams[1];

            if (!AvailableFormatsToExport.Contains(exportFormat))
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

        private static void Remove(string parameters)
        {
            bool isParameterValid = int.TryParse(parameters, out int recordIdToRemove);

            if (!isParameterValid || recordIdToRemove < 1)
            {
                Console.WriteLine("Wrong parameters. Please, try again.");
                return;
            }

            Program.FileCabinetService.RemoveRecordById(recordIdToRemove);
        }

        private static void Purge(string parameters)
        {
            if (parameters != string.Empty)
            {
                Console.WriteLine("Wrong parameters. Please, try again.");
                return;
            }

            Program.FileCabinetService.Purge();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            Program.IsRunning = false;
        }

        private static void Stat(string parameters)
        {
            Program.FileCabinetService.GetStat();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in HelpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }
    }
}
