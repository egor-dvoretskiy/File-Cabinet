using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Keeps insert command logics.
    /// </summary>
    public class InsertCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "insert";
        private const string CommandSplitter = " values ";

        private const int ParametersIndex = 0;
        private const int ValuesIndex = 1;
        private const int AmountOfBracketParameterGroups = 2;
        private const int NeccessaryAmountOfParameters = 7;

        private const char ParametersDivider = ',';
        private const char ValuesBrackets = '\'';
        private readonly char[] groupBracketsType = new char[] { '(', ')' };

        private readonly IRecordInputValidator inputValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Specified service to work with.</param>
        /// <param name="inputValidator">Validator for input args.</param>
        public InsertCommandHandler(IFileCabinetService service, IRecordInputValidator inputValidator)
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
                this.Insert(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Insert(string parameters)
        {
            try
            {
                var parsedParameterPairs = this.ParseInputParameters(parameters);

                int id = this.GetParsedParameter<int>(nameof(FileCabinetRecord.Id), parsedParameterPairs, InputConverter.IdConverter, this.inputValidator.IdValidator);

                string firstName = this.GetParsedParameter<string>(nameof(FileCabinetRecord.FirstName), parsedParameterPairs, InputConverter.StringConverter, this.inputValidator.FirstNameValidator);

                var lastName = this.GetParsedParameter<string>(nameof(FileCabinetRecord.LastName), parsedParameterPairs, InputConverter.StringConverter, this.inputValidator.LastNameValidator);

                var birthDate = this.GetParsedParameter<DateTime>(nameof(FileCabinetRecord.DateOfBirth), parsedParameterPairs, InputConverter.BirthDateConverter, this.inputValidator.DateOfBirthValidator);

                var personalRating = this.GetParsedParameter<short>(nameof(FileCabinetRecord.PersonalRating), parsedParameterPairs, InputConverter.PersonalRatingConverter, this.inputValidator.PersonalRatingValidator);

                var salary = this.GetParsedParameter<decimal>(nameof(FileCabinetRecord.Salary), parsedParameterPairs, InputConverter.SalaryConverter, this.inputValidator.SalaryValidator);

                var gender = this.GetParsedParameter<char>(nameof(FileCabinetRecord.Gender), parsedParameterPairs, InputConverter.GenderConverter, this.inputValidator.GenderValidator);

                FileCabinetRecord record = new FileCabinetRecord()
                {
                    Id = id,
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = birthDate,
                    PersonalRating = personalRating,
                    Salary = salary,
                    Gender = gender,
                };

                this.service.InsertRecord(record);
            }
            catch (ArgumentException argumentException)
            {
                Console.WriteLine(argumentException.Message);
            }
        }

        private Tuple<string[], string[]> ParseInputParameters(string parameters)
        {
            var dividedByCommandSplitter = parameters.Split(CommandSplitter);

            if (dividedByCommandSplitter.Length != AmountOfBracketParameterGroups)
            {
                throw new ArgumentException("Wrong command. Please try again!");
            }

            dividedByCommandSplitter = this.GetRidOfBrackets(dividedByCommandSplitter);

            var parametersStringArray = this.GetParsedStringArray(dividedByCommandSplitter[ParametersIndex]);
            var valuesStringArray = this.GetParsedStringArray(dividedByCommandSplitter[ValuesIndex]);

            if (parametersStringArray.Length != valuesStringArray.Length)
            {
                throw new ArgumentException("The number of parameters in the two groups does not match. Please try again!");
            }

            if (parametersStringArray.Length != NeccessaryAmountOfParameters)
            {
                throw new ArgumentException("The number of parameters does not match the neccessary amount. Please try again!");
            }

            return new Tuple<string[], string[]>(parametersStringArray, valuesStringArray);
        }

        private string[] GetRidOfBrackets(string[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (!this.groupBracketsType.Contains(input[i].FirstOrDefault()) || !this.groupBracketsType.Contains(input[i].LastOrDefault()))
                {
                    throw new ArgumentException("Wrong command. Extra parameters in the line. Please try again!");
                }

                input[i] = input[i].Trim(this.groupBracketsType);
            }

            return input;
        }

        private string[] GetParsedStringArray(string input)
        {
            string[] parameters = input.Split(ParametersDivider);

            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i] = parameters[i].Trim().Trim(ValuesBrackets);
            }

            return parameters;
        }

        private T ParseAndValidateParameter<T>(string input, Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
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

        private T GetParsedParameter<T>(string parameterToSearch, Tuple<string[], string[]> parsedParameterPairs, Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            string[] parameterNames = parsedParameterPairs.Item1;
            string[] parameterValues = parsedParameterPairs.Item2;

            int index = Array.FindIndex(parameterNames, x => x.Equals(parameterToSearch, StringComparison.OrdinalIgnoreCase));

            if (index == -1)
            {
                throw new ArgumentException($"There is no parameter, such as '{parameterToSearch}'. Please, try again!");
            }

            string inputValue = parameterValues[index];

            T value = this.ParseAndValidateParameter<T>(inputValue, converter, validator);

            return value;
        }
    }
}
