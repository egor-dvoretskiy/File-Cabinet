﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace FileCabinetApp
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
        public void CheckRecordPresence(int id)
        {
            this.writer.WriteLine($"{this.GetLogTime()} - Calling CheckRecordPresence() with id = '{id}'.");
            this.service.CheckRecordPresence(id);

            this.writer.Flush();
        }

        /// <inheritdoc/>
        public int CreateRecord(FileCabinetRecord record)
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

            int id = this.service.CreateRecord(record);

            sb.Append($"{this.GetLogTime()} - CreateRecord() returns '{id}'.");

            this.writer.WriteLine(sb.ToString());
            this.writer.Flush();

            return id;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, FileCabinetRecord record)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{this.GetLogTime()} - Calling EditRecord() with ");
            sb.Append($"FirstName = '{record.FirstName}', ");
            sb.Append($"LastName = '{record.LastName}', ");
            sb.Append($"DateOfBirth = '{record.DateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}', ");
            sb.Append($"PersonalRating = '{record.PersonalRating}', ");
            sb.Append($"Salary = '{record.Salary}', ");
            sb.Append($"Gender = '{record.Gender}'.");

            this.service.EditRecord(id, record);

            this.writer.WriteLine(sb.ToString());
            this.writer.Flush();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByBirthDate(string birthDate)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{this.GetLogTime()} - Calling FindByBirthDate() with DateOfBirth = {birthDate}.");

            var records = this.service.FindByBirthDate(birthDate);

            sb.Append($"{this.GetLogTime()} - FindByBirthDate() returns {records.Count} record(s).");

            this.writer.WriteLine(sb.ToString());
            this.writer.Flush();

            return records;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{this.GetLogTime()} - Calling FindByFirstName() with FirstName = {firstName}.");

            var records = this.service.FindByFirstName(firstName);

            sb.Append($"{this.GetLogTime()} - FindByFirstName() returns {records.Count} record(s).");

            this.writer.WriteLine(sb.ToString());
            this.writer.Flush();

            return records;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{this.GetLogTime()} - Calling FindByLastName() with LastName = {lastName}.");

            var records = this.service.FindByLastName(lastName);

            sb.Append($"{this.GetLogTime()} - FindByLastName() returns {records.Count} record(s).");

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
        public void RemoveRecordById(int id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{this.GetLogTime()} - Calling RemoveRecordById() with id = '{id}'.");

            this.service.RemoveRecordById(id);

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

        private string GetLogTime()
        {
            return DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        }
    }
}