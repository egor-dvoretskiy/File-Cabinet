using System.Collections.ObjectModel;
using System.Globalization;
using FileCabinetApp.ConditionWords;
using FileCabinetApp.Interfaces;
using FileCabinetApp.ServiceTools;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace FileCabinetApp.Services
{
    /// <summary>
    /// Class that works with database by using entity framework.
    /// </summary>
    internal class FileCabinetEntityService : IFileCabinetService, IDisposable
    {
        private EntityService context = new EntityService();
        private IRecordValidator recordValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetEntityService"/> class.
        /// </summary>
        /// <param name="recordValidator">Validator for records.</param>
        internal FileCabinetEntityService(IRecordValidator recordValidator)
        {
            this.recordValidator = recordValidator;

            using (EntityService context = new EntityService()) // ensure created lasts more than 1s.
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
            MemoizerService.RefreshMemoizer();

            try
            {
                bool isValid = this.recordValidator.ValidateParameters(record);

                if (!isValid)
                {
                    Console.WriteLine($"Record validation failed.");
                    return;
                }

                record.Id = this.GetUniqueId();

                this.context.FileCabinetRecords.Add(record);
                this.context.SaveChanges();

                Console.WriteLine($"Record #{record.Id} is created.");
            }
            catch (ArgumentNullException argumentNullException)
            {
                Console.WriteLine(argumentNullException.Message);
            }
            catch (ArgumentOutOfRangeException argumentOutOfRangeException)
            {
                Console.WriteLine(argumentOutOfRangeException.Message);
            }
            catch (ArgumentException argumentException)
            {
                Console.WriteLine(argumentException.Message);
            }
        }

        /// <inheritdoc/>
        public void Delete(List<int> ids)
        {
            MemoizerService.RefreshMemoizer();

            var recordsToDelete = this.context.FileCabinetRecords
                .Where(x => ids.Contains(x.Id))
                .ToArray();

            this.context.FileCabinetRecords.RemoveRange(recordsToDelete);
            this.context.SaveChanges();

            Console.WriteLine($"Deleted {ids.Count} record(s).");
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByBirthDate(string birthDate)
        {
            bool isValid = DateTime.TryParse(birthDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth);

            if (!isValid)
            {
                return new List<FileCabinetRecord>();
            }

            var records = this.context.FileCabinetRecords
                .Where(x => CustomComparer.IsEqualDatesUpToDays(x.DateOfBirth, dateOfBirth))
                .ToList();

            return new List<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            var records = this.context.FileCabinetRecords
                .Where(x => x.FirstName == firstName)
                .ToList();

            /*SqlParameter parameter = new SqlParameter("@firstname", firstName);
            var records = this.context.FileCabinetRecords
                .FromSqlRaw($"SELECT * FROM {ServerCommunicator.TableName} WHERE FirstName=@firstname", parameter).ToList();*/

            return new List<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByGender(string gender)
        {
            var records = this.context.FileCabinetRecords
                .Where(x => x.Gender.ToString() == gender)
                .ToList();

            /*SqlParameter parameter = new SqlParameter("@gender", gender);
            var records = this.context.FileCabinetRecords
                .FromSqlRaw($"SELECT * FROM {ServerCommunicator.TableName} WHERE Gender=@gender", parameter).ToList();*/

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
            MemoizerService.RefreshMemoizer();

            try
            {
                bool isValid = this.recordValidator.ValidateParameters(record);

                if (!isValid)
                {
                    throw new ArgumentException("Record you want to add is not valid. Please try again!");
                }

                if (this.CheckRecordPresence(record.Id))
                {
                    throw new ArgumentException($"Memory is already has a record #{record.Id}.");
                }

                this.context.FileCabinetRecords.Add(record);
                this.context.SaveChanges();

                Console.WriteLine($"Record was successfully inserted in database");
            }
            catch (ArgumentNullException argumentNullException)
            {
                Console.WriteLine(argumentNullException.Message);
            }
            catch (ArgumentOutOfRangeException argumentOutOfRangeException)
            {
                Console.WriteLine(argumentOutOfRangeException.Message);
            }
            catch (ArgumentException argumentException)
            {
                Console.WriteLine(argumentException.Message);
            }
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
            var unloadRecords = fileCabinetServiceSnapshot.Records.ToList();

            this.context.AddRange(unloadRecords);
            this.context.SaveChanges();
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
            MemoizerService.RefreshMemoizer();

            foreach (var record in records)
            {
                var tableRecord = this.context.FileCabinetRecords.Single(x => x.Id == record.Id);
                this.AssignRecordToTable(ref tableRecord, record);
                this.context.FileCabinetRecords.Update(tableRecord);
            }

            this.context.SaveChanges();

            Console.WriteLine($"Records updating completed.");
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.context.Dispose();
        }

        private int GetUniqueId()
        {
            int id = 1;

            while (this.CheckRecordPresence(id))
            {
                id++;
            }

            return id;
        }

        private void AssignRecordToTable(ref FileCabinetRecord tableRecord, FileCabinetRecord record)
        {
            tableRecord.Id = record.Id;
            tableRecord.FirstName = record.FirstName;
            tableRecord.LastName = record.LastName;
            tableRecord.DateOfBirth = record.DateOfBirth;
            tableRecord.PersonalRating = record.PersonalRating;
            tableRecord.Salary = record.Salary;
            tableRecord.Gender = record.Gender;
        }
    }
}
