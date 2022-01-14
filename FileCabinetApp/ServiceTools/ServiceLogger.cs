using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace FileCabinetApp.ServiceTools
{
    /// <summary>
    /// Add call stack logs to file.
    /// </summary>
    public class ServiceLogger : IFileCabinetService
    {
        private const string PathToLogFile = "log.txt";
        private IFileCabinetService service;
        private TextWriter writer = File.CreateText(PathToLogFile);

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="service">Service to watch for the call stack.</param>
        public ServiceLogger(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <inheritdoc/>
        public bool CheckRecordPresence(int id)
        {
            this.writer.WriteLine($"{this.GetLogTime()} - Calling CheckRecordPresence() with id = '{id}'.");
            var value = this.service.CheckRecordPresence(id);

            this.writer.Flush();

            return value;
        }

        /// <inheritdoc/>
        public void CreateRecord(FileCabinetRecord record)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{this.GetLogTime()} - Calling CreateRecord() with ");
            sb.Append($"FirstName = '{record.FirstName}', ");
            sb.Append($"LastName = '{record.LastName}', ");
            sb.Append($"DateOfBirth = '{record.DateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}', ");
            sb.Append($"PersonalRating = '{record.PersonalRating}', ");
            sb.Append($"Salary = '{record.Salary}', ");
            sb.Append($"Gender = '{record.Gender}'.");
            sb.Append(Environment.NewLine);

            this.service.CreateRecord(record);

            this.writer.WriteLine(sb.ToString());
            this.writer.Flush();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByBirthDate(string birthDate)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{this.GetLogTime()} - Calling FindByBirthDate() with DateOfBirth = {birthDate}.");

            var records = this.service.FindByBirthDate(birthDate);

            this.writer.WriteLine(sb.ToString());
            this.writer.Flush();

            return records;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{this.GetLogTime()} - Calling FindByFirstName() with FirstName = {firstName}.");

            var records = this.service.FindByFirstName(firstName);

            this.writer.WriteLine(sb.ToString());
            this.writer.Flush();

            return records;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{this.GetLogTime()} - Calling FindByLastName() with LastName = {lastName}.");

            var records = this.service.FindByLastName(lastName);

            this.writer.WriteLine(sb.ToString());
            this.writer.Flush();

            return records;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByPersonalRating(string personalRating)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{this.GetLogTime()} - Calling FindByLastName() with personalRating = {personalRating}.");

            var records = this.service.FindByPersonalRating(personalRating);

            this.writer.WriteLine(sb.ToString());
            this.writer.Flush();

            return records;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindBySalary(string salary)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{this.GetLogTime()} - Calling FindByLastName() with salary = {salary}.");

            var records = this.service.FindBySalary(salary);

            this.writer.WriteLine(sb.ToString());
            this.writer.Flush();

            return records;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByGender(string gender)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{this.GetLogTime()} - Calling FindByLastName() with gender = {gender}.");

            var records = this.service.FindByGender(gender);

            this.writer.WriteLine(sb.ToString());
            this.writer.Flush();

            return records;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{this.GetLogTime()} - Calling GetRecords().");

            var records = this.service.GetRecords();

            sb.Append($"{this.GetLogTime()} - GetRecords() returns {records.Count} record(s).");

            this.writer.WriteLine(sb.ToString());
            this.writer.Flush();

            return records;
        }

        /// <inheritdoc/>
        public void GetStat()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{this.GetLogTime()} - Calling GetStat().");

            this.service.GetStat();

            this.writer.WriteLine(sb.ToString());
            this.writer.Flush();
        }

        /// <inheritdoc/>
        public void InsertRecord(FileCabinetRecord record)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{this.GetLogTime()} - Calling InsertRecord() with ");
            sb.Append($"FirstName = '{record.FirstName}', ");
            sb.Append($"LastName = '{record.LastName}', ");
            sb.Append($"DateOfBirth = '{record.DateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}', ");
            sb.Append($"PersonalRating = '{record.PersonalRating}', ");
            sb.Append($"Salary = '{record.Salary}', ");
            sb.Append($"Gender = '{record.Gender}'.");
            sb.Append(Environment.NewLine);

            this.service.InsertRecord(record);

            this.writer.WriteLine(sb.ToString());
            this.writer.Flush();
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{this.GetLogTime()} - Calling MakeSnapshot().");

            var snapshot = this.service.MakeSnapshot();

            sb.Append($"{this.GetLogTime()} - MakeSnapshot() returns snapshot.");

            this.writer.WriteLine(sb.ToString());
            this.writer.Flush();

            return snapshot;
        }

        /// <inheritdoc/>
        public void Purge()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{this.GetLogTime()} - Calling Purge().");

            this.service.Purge();

            this.writer.WriteLine(sb.ToString());
            this.writer.Flush();
        }

        /// <inheritdoc/>
        public void Delete(List<int> ids)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{this.GetLogTime()} - Calling Delete().");

            this.service.Delete(ids);

            sb.Append($"{this.GetLogTime()} - Delete() removes next record(s) with id:");
            sb.Append(string.Join(", ", ids));
            sb.Append($".");

            this.writer.WriteLine(sb.ToString());
            this.writer.Flush();
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{this.GetLogTime()} - Calling Restore() with snapshot.");

            this.service.Restore(fileCabinetServiceSnapshot);

            this.writer.WriteLine(sb.ToString());
            this.writer.Flush();
        }

        /// <inheritdoc/>
        public void Update(List<FileCabinetRecord> records)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{this.GetLogTime()} - Calling Update().");

            this.service.Update(records);

            sb.Append($"{this.GetLogTime()} - Update() edit {records.Count} record(s).");

            this.writer.WriteLine(sb.ToString());
            this.writer.Flush();
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetRecord(int id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{this.GetLogTime()} - Calling GetRecord() with id='{id}'.");

            var record = this.service.GetRecord(id);

            this.writer.WriteLine(sb.ToString());
            this.writer.Flush();

            return record;
        }

        /// <inheritdoc/>
        public List<FileCabinetRecord> Select(string phrase, string memoizingKey, IRecordInputValidator inputValidator)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{this.GetLogTime()} - Calling Select().");

            var records = this.service.Select(phrase, memoizingKey, inputValidator);

            sb.Append($"{this.GetLogTime()} - Select() returns {records.Count} record(s).");

            this.writer.WriteLine(sb.ToString());
            this.writer.Flush();

            return records;
        }

        private string GetLogTime()
        {
            return DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        }
    }
}
