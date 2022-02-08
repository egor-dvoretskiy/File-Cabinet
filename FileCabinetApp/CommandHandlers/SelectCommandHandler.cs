using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileCabinetApp.FormatWriters;
using FileCabinetApp.Interfaces;
using FileCabinetApp.ServiceTools;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for select command.
    /// </summary>
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "select";
        private const string ConditionWord = " where ";

        private readonly string allParametersToPrint = ReflectedRecordParams.GetPropertiesNameString(typeof(FileCabinetRecord));

        private readonly IRecordInputValidator inputValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="service">File cabinet service.</param>
        /// <param name="inputValidator">Validator for input args.</param>
        public SelectCommandHandler(IFileCabinetService service, IRecordInputValidator inputValidator)
            : base(service)
        {
            this.inputValidator = inputValidator;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            this.AssignToSimilarCommands(CommandName, appCommandRequest);

            string command = appCommandRequest.Command;
            string parameters = appCommandRequest.Parameters;

            if (command.Equals(CommandName, StringComparison.OrdinalIgnoreCase))
            {
                this.Select(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Select(string parameters)
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            string parametersToPrint = string.Empty;

            if (string.IsNullOrEmpty(parameters) || string.IsNullOrWhiteSpace(parameters))
            {
                Console.WriteLine("There are no parameters to print. Please try again.");
                return;
            }

            try
            {
                if (parameters.Contains(ConditionWord))
                {
                    var splitedParams = parameters.Split(ConditionWord, StringSplitOptions.TrimEntries);

                    if (splitedParams.Length != 2)
                    {
                        throw new ArgumentException($"Wrong command. Please, try again!");
                    }

                    string condition = splitedParams.Last();
                    string memoizerKey = MemoizerService.FormIdentificatorForMemoizing(condition);
                    var filteredRecords = this.service.Select(condition, memoizerKey, this.inputValidator);

                    parametersToPrint = splitedParams.First().Equals("*", StringComparison.OrdinalIgnoreCase) ? this.allParametersToPrint : splitedParams.First();
                    records = filteredRecords;
                }
                else
                {
                    parametersToPrint = parameters.Equals("*", StringComparison.OrdinalIgnoreCase) ? this.allParametersToPrint : parameters;
                    records = this.service.GetRecords().ToList();
                }

                if (records.Count == 0)
                {
                    Console.WriteLine("There are no records, that fits the condition.");
                    return;
                }

                TablePrinter.PrintTableRecords(records, parametersToPrint);
            }
            catch (ArgumentException argumentException)
            {
                Console.WriteLine(argumentException.Message);
            }
        }
    }
}
