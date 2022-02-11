using System.Collections.ObjectModel;
using FileCabinetApp.ConditionWords;
using FileCabinetApp.Interfaces;
using FileCabinetApp.ServiceTools;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace FileCabinetApp.Services
{
    /// <summary>
    /// Class that works with database by using entity framework.
    /// </summary>
    internal class FileCabinetEntityService : IFileCabinetService, IDisposable
    {
        private ApplicationContext context = new ApplicationContext();
        private IRecordValidator recordValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetEntityService"/> class.
        /// </summary>
        /// <param name="recordValidator">Validator for records.</param>
        internal FileCabinetEntityService(IRecordValidator recordValidator)
        {
            this.recordValidator = recordValidator;

            using (ApplicationContext context = new ApplicationContext()) // ensure created lasts more than 1s. 
            {
                Console.WriteLine("Entities initialized.");
            }
        }

        /// <inheritdoc/>
        public bool CheckRecordPresence(int id)
        {
            var record = this.context.FileCabinetRecords.FirstOrDefault(x => x.Id == id);

            return record is null ? false : true;
        }

        /// <inheritdoc/>
        public void CreateRecord(FileCabinetRecord record)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Delete(List<int> ids)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByBirthDate(string birthDate)
        {
            var records = this.context.FileCabinetRecords
                .Where(x => x.DateOfBirth.ToString("yyyy-MM-dd") == birthDate)
                .ToList();

            return new List<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            var records = this.context.FileCabinetRecords
                .Where(x => x.FirstName == firstName)
                .ToList();

            return new List<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByGender(string gender)
        {
            var records = this.context.FileCabinetRecords
                .Where(x => x.Gender.ToString() == gender)
                .ToList();

            return new List<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            var records = this.context.FileCabinetRecords
                .Where(x => x.LastName == lastName)
                .ToList();

            return new List<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByPersonalRating(string personalRating)
        {
            var records = this.context.FileCabinetRecords
                .Where(x => x.PersonalRating.ToString() == personalRating)
                .ToList();

            return new List<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindBySalary(string salary)
        {
            var records = this.context.FileCabinetRecords
                .Where(x => x.Salary.ToString() == salary)
                .ToList();

            return new List<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetRecord(int id)
        {
            // record is not null, due to previous check of record presence in stored records.
            var record = this.context.FileCabinetRecords
                .Where(x => x.Id == id)
                .First();

            return record;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> records = this.context.FileCabinetRecords.ToList();

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public void GetStat()
        {
            int recordsCount = this.context.FileCabinetRecords.Count();

            Console.WriteLine($"Stored {recordsCount} record(s).");
        }

        /// <inheritdoc/>
        public void InsertRecord(FileCabinetRecord record)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.GetRecords().ToList(), this.recordValidator);
        }

        /// <inheritdoc/>
        public void Purge()
        {
            Console.WriteLine("Wrong service to use. Please, choose another one.");
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public List<FileCabinetRecord> Select(string phrase, string memoizingKey, IRecordInputValidator inputValidator) => this.Memoized(memoizingKey, x =>
        {
            ConditionWhere where = new ConditionWhere(this, inputValidator);
            var records = where.GetFilteredRecords(phrase);

            return records;
        });

        /// <inheritdoc/>
        public void Update(List<FileCabinetRecord> records)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}
