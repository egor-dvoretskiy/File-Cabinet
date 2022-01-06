using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for select command.
    /// </summary>
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "select";
        private const string ConditionWord = "where";

        private const string KeyAndWord = " and ";
        private const string KeyOrWord = " or ";
        private const string KeySetterSeparator = ",";

        private const char ParameterSeparator = '=';
        private const char ParameterValueBrackets = '\'';

        private const int ParameterNameIndex = 0;
        private const int ParameterValueIndex = 1;
        private const int AdditionalSpaceInCell = 2;

        private const char CellLine = '-';
        private const char CellCross = '+';
        private const char CellWall = '|';

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="service">File cabinet service.</param>
        public SelectCommandHandler(IFileCabinetService service)
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
                    string separator = condition.Contains(KeyOrWord, StringComparison.OrdinalIgnoreCase) ? KeyOrWord : KeyAndWord;

                    var parametersNameAndValues = this.GetParametersByNameAndValue(condition, separator);
                    var recordsFromService = this.GetListOfRecordsByParameters(parametersNameAndValues);
                    var filteredRecords = this.GetFilteredRecords(recordsFromService, separator, parametersNameAndValues);

                    parametersToPrint = splitedParams.First();
                    records = filteredRecords;
                }
                else
                {
                    parametersToPrint = parameters;
                    records = this.service.GetRecords().ToList();
                }

                if (records.Count == 0)
                {
                    Console.WriteLine("There are no records, that fits the condition.");
                    return;
                }

                this.PrintTableRecords(records, parametersToPrint);
            }
            catch (ArgumentException argumentException)
            {
                Console.WriteLine(argumentException.Message);
            }
        }

        private void PrintTableRecords(List<FileCabinetRecord> records, string parameters)
        {
            string[] listOfParametersToPrint = parameters.Split(KeySetterSeparator, StringSplitOptions.TrimEntries);
            int[] cellLengthArray = this.AssignMinimalCellLengthArrayWithInitValues(listOfParametersToPrint);
            cellLengthArray = this.AssignMinimalCellLengthArrayWithRecordValues(records, cellLengthArray, listOfParametersToPrint);
            string output = this.GetPrintString(cellLengthArray, listOfParametersToPrint, records);
            Console.WriteLine(output);
        }

        private string GetPrintString(int[] cellLengthArray, string[] parametersList, List<FileCabinetRecord> records)
        {
            StringBuilder sb = new StringBuilder();
            string line = this.GetPrintLine(cellLengthArray, parametersList);

            string header = this.GetPrintHeader(line, cellLengthArray, parametersList);
            sb.Append(header);

            for (int i = 0; i < records.Count; i++)
            {
                string bodyElement = this.GetPrintBodyElement(cellLengthArray, parametersList, records[i]);
                sb.AppendLine(bodyElement);
            }

            sb.AppendLine(line);

            return sb.ToString();
        }

        private string GetPrintBodyElement(int[] cellLengthArray, string[] parametersList, FileCabinetRecord record)
        {
            var cellFill = this.GetBodyCellStringArrayToJoin(record, cellLengthArray, parametersList);

            var middleBodyCell = string.Join(CellWall, cellFill);
            middleBodyCell = string.Concat(CellWall, middleBodyCell, CellWall);

            return middleBodyCell;
        }

        private string GetPrintLine(int[] cellLengthArray, string[] parametersList)
        {
            string[] lines = this.GetLinesArray(cellLengthArray, parametersList.Length);

            var line = string.Join(CellCross, lines);
            line = string.Concat(CellCross, line, CellCross);

            return line;
        }

        private string GetPrintHeader(string line, int[] cellLengthArray, string[] parametersList)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(line);

            string[] headerFill = this.GetHeaderCellStringArrayToJoin(cellLengthArray, parametersList);

            var middle = string.Join(CellWall, headerFill);
            middle = string.Concat(CellWall, middle, CellWall);

            sb.AppendLine(middle);
            sb.AppendLine(line);

            return sb.ToString();
        }

        private string[] GetHeaderCellStringArrayToJoin(int[] cellLengthArray, string[] parametersList)
        {
            string[] result = new string[parametersList.Length];

            for (int i = 0; i < result.Length; i++)
            {
                int amountWhiteSpaces = cellLengthArray[i] - parametersList[i].Length;
                string currentString = string.Empty.PadLeft(amountWhiteSpaces, ' ');
                currentString = currentString.Insert(parametersList[i].Equals(nameof(FileCabinetRecord.Id), StringComparison.OrdinalIgnoreCase) ? amountWhiteSpaces - 1 : 1, parametersList[i]);
                result[i] = currentString;
            }

            return result;
        }

        private string[] GetBodyCellStringArrayToJoin(FileCabinetRecord record, int[] cellLengthArray, string[] parametersList)
        {
            string[] result = new string[parametersList.Length];

            for (int i = 0; i < parametersList.Length; i++)
            {
                string currentParameter = parametersList[i];

                if (currentParameter.Equals(nameof(FileCabinetRecord.Id), StringComparison.OrdinalIgnoreCase))
                {
                    string id = record.Id.ToString();
                    result[i] = this.GetStringParameterInBodyCell(cellLengthArray[i], id, true);
                }
                else if (currentParameter.Equals(nameof(FileCabinetRecord.FirstName), StringComparison.OrdinalIgnoreCase))
                {
                    string firstName = record.FirstName;
                    result[i] = this.GetStringParameterInBodyCell(cellLengthArray[i], firstName, false);
                }
                else if (currentParameter.Equals(nameof(FileCabinetRecord.LastName), StringComparison.OrdinalIgnoreCase))
                {
                    string lastName = record.LastName;
                    result[i] = this.GetStringParameterInBodyCell(cellLengthArray[i], lastName, false);
                }
                else if (currentParameter.Equals(nameof(FileCabinetRecord.DateOfBirth), StringComparison.OrdinalIgnoreCase))
                {
                    string birth = record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
                    result[i] = this.GetStringParameterInBodyCell(cellLengthArray[i], birth, false);
                }
                else
                {
                    throw new ArgumentException($"There is no such parameter as '{currentParameter}'. Please, try again.");
                }
            }

            return result;
        }

        private string GetStringParameterInBodyCell(int length, string parameter, bool isId)
        {
            int amountWhiteSpaces = length - parameter.Length;
            int insertPosition = isId ? amountWhiteSpaces - 1 : 1;
            string currentString = string.Empty.PadLeft(amountWhiteSpaces, ' ').Insert(insertPosition, parameter);
            return currentString;
        }

        private string[] GetLinesArray(int[] cellLengthArray, int amountOfLines)
        {
            string[] lines = new string[amountOfLines];

            for (int i = 0; i < amountOfLines; i++)
            {
                lines[i] = string.Empty.PadLeft(cellLengthArray[i], CellLine);
            }

            return lines;
        }

        private int[] AssignMinimalCellLengthArrayWithRecordValues(List<FileCabinetRecord> records, int[] minimalCellLengthArray, string[] listOfParametersToPrint)
        {
            for (int j = 0; j < records.Count; j++)
            {
                for (int i = 0; i < listOfParametersToPrint.Length; i++)
                {
                    string parameterName = listOfParametersToPrint[i];

                    if (parameterName.Equals(nameof(FileCabinetRecord.Id), StringComparison.OrdinalIgnoreCase))
                    {
                        string id = records[j].Id.ToString();
                        minimalCellLengthArray[i] = this.GetCellLength(id, minimalCellLengthArray[i]);
                    }
                    else if (parameterName.Equals(nameof(FileCabinetRecord.FirstName), StringComparison.OrdinalIgnoreCase))
                    {
                        string firstName = records[j].FirstName;
                        minimalCellLengthArray[i] = this.GetCellLength(firstName, minimalCellLengthArray[i]);
                    }
                    else if (parameterName.Equals(nameof(FileCabinetRecord.LastName), StringComparison.OrdinalIgnoreCase))
                    {
                        string lastName = records[j].LastName;
                        minimalCellLengthArray[i] = this.GetCellLength(lastName, minimalCellLengthArray[i]);
                    }
                    else if (parameterName.Equals(nameof(FileCabinetRecord.DateOfBirth), StringComparison.OrdinalIgnoreCase))
                    {
                        string birth = records[j].DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
                        minimalCellLengthArray[i] = this.GetCellLength(birth, minimalCellLengthArray[i]);
                    }
                    else
                    {
                        throw new ArgumentException($"There is no such parameter as '{parameterName}'. Please, try again.");
                    }
                }
            }

            return minimalCellLengthArray;
        }

        private int GetCellLength(string parameter, int minimalLength)
        {
            int possibleLength = parameter.Length + 2;

            if (possibleLength > minimalLength)
            {
                minimalLength = possibleLength;
            }

            return minimalLength;
        }

        private int[] AssignMinimalCellLengthArrayWithInitValues(string[] listOfParametersToPrint)
        {
            int[] minimalCellLength = new int[listOfParametersToPrint.Length];

            for (int i = 0; i < listOfParametersToPrint.Length; i++)
            {
                int currentCellLength = listOfParametersToPrint[i].Length;
                minimalCellLength[i] = currentCellLength + AdditionalSpaceInCell;
            }

            return minimalCellLength;
        }

        private List<FileCabinetRecord> GetFilteredRecords(List<FileCabinetRecord> records, string keyWord, List<Tuple<string, string>> parameters)
        {
            List<FileCabinetRecord> filteredRecords = new List<FileCabinetRecord>();

            switch (keyWord)
            {
                case KeyAndWord:
                    filteredRecords = this.GetByImplementingAndCondition(records, parameters);
                    break;
                case KeyOrWord:
                    filteredRecords = records;
                    break;
                default:
                    break;
            }

            return filteredRecords;
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

        private List<FileCabinetRecord> GetByImplementingAndCondition(List<FileCabinetRecord> records, List<Tuple<string, string>> paramsToSearch)
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
                        int id = this.GetParsedAndValidatedParameter<int>(value, InputConverter.IdConverter, Program.InputValidator.IdValidator);

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
                        string firstName = this.GetParsedAndValidatedParameter<string>(value, InputConverter.StringConverter, Program.InputValidator.FirstNameValidator);

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
                        string lastName = this.GetParsedAndValidatedParameter<string>(value, InputConverter.StringConverter, Program.InputValidator.LastNameValidator);

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
                        DateTime birthName = this.GetParsedAndValidatedParameter<DateTime>(value, InputConverter.BirthDateConverter, Program.InputValidator.DateOfBirthValidator);

                        if (DateTime.Compare(birthName, records[i].DateOfBirth) == 0)
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
                else
                {
                    throw new ArgumentException($"There is no such parameter as '{parameterName}'. Please, try again.");
                }
            }

            return records;
        }

        private List<Tuple<string, string>> GetParametersByNameAndValue(string input, string separator)
        {
            List<Tuple<string, string>> setOfParameters = new List<Tuple<string, string>>();

            string[] splitedByAndDivider = input.Split(separator, StringSplitOptions.TrimEntries);

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
                throw new ArgumentException($"There are no parameters in 'where'. Please, try again!");
            }

            return setOfParameters;
        }
    }
}
