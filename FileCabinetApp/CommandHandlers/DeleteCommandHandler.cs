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
    /// Handler for delete command.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "delete";
        private const string KeyWord = "where ";

        private const int KeyWordPosition = 0;
        private const int ParameterIndex = 0;
        private const int ValueIndex = 1;
        private const int AmountOfSplitedParameters = 2;

        private const char ValuesBrackets = '\'';
        private const char ParametersDivider = '=';

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="service">File cabinet service.</param>
        public DeleteCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            this.AssignToSimilarCommands(CommandName, appCommandRequest);

            string command = appCommandRequest.Command;
            string parameters = appCommandRequest.Parameters;

            if (command.Equals(CommandName, StringComparison.OrdinalIgnoreCase))
            {
                this.Delete(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Delete(string parameters)
        {
            try
            {
                var parameterTuple = this.GetParameterTuple(parameters);

                List<int> listOfIdToDelete = this.GetRecordsToDelete(parameterTuple);

                this.service.Delete(listOfIdToDelete);
            }
            catch (ArgumentException argumentException)
            {
                Console.WriteLine(argumentException.Message);
            }
        }

        private Tuple<string, string> GetParameterTuple(string input)
        {
            this.ValidateInputString(input);

            input = input.Substring(KeyWord.Length);

            string[] inputParts = input.Split(ParametersDivider);

            if (inputParts.Length != AmountOfSplitedParameters)
            {
                throw new ArgumentException($"Wrong condition after keyword '{KeyWord}'. Please, try again!");
            }

            inputParts = this.GetRidOfExtraSpace(inputParts);

            string parameter = inputParts[ParameterIndex];
            string valueInStringRepresentation = inputParts[ValueIndex];

            return Tuple.Create(parameter, valueInStringRepresentation);
        }

        private List<int> GetRecordsToDelete(Tuple<string, string> parameterTuple)
        {
            string parameter = parameterTuple.Item1;
            string value = parameterTuple.Item2;

            List<int> listOfIds = new List<int>();

            if (parameter.Equals(nameof(FileCabinetRecord.Id), StringComparison.OrdinalIgnoreCase))
            {
                int id = this.GetParsedAndValidatedParameter(value, InputConverter.IdConverter, Program.InputValidator.IdValidator);
                listOfIds.Add(id);
            }
            else if (parameter.Equals(nameof(FileCabinetRecord.FirstName), StringComparison.OrdinalIgnoreCase))
            {
                var enumerableRecords = this.service.FindByFirstName(value);
                listOfIds = this.FillListFromEnumerable(enumerableRecords);
            }
            else if (parameter.Equals(nameof(FileCabinetRecord.LastName), StringComparison.OrdinalIgnoreCase))
            {
                var enumerableRecords = this.service.FindByLastName(value);
                listOfIds = this.FillListFromEnumerable(enumerableRecords);
            }
            else if (parameter.Equals(nameof(FileCabinetRecord.DateOfBirth), StringComparison.OrdinalIgnoreCase))
            {
                var enumerableRecords = this.service.FindByBirthDate(value);
                listOfIds = this.FillListFromEnumerable(enumerableRecords);
            }
            else
            {
                throw new ArgumentException($"There is no such parameter as '{parameter}'. Please, try again.");
            }

            return listOfIds;
        }

        private List<int> FillListFromEnumerable(IEnumerable<FileCabinetRecord> records)
        {
            List<int> listOfIds = new List<int>();

            foreach (var record in records)
            {
                listOfIds.Add(record.Id);
            }

            return listOfIds;
        }

        private void ValidateInputString(string input)
        {
            if (!input.Contains(KeyWord))
            {
                throw new ArgumentException($"There is no keyword '{KeyWord}' in input string. Please, try again!");
            }

            var index = input.Trim().IndexOf(KeyWord);

            if (index != KeyWordPosition)
            {
                throw new ArgumentException($"Keyword '{KeyWord}' should follow 'delete' command. Please, try again!");
            }
        }

        private string[] GetRidOfExtraSpace(string[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = input[i].Trim().Trim(ValuesBrackets);
            }

            return input;
        }

        private int GetParsedAndValidatedParameter(string input, Func<string, Tuple<bool, string, int>> converter, Func<int, Tuple<bool, string>> validator)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException($"Input parameter is null or empty. Please, try again.");
            }

            var conversionResult = converter(input);

            if (!conversionResult.Item1)
            {
                throw new ArgumentException($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
            }

            var value = conversionResult.Item3;
            var validationResult = validator(value);

            if (!validationResult.Item1)
            {
                throw new ArgumentException($"Validation failed: {validationResult.Item2}. Please, correct your input.");
            }

            return value;
        }
    }
}
