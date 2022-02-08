using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.ServiceTools
{
    /// <summary>
    /// Prints records with table-view in console.
    /// </summary>
    internal static class TablePrinter
    {
        private const char CellLine = '-';
        private const char CellCross = '+';
        private const char CellWall = '|';
        private const string KeySetterSeparator = ",";

        private const int AdditionalSpaceInCell = 2;

        /// <summary>
        /// Prints records with table-view in console.
        /// </summary>
        /// <param name="records">Records to print.</param>
        /// <param name="parameters">Params to show in console.</param>
        internal static void PrintTableRecords(List<FileCabinetRecord> records, string parameters)
        {
            string[] listOfParametersToPrint = parameters.Split(KeySetterSeparator, StringSplitOptions.TrimEntries);
            int[] cellLengthArray = TablePrinter.AssignMinimalCellLengthArrayWithInitValues(listOfParametersToPrint);
            cellLengthArray = TablePrinter.AssignMinimalCellLengthArrayWithRecordValues(records, cellLengthArray, listOfParametersToPrint);
            string output = TablePrinter.GetPrintString(cellLengthArray, listOfParametersToPrint, records);
            Console.WriteLine(output);
            Console.WriteLine($"{records.Count} records displayed");
        }

        private static string GetPrintString(int[] cellLengthArray, string[] parametersList, List<FileCabinetRecord> records)
        {
            StringBuilder sb = new StringBuilder();
            string line = TablePrinter.GetPrintLine(cellLengthArray, parametersList);

            string header = TablePrinter.GetPrintHeader(line, cellLengthArray, parametersList);
            sb.Append(header);

            for (int i = 0; i < records.Count; i++)
            {
                string bodyElement = TablePrinter.GetPrintBodyElement(cellLengthArray, parametersList, records[i]);
                sb.AppendLine(bodyElement);
            }

            sb.AppendLine(line);

            return sb.ToString();
        }

        private static string GetPrintBodyElement(int[] cellLengthArray, string[] parametersList, FileCabinetRecord record)
        {
            var cellFill = TablePrinter.GetBodyCellStringArrayToJoin(record, cellLengthArray, parametersList);

            var middleBodyCell = string.Join(CellWall, cellFill);
            middleBodyCell = string.Concat(CellWall, middleBodyCell, CellWall);

            return middleBodyCell;
        }

        private static string GetPrintLine(int[] cellLengthArray, string[] parametersList)
        {
            string[] lines = TablePrinter.GetLinesArray(cellLengthArray, parametersList.Length);

            var line = string.Join(CellCross, lines);
            line = string.Concat(CellCross, line, CellCross);

            return line;
        }

        private static string GetPrintHeader(string line, int[] cellLengthArray, string[] parametersList)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(line);

            string[] headerFill = TablePrinter.GetHeaderCellStringArrayToJoin(cellLengthArray, parametersList);

            var middle = string.Join(CellWall, headerFill);
            middle = string.Concat(CellWall, middle, CellWall);

            sb.AppendLine(middle);
            sb.AppendLine(line);

            return sb.ToString();
        }

        private static string[] GetHeaderCellStringArrayToJoin(int[] cellLengthArray, string[] parametersList)
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

        private static string[] GetBodyCellStringArrayToJoin(FileCabinetRecord record, int[] cellLengthArray, string[] parametersList)
        {
            string[] result = new string[parametersList.Length];

            for (int i = 0; i < parametersList.Length; i++)
            {
                string currentParameter = parametersList[i];

                if (currentParameter.Equals(nameof(FileCabinetRecord.Id), StringComparison.OrdinalIgnoreCase))
                {
                    string id = record.Id.ToString();
                    result[i] = TablePrinter.GetStringParameterInBodyCell(cellLengthArray[i], id, true);
                }
                else if (currentParameter.Equals(nameof(FileCabinetRecord.FirstName), StringComparison.OrdinalIgnoreCase))
                {
                    string firstName = record.FirstName;
                    result[i] = TablePrinter.GetStringParameterInBodyCell(cellLengthArray[i], firstName, false);
                }
                else if (currentParameter.Equals(nameof(FileCabinetRecord.LastName), StringComparison.OrdinalIgnoreCase))
                {
                    string lastName = record.LastName;
                    result[i] = TablePrinter.GetStringParameterInBodyCell(cellLengthArray[i], lastName, false);
                }
                else if (currentParameter.Equals(nameof(FileCabinetRecord.DateOfBirth), StringComparison.OrdinalIgnoreCase))
                {
                    string birth = record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
                    result[i] = TablePrinter.GetStringParameterInBodyCell(cellLengthArray[i], birth, false);
                }
                else if (currentParameter.Equals(nameof(FileCabinetRecord.PersonalRating), StringComparison.OrdinalIgnoreCase))
                {
                    string personalRating = record.PersonalRating.ToString();
                    result[i] = TablePrinter.GetStringParameterInBodyCell(cellLengthArray[i], personalRating, false);
                }
                else if (currentParameter.Equals(nameof(FileCabinetRecord.Salary), StringComparison.OrdinalIgnoreCase))
                {
                    string salary = record.Salary.ToString();
                    result[i] = TablePrinter.GetStringParameterInBodyCell(cellLengthArray[i], salary, false);
                }
                else if (currentParameter.Equals(nameof(FileCabinetRecord.Gender), StringComparison.OrdinalIgnoreCase))
                {
                    string gender = record.Gender.ToString();
                    result[i] = TablePrinter.GetStringParameterInBodyCell(cellLengthArray[i], gender, false);
                }
                else
                {
                    throw new ArgumentException($"There is no such parameter as '{currentParameter}'. Please, try again.");
                }
            }

            return result;
        }

        private static string GetStringParameterInBodyCell(int length, string parameter, bool isId)
        {
            int amountWhiteSpaces = length - parameter.Length;
            int insertPosition = isId ? amountWhiteSpaces - 1 : 1;
            string currentString = string.Empty.PadLeft(amountWhiteSpaces, ' ').Insert(insertPosition, parameter);
            return currentString;
        }

        private static string[] GetLinesArray(int[] cellLengthArray, int amountOfLines)
        {
            string[] lines = new string[amountOfLines];

            for (int i = 0; i < amountOfLines; i++)
            {
                lines[i] = string.Empty.PadLeft(cellLengthArray[i], CellLine);
            }

            return lines;
        }

        private static int[] AssignMinimalCellLengthArrayWithRecordValues(List<FileCabinetRecord> records, int[] minimalCellLengthArray, string[] listOfParametersToPrint)
        {
            for (int j = 0; j < records.Count; j++)
            {
                for (int i = 0; i < listOfParametersToPrint.Length; i++)
                {
                    string parameterName = listOfParametersToPrint[i];

                    if (parameterName.Equals(nameof(FileCabinetRecord.Id), StringComparison.OrdinalIgnoreCase))
                    {
                        string id = records[j].Id.ToString();
                        minimalCellLengthArray[i] = TablePrinter.GetCellLength(id, minimalCellLengthArray[i]);
                    }
                    else if (parameterName.Equals(nameof(FileCabinetRecord.FirstName), StringComparison.OrdinalIgnoreCase))
                    {
                        string firstName = records[j].FirstName;
                        minimalCellLengthArray[i] = TablePrinter.GetCellLength(firstName, minimalCellLengthArray[i]);
                    }
                    else if (parameterName.Equals(nameof(FileCabinetRecord.LastName), StringComparison.OrdinalIgnoreCase))
                    {
                        string lastName = records[j].LastName;
                        minimalCellLengthArray[i] = TablePrinter.GetCellLength(lastName, minimalCellLengthArray[i]);
                    }
                    else if (parameterName.Equals(nameof(FileCabinetRecord.DateOfBirth), StringComparison.OrdinalIgnoreCase))
                    {
                        string birth = records[j].DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
                        minimalCellLengthArray[i] = TablePrinter.GetCellLength(birth, minimalCellLengthArray[i]);
                    }
                    else if (parameterName.Equals(nameof(FileCabinetRecord.PersonalRating), StringComparison.OrdinalIgnoreCase))
                    {
                        string personalRating = records[j].PersonalRating.ToString();
                        minimalCellLengthArray[i] = TablePrinter.GetCellLength(personalRating, minimalCellLengthArray[i]);
                    }
                    else if (parameterName.Equals(nameof(FileCabinetRecord.Salary), StringComparison.OrdinalIgnoreCase))
                    {
                        string salary = records[j].Salary.ToString();
                        minimalCellLengthArray[i] = TablePrinter.GetCellLength(salary, minimalCellLengthArray[i]);
                    }
                    else if (parameterName.Equals(nameof(FileCabinetRecord.Gender), StringComparison.OrdinalIgnoreCase))
                    {
                        string gender = records[j].Gender.ToString();
                        minimalCellLengthArray[i] = TablePrinter.GetCellLength(gender, minimalCellLengthArray[i]);
                    }
                    else
                    {
                        throw new ArgumentException($"There is no such parameter as '{parameterName}'. Please, try again.");
                    }
                }
            }

            return minimalCellLengthArray;
        }

        private static int GetCellLength(string parameter, int minimalLength)
        {
            int possibleLength = parameter.Length + 2;

            if (possibleLength > minimalLength)
            {
                minimalLength = possibleLength;
            }

            return minimalLength;
        }

        private static int[] AssignMinimalCellLengthArrayWithInitValues(string[] listOfParametersToPrint)
        {
            int[] minimalCellLength = new int[listOfParametersToPrint.Length];

            for (int i = 0; i < listOfParametersToPrint.Length; i++)
            {
                int currentCellLength = listOfParametersToPrint[i].Length;
                minimalCellLength[i] = currentCellLength + AdditionalSpaceInCell;
            }

            return minimalCellLength;
        }
    }
}
