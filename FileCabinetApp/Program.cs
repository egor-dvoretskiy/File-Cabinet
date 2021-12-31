using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Services;
using FileCabinetApp.Validators;

#pragma warning disable SA1401 // Fields should be private
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace FileCabinetApp
{
    /// <summary>
    /// Validator settings types.
    /// </summary>
    public enum SettingsType
    {
        /// <summary>
        /// Default settings for validators.
        /// </summary>
        Default,

        /// <summary>
        /// Custom setting for validators.
        /// </summary>
        Custom,
    }

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
        public static IRecordInputValidator InputValidator;

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
        private const string CorrectServiceMeterInputArgsMessage = "Using service time measuring.";
        private const string CorrectServiceLoggerInputArgsMessage = "Using service logger.";

        private static IRecordValidator recordValidator;
        private static IFileCabinetService fileCabinetService;
        private static bool isRunning = true;

        private static FileStream fileStream = File.Open("cabinet-records.db", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

        private static Dictionary<string, CommandLineParameters> dictCommandLineParameters = new Dictionary<string, CommandLineParameters>()
        {
            { "-v", CommandLineParameters.Validation },
            { "--validation-rules", CommandLineParameters.Validation },
            { "-s", CommandLineParameters.Storage },
            { "--storage", CommandLineParameters.Storage },
        };

        private static Dictionary<string, SettingsType> dictSettingsType = new ()
        {
            { "default", SettingsType.Default },
            { "custom", SettingsType.Custom },
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

            try
            {
                ParseInputArgs(args);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Console.WriteLine(argumentNullException.Message);
            }
            catch (ArgumentException argumentException)
            {
                Console.WriteLine(argumentException.Message);
            }

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
                Console.WriteLine(Program.GetRecordInStringRepresentation(record));
            }
        }

        private static string GetRecordInStringRepresentation(FileCabinetRecord record)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"#{record.Id}, ");
            stringBuilder.Append($"{record.FirstName}, ");
            stringBuilder.Append($"{record.LastName}, ");
            stringBuilder.Append($"{record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, ");
            stringBuilder.Append($"{record.PersonalRating}, ");
            stringBuilder.Append($"{record.Salary}, ");
            stringBuilder.Append($"{record.Gender}.");

            return stringBuilder.ToString();
        }

        private static void ParseInputArgs(string[] args)
        {
            args = ParseArgumentArray(args);

            args.SetValidators();
            args.SetService();
            args.SetServiceMeterMode();
            args.SetServiceLoggerMode();
        }

        private static void SetServiceLoggerMode(this string[] args)
        {
            string loggerModeStringToFind = "-use-logger";
            int serviceMeterModeIndex = Array.FindIndex(args, 0, args.Length, i => i.Equals(loggerModeStringToFind, StringComparison.OrdinalIgnoreCase));

            if (serviceMeterModeIndex != -1)
            {
                ServiceLogger serviceLogger = new ServiceLogger(Program.fileCabinetService);
                Program.fileCabinetService = serviceLogger;

                Console.WriteLine(Program.CorrectServiceLoggerInputArgsMessage);
            }
        }

        private static void SetServiceMeterMode(this string[] args)
        {
            string meterModeStringToFind = "-use-stopwatch";
            int serviceMeterModeIndex = Array.FindIndex(args, 0, args.Length, i => i.Equals(meterModeStringToFind, StringComparison.OrdinalIgnoreCase));

            if (serviceMeterModeIndex != -1)
            {
                ServiceMeter serviceMeter = new ServiceMeter(Program.fileCabinetService);
                Program.fileCabinetService = serviceMeter;

                Console.WriteLine(Program.CorrectServiceMeterInputArgsMessage);
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

            if (validationModeIndex != -1)
            {
                if (validationModeIndex + 1 >= args.Length)
                {
                    throw new ArgumentOutOfRangeException($"Wrong input validation args order.");
                }

                var validationValue = args[validationModeIndex + 1];

                if (!dictSettingsType.ContainsKey(validationValue))
                {
                    throw new ArgumentException($"Validation args don't have such parameter as {validationValue}");
                }

                var type = dictSettingsType[validationValue];

                switch (type)
                {
                    case SettingsType.Default:
                        Program.recordValidator = new ValidatorBuilder().CreateDefault();
                        Program.InputValidator = new DefaultInputValidator();

                        Console.WriteLine(CorrectDefaultInputArgsMessage);
                        break;
                    case SettingsType.Custom:
                        Program.recordValidator = new ValidatorBuilder().CreateCustom();
                        Program.InputValidator = new CustomInputValidator();

                        Console.WriteLine(CorrectCustomInputArgsMessage);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Console.WriteLine(Program.NoInputValidationArgsMessage);
            }
        }

        private static void SetService(this string[] args)
        {
            int storageModeIndex = Array.FindIndex(args, 0, args.Length, i => dictCommandLineParameters.ContainsKey(i) && dictCommandLineParameters[i] == CommandLineParameters.Storage);

            if (storageModeIndex != -1)
            {
                if (storageModeIndex + 1 >= args.Length)
                {
                    throw new ArgumentOutOfRangeException($"Wrong input storage args order.");
                }

                var storageValue = args[storageModeIndex + 1];

                int indexStorageMode = Array.IndexOf(storageModes, storageValue);

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
                        throw new ArgumentException($"Storage args doesn't have such parameter as {storageValue}");
                }
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
            var insertHandler = new InsertCommandHandler(Program.fileCabinetService);
            var deleteHandler = new DeleteCommandHandler(Program.fileCabinetService);
            var updateHandler = new UpdateCommandHandler(Program.fileCabinetService);

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
            statHandler.SetNext(insertHandler);
            insertHandler.SetNext(deleteHandler);
            deleteHandler.SetNext(updateHandler);

            return createHandler;
        }

        private static void SetAppRunningStatus(bool value)
        {
            Program.isRunning = value;
        }
    }
}