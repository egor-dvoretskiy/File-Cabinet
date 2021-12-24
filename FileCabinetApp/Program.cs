using System;
using System.Text.RegularExpressions;

using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Services;
using FileCabinetApp.Validators;

#pragma warning disable CS8601 // Possible null reference argument.
#pragma warning disable SA1401 // Fields should be private

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
        /// <summary>
        /// Hint message.
        /// </summary>
        public const string HintMessage = "Enter your command, or enter 'help' to get help.";

        /// <summary>
        /// Available format to export/import.
        /// </summary>
        public static readonly string[] AvailableFormatsToExportImport = new string[]
        {
            "csv",
            "xml",
        };

        /// <summary>
        /// Validator.
        /// </summary>
        public static IRecordInputValidator InputValidator = new DefaultInputValidator();

        private const string DeveloperName = "Egor Dvoretskiy";
        private const string WrongInputArgsMessage = "Wrong input arguments.";
        private const string WrongInputValidationArgsMessage = "Wrong validation rule.";
        private const string WrongInputStorageArgsMessage = "Wrong storage mode.";
        private const string CorrectCustomInputArgsMessage = "Using custom validation rules.";
        private const string CorrectDefaultInputArgsMessage = "Using default validation rules.";
        private const string NoInputValidationArgsMessage = "There is no specified validation rule. Using default validation rules.";
        private const string NoInputStorageArgsMessage = "There is no specified storage mode. Using file system storage mode.";
        private const string CorrectStorageMemoryInputArgsMessage = "Using storage memory mode.";
        private const string CorrectStorageFilesystemInputArgsMessage = "Using storage filesystem mode.";

        private static IRecordValidator recordValidator = new ValidatorBuilder().CreateDefault();
        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(Program.recordValidator);
        private static bool isRunning = true;

        private static FileStream fileStream = File.Open("cabinet-records.db", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

        private static Dictionary<string, CommandLineParameters> dictCommandLineParameters = new Dictionary<string, CommandLineParameters>()
        {
            { "-v", CommandLineParameters.Validation },
            { "--validation-rules", CommandLineParameters.Validation },
            { "-s", CommandLineParameters.Storage },
            { "--storage", CommandLineParameters.Storage },
        };

        private static Dictionary<string, Tuple<IRecordInputValidator, IRecordValidator, string>> dictCommandLineValidationParameter = new ()
        {
            { "default", new Tuple<IRecordInputValidator, IRecordValidator, string>(new DefaultInputValidator(), new ValidatorBuilder().CreateDefault(), CorrectDefaultInputArgsMessage) },
            { "custom", new Tuple<IRecordInputValidator, IRecordValidator, string>(new CustomInputValidator(), new ValidatorBuilder().CreateCustom(), CorrectCustomInputArgsMessage) },
        };

        private static string[] storageModes = new string[]
        {
            "memory",
            "file",
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

            var commandHandler = Program.CreateCommandHandlers();

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
                const int parametersIndex = 1;

                var command = inputs[commandIndex];
                var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;

                var appCommandRequest = new AppCommandRequest(command, parameters);

                commandHandler.Handle(appCommandRequest);
            }
            while (isRunning);
        }

        /// <summary>
        /// Reads input string.
        /// </summary>
        /// <typeparam name="T">Depends on input.</typeparam>
        /// <param name="converter">Converter depends on input.</param>
        /// <param name="validator">Input validator.</param>
        /// <returns>Type depends on input.</returns>
        public static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
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

        /// <summary>
        /// Print records in console.
        /// </summary>
        /// <param name="records">Data to print.</param>
        public static void DefaultRecordPrint(IEnumerable<FileCabinetRecord> records)
        {
            foreach (FileCabinetRecord record in records)
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

        private static void ParseInputArgs(string[] args)
        {
            args = ParseArgumentArray(args);

            if (args.Length % 2 != 0)
            {
                throw new ArgumentException($"{Program.WrongInputArgsMessage}: amount of args is not even.");
            }

            args.SetValidators();
            args.SetService();
        }

        private static string[] ParseArgumentArray(string[] args)
        {
            List<string> arguments = new List<string>();

            foreach (string arg in args)
            {
                if (Regex.IsMatch(arg, @"^--[a-zA-Z]*-?[a-zA-Z]*=[a-zA-Z]*$"))
                {
                    var splitedArg = arg.Split('=');
                    arguments.Add(splitedArg[0].ToLower());
                    arguments.Add(splitedArg[1].ToLower());
                }
                else
                {
                    arguments.Add(arg);
                }
            }

            return arguments.ToArray();
        }

        private static void SetValidators(this string[] args)
        {
            int validationModeIndex = Array.FindIndex(args, 0, args.Length, i => dictCommandLineParameters.ContainsKey(i) && dictCommandLineParameters[i] == CommandLineParameters.Validation);

            if (validationModeIndex != -1 && validationModeIndex % 2 == 0)
            {
                var validationValue = args[validationModeIndex + 1];

                if (!dictCommandLineValidationParameter.ContainsKey(validationValue))
                {
                    throw new ArgumentException($"{Program.WrongInputValidationArgsMessage}: {nameof(dictCommandLineValidationParameter)} doesn't have such parameter as {validationValue}");
                }

                Program.recordValidator = dictCommandLineValidationParameter[validationValue].Item2;
                Program.InputValidator = dictCommandLineValidationParameter[validationValue].Item1;

                Console.WriteLine(dictCommandLineValidationParameter[validationValue].Item3);
            }
            else if (validationModeIndex % 2 != 0)
            {
                throw new ArgumentException(Program.WrongInputArgsMessage);
            }
            else
            {
                Console.WriteLine(Program.NoInputValidationArgsMessage);
            }
        }

        private static void SetService(this string[] args)
        {
            int storageModeIndex = Array.FindIndex(args, 0, args.Length, i => dictCommandLineParameters[i] == CommandLineParameters.Storage);

            if (storageModeIndex != -1 && storageModeIndex % 2 == 0)
            {
                var storageValue = args[storageModeIndex + 1];

                int indexStorageMode = Array.IndexOf(args, storageValue);

                switch (indexStorageMode)
                {
                    case 0:
                        Program.fileCabinetService = new FileCabinetMemoryService(Program.recordValidator);
                        Console.WriteLine(Program.CorrectStorageMemoryInputArgsMessage);
                        break;
                    case 1:
                        Program.fileCabinetService = new FileCabinetFileSystemService(Program.fileStream, Program.recordValidator);
                        Console.WriteLine(Program.CorrectStorageFilesystemInputArgsMessage);
                        break;
                    default:
                        throw new ArgumentException($"{Program.WrongInputStorageArgsMessage}: {nameof(storageModes)} doesn't have such parameter as {storageValue}");
                }
            }
            else if (storageModeIndex % 2 != 0)
            {
                throw new ArgumentException(Program.WrongInputArgsMessage);
            }
            else
            {
                Console.WriteLine(Program.NoInputStorageArgsMessage);
            }
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            var helpHandler = new HelpCommandHandler();
            var createHandler = new CreateCommandHandler(Program.fileCabinetService);
            var editHandler = new EditCommandHandler(Program.fileCabinetService);
            var exitHandler = new ExitCommandHandler(Program.SetAppRunningStatus);
            var exportHandler = new ExportCommandHandler(Program.fileCabinetService);
            var importHandler = new ImportCommandHandler(Program.fileCabinetService);
            var findHandler = new FindCommandHandler(Program.fileCabinetService, Program.DefaultRecordPrint);
            var listHandler = new ListCommandHandler(Program.fileCabinetService, Program.DefaultRecordPrint);
            var purgeHandler = new PurgeCommandHandler(Program.fileCabinetService);
            var removeHandler = new RemoveCommandHandler(Program.fileCabinetService);
            var statHandler = new StatCommandHandler(Program.fileCabinetService);

            createHandler.SetNext(editHandler);
            editHandler.SetNext(exitHandler);
            exitHandler.SetNext(exportHandler);
            exportHandler.SetNext(findHandler);
            findHandler.SetNext(helpHandler);
            helpHandler.SetNext(importHandler);
            importHandler.SetNext(listHandler);
            listHandler.SetNext(purgeHandler);
            purgeHandler.SetNext(removeHandler);
            removeHandler.SetNext(statHandler);

            return createHandler;
        }

        private static void SetAppRunningStatus(bool value)
        {
            Program.isRunning = value;
        }
    }
}