using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
        public void CheckRecordPresence(int id)
        {
            this.stopwatch.Restart();
            this.service.CheckRecordPresence(id);
            this.stopwatch.Stop();

            var elapsed = this.stopwatch.ElapsedTicks;
            Console.WriteLine($"CheckRecordPresence method execution duration is {elapsed} ticks.");
        }

        /// <inheritdoc/>
        public int CreateRecord(FileCabinetRecord record)
        {
            this.stopwatch.Restart();
            int id = this.service.CreateRecord(record);
            this.stopwatch.Stop();

            var elapsed = this.stopwatch.ElapsedTicks;
            Console.WriteLine($"Create method execution duration is {elapsed} ticks.");

            return id;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, FileCabinetRecord record)
        {
            this.stopwatch.Restart();
            this.service.EditRecord(id, record);
            this.stopwatch.Stop();

            var elapsed = this.stopwatch.ElapsedTicks;
            Console.WriteLine($"Edit method execution duration is {elapsed} ticks.");
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByBirthDate(string birthDate)
        {
            this.stopwatch.Restart();
            var collection = this.service.FindByBirthDate(birthDate);
            this.stopwatch.Stop();

            var elapsed = this.stopwatch.ElapsedTicks;
            Console.WriteLine($"FindByBirthDate method execution duration is {elapsed} ticks.");

            return collection;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.stopwatch.Restart();
            var collection = this.service.FindByFirstName(firstName);
            this.stopwatch.Stop();

            var elapsed = this.stopwatch.ElapsedTicks;
            Console.WriteLine($"FindByFirstName method execution duration is {elapsed} ticks.");

            return collection;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.stopwatch.Restart();
            var collection = this.service.FindByLastName(lastName);
            this.stopwatch.Stop();

            var elapsed = this.stopwatch.ElapsedTicks;
            Console.WriteLine($"FindByLastName method execution duration is {elapsed} ticks.");

            return collection;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.stopwatch.Restart();
            var records = this.service.GetRecords();
            this.stopwatch.Stop();

            var elapsed = this.stopwatch.ElapsedTicks;
            Console.WriteLine($"GetRecords method execution duration is {elapsed} ticks.");

            return records;
        }

        /// <inheritdoc/>
        public void GetStat()
        {
            this.stopwatch.Restart();
            this.service.GetStat();
            this.stopwatch.Stop();

            var elapsed = this.stopwatch.ElapsedTicks;
            Console.WriteLine($"GetStat method execution duration is {elapsed} ticks.");
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            this.stopwatch.Restart();
            var snapshot = this.service.MakeSnapshot();
            this.stopwatch.Stop();

            var elapsed = this.stopwatch.ElapsedTicks;
            Console.WriteLine($"MakeSnapshot method execution duration is {elapsed} ticks.");

            return snapshot;
        }

        /// <inheritdoc/>
        public void Purge()
        {
            this.stopwatch.Restart();
            this.service.Purge();
            this.stopwatch.Stop();

            var elapsed = this.stopwatch.ElapsedTicks;
            Console.WriteLine($"Purge method execution duration is {elapsed} ticks.");
        }

        /// <inheritdoc/>
        public void RemoveRecordById(int id)
        {
            this.stopwatch.Restart();
            this.service.RemoveRecordById(id);
            this.stopwatch.Stop();

            var elapsed = this.stopwatch.ElapsedTicks;
            Console.WriteLine($"RemoveRecordById method execution duration is {elapsed} ticks.");
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot)
        {
            this.stopwatch.Restart();
            this.service.Restore(fileCabinetServiceSnapshot);
            this.stopwatch.Stop();

            var elapsed = this.stopwatch.ElapsedTicks;
            Console.WriteLine($"Restore method execution duration is {elapsed} ticks.");
        }
    }
}
