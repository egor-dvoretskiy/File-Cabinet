using System;
using System.Text.RegularExpressions;

using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Interfaces;
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
        /// App status.
        /// </summary>
        public static bool IsRunning = true;

        /// <summary>
        /// Current storage mode.
        /// </summary>
        public static IFileCabinetService FileCabinetService = new FileCabinetMemoryService();

        /// <summary>
        /// Validator.
        /// </summary>
        public static IRecordValidator Validator = new DefaultValidator();

        private const string DeveloperName = "Egor Dvoretskiy";
        private const string WrongInputArgsMessage = "Wrong input arguments. Using default settings.";
        private const string CorrectCustomInputArgsMessage = "Using custom validation rules.";
        private const string CorrectDefaultInputArgsMessage = "Using default validation rules.";
        private const string CorrectStorageMemoryInputArgsMessage = "Using storage memory mode.";
        private const string CorrectStorageFilesystemInputArgsMessage = "Using storage filesystem mode.";

        private static FileStream fileStream = File.Open("cabinet-records.db", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

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

                commandHandler.Handle(new AppCommandRequest(command, parameters));
            }
            while (IsRunning);
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
                        Program.Validator = dictCommandLineValidationParameter[parameter].Item1;
                        Console.WriteLine(dictCommandLineValidationParameter[parameter].Item2);
                        break;
                    case CommandLineParameters.Storage when dictCommandLineStorageParameter.ContainsKey(args[indexArgs + 1]):
                        Program.FileCabinetService = dictCommandLineStorageParameter[parameter].Item1;
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

        private static ICommandHandler CreateCommandHandlers()
        {
            var commandHandler = new CommandHandler();
            return commandHandler;
        }
    }
}