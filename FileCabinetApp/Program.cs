using System;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Validators;

#pragma warning disable CS8601 // Possible null reference argument.

namespace FileCabinetApp
{
    /// <summary>
    /// Main Class.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Egor Dvoretskiy";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const string WrongInputArgsMessage = "Wrong input arguments. Using default validation rules.";
        private const string CorrectCustomInputArgsMessage = "Using custom validation rules.";
        private const string CorrectDefaultInputArgsMessage = "Using default validation rules.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static FileCabinetService fileCabinetService = new FileCabinetDefaultService(new DefaultValidator());

        private static bool isRunning = true;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
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
        };

        private static string[] availableArgsValues = new string[]
            {
                "default",
                "custom",
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
            bool isArgsProcessed = false;

            if (args.Length == 1)
            {
                args = args[0].Split('=');
                isArgsProcessed = true;
            }

            if ((args.Length != 2 && args.Length != 1) || (args.Length != 2 && isArgsProcessed))
            {
                Console.WriteLine(WrongInputArgsMessage);
                return;
            }

            string currentServiceModeCaseLowered = args[1].ToLower();

            if ((args[0] == "-v" || args[0] == "--validation-rules") && availableArgsValues.Contains(currentServiceModeCaseLowered))
            {
                if (string.Equals(currentServiceModeCaseLowered, availableArgsValues[0]))
                {
                    Console.WriteLine(CorrectDefaultInputArgsMessage);
                }
                else
                {
                    fileCabinetService = new FileCabinetCustomService(new CustomValidator());
                    Console.WriteLine(CorrectCustomInputArgsMessage);
                }
            }
            else
            {
                Console.WriteLine(WrongInputArgsMessage);
            }
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
            int id = -1;
            while (id == -1)
            {
                Console.Write("First name: ");
                var firstName = Console.ReadLine();

                Console.Write("Last name: ");
                var lastName = Console.ReadLine();

                Console.Write("Date of birth (month/day/year): ");
                var birthDateString = Console.ReadLine();

                Console.Write("Personal rating: ");
                var personalRatingString = Console.ReadLine();

                Console.Write("Debt: ");
                var debtString = Console.ReadLine();

                Console.Write("Gender: ");
                var genderString = Console.ReadLine();

                RecordInputObject recordInputObject = new RecordInputObject()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = birthDateString,
                    PersonalRating = personalRatingString,
                    Debt = debtString,
                    Gender = genderString,
                };

                id = Program.fileCabinetService.CreateRecord(recordInputObject);
            }

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
                    foundDataContainer = Program.fileCabinetService.FindByFirstName(parameterToFind);
                    break;
                case "lastname":
                    foundDataContainer = Program.fileCabinetService.FindByLastName(parameterToFind);
                    break;
                case "dateofbirth":
                    foundDataContainer = Program.fileCabinetService.FindByBirthDate(parameterToFind);
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
                int listValue = Program.fileCabinetService.GetPositionInListRecordsById(id);
                if (listValue == -1)
                {
                    Console.WriteLine($"#{id} record is not found.");
                    Program.fileCabinetService.EditRecord(listValue, new RecordInputObject());
                    return;
                }

                Console.Write("First name: ");
                var firstName = Console.ReadLine();

                Console.Write("Last name: ");
                var lastName = Console.ReadLine();

                Console.Write("Date of birth (month/day/year): ");
                var birthDateString = Console.ReadLine();

                Console.Write("Personal rating: ");
                var personalRatingString = Console.ReadLine();

                Console.Write("Debt: ");
                var debtString = Console.ReadLine();

                Console.Write("Gender: ");
                var genderString = Console.ReadLine();

                RecordInputObject recordInputObject = new RecordInputObject()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = birthDateString,
                    PersonalRating = personalRatingString,
                    Debt = debtString,
                    Gender = genderString,
                };

                Program.fileCabinetService.EditRecord(listValue, recordInputObject);

                Console.WriteLine($"Record #{id} is edited.");
            }
            catch (ArgumentException aex)
            {
                _ = aex;
            }
        }

        private static void List(string parameters)
        {
            var records = Program.fileCabinetService.GetRecords();

            if (records.Length == 0)
            {
                Console.WriteLine("List is empty.");
                return;
            }

            for (int i = 0; i < records.Length; i++)
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
    }
}