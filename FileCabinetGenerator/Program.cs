using System;
using System.Text.RegularExpressions;
using FileCabinetGenerator.FormatWriters;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Available command line parameters.
    /// </summary>
    public enum CommandLineGeneratorParameters
    {
        /// <summary>
        /// Output type.
        /// </summary>
        OutputType,

        /// <summary>
        /// Output.
        /// </summary>
        Output,

        /// <summary>
        /// Amount of Record.
        /// </summary>
        RecordAmount,

        /// <summary>
        /// Init id.
        /// </summary>
        StartId,
    }

    /// <summary>
    /// Main Class.
    /// </summary>
    public static class Program
    {
        private const string WrongInputArgsMessage = "Wrong input arguments. Please, try again.";

        private static Dictionary<string, CommandLineGeneratorParameters> dictCommandLineParameters = new Dictionary<string, CommandLineGeneratorParameters>()
        {
            { "--output-type", CommandLineGeneratorParameters.OutputType },
            { "-t", CommandLineGeneratorParameters.OutputType },
            { "--output", CommandLineGeneratorParameters.Output },
            { "-o", CommandLineGeneratorParameters.Output },
            { "--records-amount", CommandLineGeneratorParameters.RecordAmount },
            { "-a", CommandLineGeneratorParameters.RecordAmount },
            { "--start-id", CommandLineGeneratorParameters.StartId },
            { "-i", CommandLineGeneratorParameters.StartId },
        };

        private static int numFormat = 0;

        private static string[] availableFormatsToExport = new string[]
        {
            "csv",
            "xml",
            "sqldatabase",
            "nosqldatabase",
        };

        private static string pathToFileWithRecords = string.Empty;
        private static int recordsAmount = -1;
        private static int startId = -1;

        /// <summary>
        /// Init Method.
        /// </summary>
        /// <param name="args">Default parameters.</param>
        public static void Main(string[] args)
        {
            args = ParseInputArgs(args);

            int indexArgs = 0;
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
                    case CommandLineGeneratorParameters.OutputType when availableFormatsToExport.Contains(parameter):
                        numFormat = Array.IndexOf(availableFormatsToExport, parameter);
                        break;
                    case CommandLineGeneratorParameters.Output:
                        pathToFileWithRecords = parameter;
                        break;
                    case CommandLineGeneratorParameters.RecordAmount when int.TryParse(parameter, out int count):
                        recordsAmount = count;
                        break;
                    case CommandLineGeneratorParameters.StartId when int.TryParse(parameter, out int initId):
                        startId = initId;
                        break;
                    default:
                        Console.WriteLine(WrongInputArgsMessage);
                        return;
                }

                indexArgs += 2;
            }

            try
            {
                RecordGenerator.InitArrays(startId);

                using (StreamWriter writer = new StreamWriter(pathToFileWithRecords))
                {
                    switch (availableFormatsToExport[numFormat])
                    {
                        case "csv":
                            WriterDistributor.WriteToCsv(writer, recordsAmount);
                            break;
                        case "xml":
                            WriterDistributor.WriteToXml(writer, recordsAmount);
                            break;
                        case "sqldatabase":
                            WriterDistributor.WriteToSQLDatabase(recordsAmount);
                            break;
                        case "nosqldatabase":
                            WriterDistributor.WriteToNoSQLDatabase(recordsAmount);
                            break;
                        default:
                            Console.WriteLine("Wrong type of file.");
                            break;
                    }
                }

                Console.WriteLine($"{recordsAmount} records were written to {pathToFileWithRecords}");
            }
            catch (FileNotFoundException fileNotFoundexception)
            {
                Console.WriteLine(fileNotFoundexception.Message);
            }
            catch (DirectoryNotFoundException directoryNotFoundException)
            {
                Console.WriteLine(directoryNotFoundException.Message);
            }
        }

        private static string[] ParseInputArgs(string[] args)
        {
            List<string> arguments = new List<string>();

            foreach (string arg in args)
            {
                if (Regex.IsMatch(arg, @"^--[a-zA-Z]*-?[a-zA-Z]*=.*$"))
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
    }
}