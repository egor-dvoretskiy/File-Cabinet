using System;
using System.Text.RegularExpressions;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Validators;

#pragma warning disable CS8601 // Possible null reference argument.

namespace FileCabinetApp
{
    /// <summary>
    /// Available command line parameters.
    /// </summary>
    public enum CommandLineParameters
    {
        /// <summary>
        /// Validation commands.
        /// </summary>
        Validation,

        /// <summary>
        /// Storage commands.
        /// </summary>
        Storage,
    }

    /// <summary>
    /// Main Class.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Egor Dvoretskiy";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const string WrongInputArgsMessage = "Wrong input arguments. Using default settings.";
        private const string CorrectCustomInputArgsMessage = "Using custom validation rules.";
        private const string CorrectDefaultInputArgsMessage = "Using default validation rules.";
        private const string CorrectStorageMemoryInputArgsMessage = "Using storage memory mode.";
        private const string CorrectStorageFilesystemInputArgsMessage = "Using storage filesystem mode.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService();
        private static IRecordValidator validator = new DefaultValidator();

        private static bool isRunning = true;

        private static FileStream fileStream = File.Open("cabinet-records.db", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

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
        };

        private static string[][] helpMessages = new string[][]
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
        };

        private static string[] availableFormatsToExport = new string[]
        {
            "csv",
            "xml",
        };

        private static Dictionary<string, CommandLineParameters> dictCommandLineParameters = new Dictionary<string, CommandLineParameters>()
        {
            { "-v", CommandLineParameters.Validation },
            { "--validation-rules", CommandLineParameters.Validation },
            { "-s", CommandLineParameters.Storage },
            { "--storage", CommandLineParameters.Storage },
        };

        private static Dictionary<string, Tuple<IRecordValidator, string>> dictCommandLineValidationParameter = new Dictionary<string, Tuple<IRecordValidator, string>>()
        {
            { "default", new Tuple<IRecordValidator, string>(new DefaultValidator(), CorrectDefaultInputArgsMessage) },
            { "custom", new Tuple<IRecordValidator, string>(new CustomValidator(), CorrectCustomInputArgsMessage) },
        };

        private static Dictionary<string, Tuple<IFileCabinetService, string>> dictCommandLineStorageParameter = new Dictionary<string, Tuple<IFileCabinetService, string>>()
        {
            { "memory", new Tuple<IFileCabinetService, string>(new FileCabinetMemoryService(), CorrectStorageMemoryInputArgsMessage) },
            { "file", new Tuple<IFileCabinetService, string>(new FileCabinetFileSystemService(Program.fileStream), CorrectStorageFilesystemInputArgsMessage) },
        };

        /// <summary>
        /// Init Method.
        /// </summary>
        /// <param name="args">Default parameters.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");

            ParseInputArgs(args);

            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                if (input is null)
                {
                    continue;
                }

                var inputs = input.Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void ParseInputArgs(string[] args)
        {
            int indexArgs = 0;

            args = ParseArgumentArray(args);

            while (indexArgs < args.Length)
            {
                string arg = args[indexArgs].ToLower();

                if (args.Length % 2 != 0 || !dictCommandLineParameters.ContainsKey(arg))
                {
                    Console.WriteLine(WrongInputArgsMessage);
                    return;
                }

                var parameter = args[indexArgs + 1];

                switch (dictCommandLineParameters[arg])
                {
                    case CommandLineParameters.Validation when dictCommandLineValidationParameter.ContainsKey(args[indexArgs + 1]):
                        Program.validator = dictCommandLineValidationParameter[parameter].Item1;
                        Console.WriteLine(dictCommandLineValidationParameter[parameter].Item2);
                        break;
                    case CommandLineParameters.Storage when dictCommandLineStorageParameter.ContainsKey(args[indexArgs + 1]):
                        Program.fileCabinetService = dictCommandLineStorageParameter[parameter].Item1;
                        Console.WriteLine(dictCommandLineStorageParameter[parameter].Item2);
                        break;
                    default:
                        Console.WriteLine(WrongInputArgsMessage);
                        return;
                }

                indexArgs += 2;
            }
        }

        private static string[] ParseArgumentArray(string[] args)
        {
            List<string> arguments = new List<string>();

            foreach (string arg in args)
            {
                if (Regex.IsMatch(arg, @"^--[a-zA-Z]*-?[a-zA-Z]*=[a-zA-Z]*$"))
                {
                    var splitedArg = arg.Split('=');
                    arguments.Add(splitedArg[0]);
                    arguments.Add(splitedArg[1]);
                }
                else
                {
                    arguments.Add(arg);
                }
            }

            return arguments.ToArray();
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            Console.Write("First name: ");
            var firstName = Program.ReadInput(InputConverter.StringConverter, validator.FirstNameValidator);

            Console.Write("Last name: ");
            var lastName = Program.ReadInput(InputConverter.StringConverter, validator.LastNameValidator);

            Console.Write("Date of birth (month/day/year): ");
            var birthDate = Program.ReadInput(InputConverter.BirthDateConverter, validator.DateOfBirthValidator);

            Console.Write("Personal rating: ");
            var personalRating = Program.ReadInput(InputConverter.PersonalRatingConverter, validator.PersonalRatingValidator);

            Console.Write("Debt: ");
            var debt = Program.ReadInput(InputConverter.DebtConverter, validator.DebtValidator);

            Console.Write("Gender: ");
            var gender = Program.ReadInput(InputConverter.GenderConverter, validator.GenderValidator);

            FileCabinetRecord record = new FileCabinetRecord()
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = birthDate,
                PersonalRating = personalRating,
                Debt = debt,
                Gender = gender,
            };

            int id = Program.fileCabinetService.CreateRecord(record);

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
                    foundDataContainer = Program.fileCabinetService.FindByFirstName(parameterToFind).ToArray();
                    break;
                case "lastname":
                    foundDataContainer = Program.fileCabinetService.FindByLastName(parameterToFind).ToArray();
                    break;
                case "dateofbirth":
                    foundDataContainer = Program.fileCabinetService.FindByBirthDate(parameterToFind).ToArray();
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
                int recordPosition = Program.fileCabinetService.GetRecordPosition(id);

                Console.Write("First name: ");
                var firstName = Program.ReadInput(InputConverter.StringConverter, validator.FirstNameValidator);

                Console.Write("Last name: ");
                var lastName = Program.ReadInput(InputConverter.StringConverter, validator.LastNameValidator);

                Console.Write("Date of birth (month/day/year): ");
                var birthDate = Program.ReadInput(InputConverter.BirthDateConverter, validator.DateOfBirthValidator);

                Console.Write("Personal rating: ");
                var personalRating = Program.ReadInput(InputConverter.PersonalRatingConverter, validator.PersonalRatingValidator);

                Console.Write("Debt: ");
                var debt = Program.ReadInput(InputConverter.DebtConverter, validator.DebtValidator);

                Console.Write("Gender: ");
                var gender = Program.ReadInput(InputConverter.GenderConverter, validator.GenderValidator);

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

                Program.fileCabinetService.EditRecord(recordPosition, record);

                Console.WriteLine($"Record #{id} is edited.");
            }
            catch (ArgumentException aex)
            {
                Console.WriteLine(aex.Message);
            }
        }

        private static void List(string parameters)
        {
            var records = Program.fileCabinetService.GetRecords();

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

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();

                if (input is null)
                {
                    Console.WriteLine($"Incorrect input. Please, try again.");
                    continue;
                }

                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
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

            if (!availableFormatsToExport.Contains(exportFormat))
            {
                Console.WriteLine("Wrong format. Please, try again.");
                return;
            }

            if (File.Exists(pathToFile))
            {
                Console.Write($"File is exist - rewrite {pathToFile}? [Y/n] ");
                string keyAgreement = Program.ReadKeyAgreement();
                if (keyAgreement == "n")
                {
                    return;
                }
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(pathToFile))
                {
                    var snapshot = fileCabinetService.MakeSnapshot(Program.validator);

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

            if (!availableFormatsToExport.Contains(exportFormat))
            {
                Console.WriteLine("Wrong format. Please, try again.");
                return;
            }

            try
            {
                var snapshot = fileCabinetService.MakeSnapshot(Program.validator);

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

                Program.fileCabinetService.Restore(snapshot);
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

            Program.fileCabinetService.RemoveRecordById(recordIdToRemove);

            Console.WriteLine($"Record #{recordIdToRemove} is removed.");
        }
    }
}