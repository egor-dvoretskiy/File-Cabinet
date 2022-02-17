using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileCabinetApp.Interfaces;
using FileCabinetApp.ServiceTools;

namespace FileCabinetApp.ConditionWords
{
    /// <summary>
    /// Consdition processor.
    /// </summary>
    internal class ConditionWhere
    {
        private const string OperatorAnd = " and ";
        private const string OperatorOr = " or ";

        private readonly List<int> recordsId = new List<int>();
        private readonly IFileCabinetService service;
        private readonly IRecordInputValidator inputValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionWhere"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Record service.</param>
        /// <param name="inputValidator">Input Validator.</param>
        internal ConditionWhere(IFileCabinetService fileCabinetService, IRecordInputValidator inputValidator)
        {
            this.service = fileCabinetService;
            this.inputValidator = inputValidator;
        }

        /// <summary>
        /// Gets records by using condition.
        /// </summary>
        /// <param name="phrase">input condition.</param>
        /// <returns>List of records.</returns>
        internal List<FileCabinetRecord> GetFilteredRecords(string phrase)
        {
            List<FileCabinetRecord> recordsContainer = new List<FileCabinetRecord>();
            string[] splittedPhraseByOr = phrase.Split(OperatorOr, StringSplitOptions.TrimEntries);

            for (int i = 0; i < splittedPhraseByOr.Length; i++)
            {
                var listOfPairs = this.GetParameterPairsByAnd(splittedPhraseByOr[i]);

                var partOfRecords = this.GetListOfRecordsByParameters(listOfPairs);
                partOfRecords = this.GetRecordsByImplementingAndCondition(partOfRecords, listOfPairs);
                recordsContainer.AddRange(partOfRecords);
            }

            this.GetRidOfRepeatable(ref recordsContainer);

            return recordsContainer;
        }

        private List<FileCabinetRecord> GetRecordsByImplementingAndCondition(List<FileCabinetRecord> records, List<Tuple<string, string>> paramsToSearch)
        {
            List<FileCabinetRecord> listOfFilteredRecords = new List<FileCabinetRecord>();

            for (int i = 0; i < records.Count; i++)
            {
                bool condition = true;

                for (int j = 0; j < paramsToSearch.Count; j++)
                {
                    var parameter = paramsToSearch[j].Item1;
                    var value = paramsToSearch[j].Item2;

                    if (parameter.Equals(nameof(FileCabinetRecord.Id), StringComparison.OrdinalIgnoreCase))
                    {
                        int id = this.GetParsedAndValidatedParameter<int>(value, InputConverter.IdConverter, this.inputValidator.IdValidator);

                        if (records[i].Id == id)
                        {
                            condition &= true;
                        }
                        else
                        {
                            condition &= false;
                        }
                    }
                    else if (parameter.Equals(nameof(FileCabinetRecord.FirstName), StringComparison.OrdinalIgnoreCase))
                    {
                        string firstName = this.GetParsedAndValidatedParameter<string>(value, InputConverter.StringConverter, this.inputValidator.FirstNameValidator);

                        if (firstName.Equals(records[i].FirstName))
                        {
                            condition &= true;
                        }
                        else
                        {
                            condition &= false;
                        }
                    }
                    else if (parameter.Equals(nameof(FileCabinetRecord.LastName), StringComparison.OrdinalIgnoreCase))
                    {
                        string lastName = this.GetParsedAndValidatedParameter<string>(value, InputConverter.StringConverter, this.inputValidator.LastNameValidator);

                        if (lastName.Equals(records[i].LastName))
                        {
                            condition &= true;
                        }
                        else
                        {
                            condition &= false;
                        }
                    }
                    else if (parameter.Equals(nameof(FileCabinetRecord.DateOfBirth), StringComparison.OrdinalIgnoreCase))
                    {
                        DateTime birthName = this.GetParsedAndValidatedParameter<DateTime>(value, InputConverter.BirthDateConverter, this.inputValidator.DateOfBirthValidator);

                        if (CustomComparer.IsEqualDatesUpToDays(birthName, records[i].DateOfBirth))
                        {
                            condition &= true;
                        }
                        else
                        {
                            condition &= false;
                        }
                    }
                    else if (parameter.Equals(nameof(FileCabinetRecord.PersonalRating), StringComparison.OrdinalIgnoreCase))
                    {
                        short personalRating = this.GetParsedAndValidatedParameter<short>(value, InputConverter.PersonalRatingConverter, this.inputValidator.PersonalRatingValidator);

                        if (personalRating == records[i].PersonalRating)
                        {
                            condition &= true;
                        }
                        else
                        {
                            condition &= false;
                        }
                    }
                    else if (parameter.Equals(nameof(FileCabinetRecord.Salary), StringComparison.OrdinalIgnoreCase))
                    {
                        decimal salary = this.GetParsedAndValidatedParameter<decimal>(value, InputConverter.SalaryConverter, this.inputValidator.SalaryValidator);

                        if (salary == records[i].Salary)
                        {
                            condition &= true;
                        }
                        else
                        {
                            condition &= false;
                        }
                    }
                    else if (parameter.Equals(nameof(FileCabinetRecord.Gender), StringComparison.OrdinalIgnoreCase))
                    {
                        char gender = this.GetParsedAndValidatedParameter<char>(value, InputConverter.GenderConverter, this.inputValidator.GenderValidator);

                        if (gender.Equals(records[i].Gender))
                        {
                            condition &= true;
                        }
                        else
                        {
                            condition &= false;
                        }
                    }
                }

                if (condition)
                {
                    listOfFilteredRecords.Add(records[i]);
                }
            }

            return listOfFilteredRecords;
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

        private List<Tuple<string, string>> GetParameterPairsByAnd(string option)
        {
            List<Tuple<string, string>> setOfParameters = new List<Tuple<string, string>>();
            string[] splitedByAnd = option.Split(OperatorAnd, StringSplitOptions.TrimEntries);

            for (int i = 0; i < splitedByAnd.Length; i++)
            {
                var splittedParametersBySeparator = splitedByAnd[i].Split("=", StringSplitOptions.TrimEntries);

                if (splittedParametersBySeparator.Length != 2)
                {
                    throw new ArgumentException($"Wrong parameters representation in {splitedByAnd[i]}. Please, try again!");
                }

                string parameterName = splittedParametersBySeparator[0];
                string parameteValue = splittedParametersBySeparator[1].Trim('\'');

                setOfParameters.Add(new Tuple<string, string>(parameterName, parameteValue));
            }

            return setOfParameters;
        }

        private List<FileCabinetRecord> GetListOfRecordsByParameters(List<Tuple<string, string>> parametersSet)
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();

            for (int i = 0; i < parametersSet.Count; i++)
            {
                string parameterName = parametersSet[i].Item1;
                string parameterValue = parametersSet[i].Item2;

                if (parameterName.Equals(nameof(FileCabinetRecord.Id), StringComparison.OrdinalIgnoreCase))
                {
                    int id = int.Parse(parameterValue);

                    if (this.service.CheckRecordPresence(id))
                    {
                        records.Add(this.service.GetRecord(id));
                    }
                }
                else if (parameterName.Equals(nameof(FileCabinetRecord.FirstName), StringComparison.OrdinalIgnoreCase))
                {
                    var enumerableRecords = this.service.FindByFirstName(parameterValue);
                    records.AddRange(enumerableRecords.ToList());
                }
                else if (parameterName.Equals(nameof(FileCabinetRecord.LastName), StringComparison.OrdinalIgnoreCase))
                {
                    var enumerableRecords = this.service.FindByLastName(parameterValue);
                    records.AddRange(enumerableRecords.ToList());
                }
                else if (parameterName.Equals(nameof(FileCabinetRecord.DateOfBirth), StringComparison.OrdinalIgnoreCase))
                {
                    var enumerableRecords = this.service.FindByBirthDate(parameterValue);
                    records.AddRange(enumerableRecords.ToList());
                }
                else if (parameterName.Equals(nameof(FileCabinetRecord.PersonalRating), StringComparison.OrdinalIgnoreCase))
                {
                    var enumerableRecords = this.service.FindByPersonalRating(parameterValue);
                    records.AddRange(enumerableRecords.ToList());
                }
                else if (parameterName.Equals(nameof(FileCabinetRecord.Salary), StringComparison.OrdinalIgnoreCase))
                {
                    var enumerableRecords = this.service.FindBySalary(parameterValue);
                    records.AddRange(enumerableRecords.ToList());
                }
                else if (parameterName.Equals(nameof(FileCabinetRecord.Gender), StringComparison.OrdinalIgnoreCase))
                {
                    var enumerableRecords = this.service.FindByGender(parameterValue);
                    records.AddRange(enumerableRecords.ToList());
                }
                else
                {
                    throw new ArgumentException($"There is no such parameter as '{parameterName}'. Please, try again.");
                }
            }

            return records;
        }

        private void GetRidOfRepeatable(ref List<FileCabinetRecord> list)
        {
            int index = 0;
            while (index < list.Count)
            {
                FileCabinetRecord record = list[index];
                int id = record.Id;

                if (this.recordsId.IndexOf(id) == -1)
                {
                    this.recordsId.Add(id);
                    index++;
                }
                else
                {
                    list.RemoveAt(index);
                }
            }
        }
    }
}
