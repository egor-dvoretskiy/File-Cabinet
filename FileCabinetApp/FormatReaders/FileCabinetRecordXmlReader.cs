using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Validators;

#pragma warning disable CS8604 // Possible null reference argument.

namespace FileCabinetApp.FormatReaders
{
    /// <summary>
    /// Class provides Xml file reading.
    /// </summary>
    public class FileCabinetRecordXmlReader
    {
        private readonly StreamReader reader;
        private readonly IRecordValidator recordValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="reader">Stream reader.</param>
        /// <param name="validator">Record validator.</param>
        public FileCabinetRecordXmlReader(StreamReader reader, IRecordValidator validator)
        {
            this.reader = reader;
            this.recordValidator = validator;
        }

        /// <summary>
        /// Reads all data from xml file.
        /// </summary>
        /// <returns>List of data.</returns>
        public List<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(this.reader);

            var root = xmlDocument.FirstChild?.NextSibling?.ChildNodes;

            if (root is null)
            {
                return records;
            }

            for (int i = 0; i < root.Count; i++)
            {
                if (root[i] is null)
                {
                    continue;
                }

                FileCabinetRecord record = this.ParseNode(root[i]);

                bool isValid = this.recordValidator.ValidateParameters(record);

                if (isValid)
                {
                    records.Add(record);
                }
            }

            return records;
        }

        private FileCabinetRecord ParseNode(XmlNode node)
        {
            FileCabinetRecord record = new FileCabinetRecord();

            record.Id = this.GetIdValue(node.Attributes?[0].Value);
            record.FirstName = this.GetNameValue(node.ChildNodes?[0]?.Attributes?[0].Value);
            record.LastName = this.GetNameValue(node.ChildNodes?[0]?.Attributes?[1].Value);
            record.DateOfBirth = this.GetBirthDate(node.ChildNodes?[1]?.InnerText);
            record.PersonalRating = this.GetPersonalRating(node.ChildNodes?[2]?.InnerText);
            record.Salary = this.GetSalary(node.ChildNodes?[3]?.InnerText);
            record.Gender = this.GetGender(node.ChildNodes?[4]?.InnerText);

            return record;
        }

        private int GetIdValue(string stringId)
        {
            int id = -1;

            if (!string.IsNullOrEmpty(stringId))
            {
                id = int.Parse(stringId);
            }

            return id;
        }

        private string GetNameValue(string xmlName)
        {
            string name = string.Empty;

            if (!string.IsNullOrEmpty(xmlName))
            {
                name = xmlName;
            }

            return name;
        }

        private DateTime GetBirthDate(string stringDate)
        {
            DateTime date = DateTime.Now;

            if (!string.IsNullOrEmpty(stringDate))
            {
                date = DateTime.Parse(stringDate);
            }

            return date;
        }

        private short GetPersonalRating(string stringPersonalRating)
        {
            short rating = -1;

            if (!string.IsNullOrEmpty(stringPersonalRating))
            {
                rating = short.Parse(stringPersonalRating);
            }

            return rating;
        }

        private decimal GetSalary(string stringSalary)
        {
            decimal salary = -1;

            if (!string.IsNullOrEmpty(stringSalary))
            {
                salary = decimal.Parse(stringSalary);
            }

            return salary;
        }

        private char GetGender(string stringGender)
        {
            char gender = ' ';

            if (!string.IsNullOrEmpty(stringGender))
            {
                gender = char.Parse(stringGender);
            }

            return gender;
        }
    }
}
