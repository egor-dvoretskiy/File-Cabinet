using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace FileCabinetApp
{
    /// <summary>
    /// Measures working time of service methods.
    /// </summary>
    public class ServiceMeter : IFileCabinetService
    {
        private IFileCabinetService service;
        private Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        /// <param name="service">Service to measure time.</param>
        public ServiceMeter(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <inheritdoc/>
        public bool CheckRecordPresence(int id)
        {
            this.stopwatch.Restart();
            var value = this.service.CheckRecordPresence(id);
            this.stopwatch.Stop();

            this.GetElapsedTime(this.GetCallerName());

            return value;
        }

        /// <inheritdoc/>
        public int CreateRecord(FileCabinetRecord record)
        {
            this.stopwatch.Restart();
            int id = this.service.CreateRecord(record);
            this.stopwatch.Stop();

            this.GetElapsedTime(this.GetCallerName());

            return id;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, FileCabinetRecord record)
        {
            this.stopwatch.Restart();
            this.service.EditRecord(id, record);
            this.stopwatch.Stop();

            this.GetElapsedTime(this.GetCallerName());
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByBirthDate(string birthDate)
        {
            this.stopwatch.Restart();
            var collection = this.service.FindByBirthDate(birthDate);
            this.stopwatch.Stop();

            this.GetElapsedTime(this.GetCallerName());

            return collection;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.stopwatch.Restart();
            var collection = this.service.FindByFirstName(firstName);
            this.stopwatch.Stop();

            this.GetElapsedTime(this.GetCallerName());

            return collection;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.stopwatch.Restart();
            var collection = this.service.FindByLastName(lastName);
            this.stopwatch.Stop();

            this.GetElapsedTime(this.GetCallerName());

            return collection;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.stopwatch.Restart();
            var records = this.service.GetRecords();
            this.stopwatch.Stop();

            this.GetElapsedTime(this.GetCallerName());

            return records;
        }

        /// <inheritdoc/>
        public void GetStat()
        {
            this.stopwatch.Restart();
            this.service.GetStat();
            this.stopwatch.Stop();

            this.GetElapsedTime(this.GetCallerName());
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            this.stopwatch.Restart();
            var snapshot = this.service.MakeSnapshot();
            this.stopwatch.Stop();

            this.GetElapsedTime(this.GetCallerName());

            return snapshot;
        }

        /// <inheritdoc/>
        public void Purge()
        {
            this.stopwatch.Restart();
            this.service.Purge();
            this.stopwatch.Stop();

            this.GetElapsedTime(this.GetCallerName());
        }

        /// <inheritdoc/>
        public void RemoveRecordById(int id)
        {
            this.stopwatch.Restart();
            this.service.RemoveRecordById(id);
            this.stopwatch.Stop();

            this.GetElapsedTime(this.GetCallerName());
        }

        /// <inheritdoc/>
        public void Delete(List<int> ids)
        {
            this.stopwatch.Restart();
            this.service.Delete(ids);
            this.stopwatch.Stop();

            this.GetElapsedTime(this.GetCallerName());
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot)
        {
            this.stopwatch.Restart();
            this.service.Restore(fileCabinetServiceSnapshot);
            this.stopwatch.Stop();

            this.GetElapsedTime(this.GetCallerName());
        }

        /// <inheritdoc/>
        public void InsertRecord(FileCabinetRecord record)
        {
            this.stopwatch.Restart();
            this.service.InsertRecord(record);
            this.stopwatch.Stop();

            this.GetElapsedTime(this.GetCallerName());
        }

        /// <inheritdoc/>
        public void Update(List<FileCabinetRecord> records)
        {
            this.stopwatch.Restart();
            this.service.Update(records);
            this.stopwatch.Stop();

            this.GetElapsedTime(this.GetCallerName());
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetRecord(int id)
        {
            this.stopwatch.Restart();
            var record = this.service.GetRecord(id);
            this.stopwatch.Stop();

            this.GetElapsedTime(this.GetCallerName());

            return record;
        }

        private void GetElapsedTime(string methodName)
        {
            var elapsed = this.stopwatch.Elapsed.TotalMilliseconds; // ms
            Console.WriteLine($"{methodName} method execution duration is {elapsed} ms.");
        }

        private string GetCallerName([CallerMemberName] string name = "")
        {
            return name;
        }
    }
}
