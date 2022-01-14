using FileCabinetApp.ConditionWords;
using FileCabinetApp.Interfaces;
using FileCabinetApp.ServiceTools;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for update command.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "update";
        private const string CommandDivider = " where ";
        private const string KeySetWord = "set ";
        private const string KeySetterSeparator = ",";

        private const char ParameterSeparator = '=';
        private const char ParameterValueBrackets = '\'';

        private const int KeySetWordPosition = 0;
        private const int ParameterSetterPosition = 0;
        private const int ParameterSearcherPosition = 1;
        private const int ParameterNameIndex = 0;
        private const int ParameterValueIndex = 1;

        private readonly IRecordInputValidator inputValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">File cabinet service.</param>
        /// <param name="inputValidator">Validator for input args.</param>
        public UpdateCommandHandler(IFileCabinetService service, IRecordInputValidator inputValidator)
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
                this.Update(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Update(string parameters)
        {
            try
            {
                this.ValidateInputString(parameters);
                var setAndSearchStringsHolder = this.ParseInputString(parameters);

                ConditionWhere where = new ConditionWhere(this.service, this.inputValidator);
                List<FileCabinetRecord> filtredRecords = where.GetFilteredRecords(setAndSearchStringsHolder.Item2);

                var setParams = this.GetTupleOfParameters(setAndSearchStringsHolder.Item1, KeySetterSeparator, "set");

                List<FileCabinetRecord> updatedRecords = this.UpdateRecords(filtredRecords, setParams);

                this.service.Update(updatedRecords);
            }
            catch (ArgumentException aex)
            {
                Console.WriteLine(aex.Message);
            }
        }

        private List<FileCabinetRecord> UpdateRecords(List<FileCabinetRecord> records, List<Tuple<string, string>> setParams)
        {
            for (int i = 0; i < records.Count; i++)
            {
                for (int j = 0; j < setParams.Count; j++)
                {
                    var parameter = setParams[j].Item1;
                    var value = setParams[j].Item2;

                    if (parameter.Equals(nameof(FileCabinetRecord.Id), StringComparison.OrdinalIgnoreCase))
                    {
                        int id = this.GetParsedAndValidatedParameter<int>(value, InputConverter.IdConverter, this.inputValidator.IdValidator);

                        records[i].Id = id;
                    }
                    else if (parameter.Equals(nameof(FileCabinetRecord.FirstName), StringComparison.OrdinalIgnoreCase))
                    {
                        string firstName = this.GetParsedAndValidatedParameter<string>(value, InputConverter.StringConverter, this.inputValidator.FirstNameValidator);

                        records[i].FirstName = firstName;
                    }
                    else if (parameter.Equals(nameof(FileCabinetRecord.LastName), StringComparison.OrdinalIgnoreCase))
                    {
                        string lastName = this.GetParsedAndValidatedParameter<string>(value, InputConverter.StringConverter, this.inputValidator.LastNameValidator);

                        records[i].LastName = lastName;
                    }
                    else if (parameter.Equals(nameof(FileCabinetRecord.DateOfBirth), StringComparison.OrdinalIgnoreCase))
                    {
                        DateTime birthName = this.GetParsedAndValidatedParameter<DateTime>(value, InputConverter.BirthDateConverter, this.inputValidator.DateOfBirthValidator);

                        records[i].DateOfBirth = birthName;
                    }
                    else if (parameter.Equals(nameof(FileCabinetRecord.PersonalRating), StringComparison.OrdinalIgnoreCase))
                    {
                        short personalRating = this.GetParsedAndValidatedParameter<short>(value, InputConverter.PersonalRatingConverter, this.inputValidator.PersonalRatingValidator);

                        records[i].PersonalRating = personalRating;
                    }
                    else if (parameter.Equals(nameof(FileCabinetRecord.Salary), StringComparison.OrdinalIgnoreCase))
                    {
                        decimal salary = this.GetParsedAndValidatedParameter<decimal>(value, InputConverter.SalaryConverter, this.inputValidator.SalaryValidator);

                        records[i].Salary = salary;
                    }
                    else if (parameter.Equals(nameof(FileCabinetRecord.Gender), StringComparison.OrdinalIgnoreCase))
                    {
                        char gender = this.GetParsedAndValidatedParameter<char>(value, InputConverter.GenderConverter, this.inputValidator.GenderValidator);

                        records[i].Gender = gender;
                    }
                }
            }

            return records;
        }

        private T GetParsedAndValidatedParameter<T>(string input, Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
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

            T value = conversionResult.Item3;
            var validationResult = validator(value);

            if (!validationResult.Item1)
            {
                throw new ArgumentException($"Validation failed: {validationResult.Item2}. Please, correct your input.");
            }

            return value;
        }

        private Tuple<string, string> ParseInputString(string input)
        {
            var splittedInput = input.Split(CommandDivider, StringSplitOptions.RemoveEmptyEntries);

            if (splittedInput.Length != 2)
            {
                throw new ArgumentException($"Wrong command. Please, try again!");
            }

            string parameterSetter = splittedInput[ParameterSetterPosition];

            parameterSetter = parameterSetter.Substring(KeySetWord.Length);

            string parameterSearcher = splittedInput[ParameterSearcherPosition];

            return Tuple.Create(parameterSetter, parameterSearcher);
        }

        private void ValidateInputString(string input)
        {
            if (!input.Contains(KeySetWord))
            {
                throw new ArgumentException($"There is no keyword '{KeySetWord}' in input string. Please, try again!");
            }

            var index = input.Trim().IndexOf(KeySetWord);

            if (index != KeySetWordPosition)
            {
                throw new ArgumentException($"Keyword '{KeySetWord}' should follow 'update' command. Please, try again!");
            }
        }

        private List<Tuple<string, string>> GetTupleOfParameters(string input, string key, string msg)
        {
            List<Tuple<string, string>> setOfParameters = new List<Tuple<string, string>>();
            string[] splitedByAndDivider = input.Split(key, StringSplitOptions.TrimEntries);

            for (int i = 0; i < splitedByAndDivider.Length; i++)
            {
                var splittedParametersBySeparator = splitedByAndDivider[i].Split(ParameterSeparator, StringSplitOptions.TrimEntries);

                if (splittedParametersBySeparator.Length != 2)
                {
                    throw new ArgumentException($"Wrong parameters representation. Please, try again!");
                }

                string parameterName = splittedParametersBySeparator[ParameterNameIndex];
                string parameteValue = splittedParametersBySeparator[ParameterValueIndex].Trim(ParameterValueBrackets);

                setOfParameters.Add(new Tuple<string, string>(parameterName, parameteValue));
            }

            if (setOfParameters.Count == 0)
            {
                throw new ArgumentException($"There are no parameters to {msg}. Please, try again!");
            }

            return setOfParameters;
        }
    }
}
