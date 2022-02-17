using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.ConditionWords;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Iterators;
using FileCabinetApp.ServiceTools;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Class for work with noSQL mongoDB.
    /// </summary>
    internal class FileCabinetMongoService : IFileCabinetService
    {
        private IRecordValidator recordValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMongoService"/> class.
        /// </summary>
        /// <param name="recordValidator">Validator for records.</param>
        public FileCabinetMongoService(IRecordValidator recordValidator)
        {
            this.recordValidator = recordValidator;
        }

        /// <inheritdoc/>
        public bool CheckRecordPresence(int id)
        {
            var collection = MongoService.GetMongoCollection();

            bool isRecordInDatabase = collection
                .AsQueryable<FileCabinetRecord>()
                .Where(x => x.Id == id)
                .Count() > 0;

            return isRecordInDatabase;
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

                var collection = MongoService.GetMongoCollection();
                collection.InsertOne(record);

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

            var deleteResult = MongoService.GetMongoCollection().DeleteMany(x => ids.Contains(x.Id));

            Console.WriteLine($"Deleted {ids.Count} record(s).");
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByBirthDate(string birthDate) => this.Memoized(birthDate, x =>
        {
            bool isValid = DateTime.TryParse(birthDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth);

            if (!isValid)
            {
                return new RecordDatabaseEnumerable(new List<FileCabinetRecord>());
            }

            var collection = MongoService.GetMongoCollection();

            List<FileCabinetRecord> records = collection
                .AsQueryable<FileCabinetRecord>()
                .Where(x => CustomComparer.IsEqualDatesUpToDays(x.DateOfBirth, dateOfBirth))
                .ToList();

            return new RecordDatabaseEnumerable(records);
        });

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName) => this.Memoized(firstName, x =>
        {
            if (string.IsNullOrEmpty(firstName))
            {
                return new RecordDatabaseEnumerable(new List<FileCabinetRecord>());
            }

            var collection = MongoService.GetMongoCollection();

            List<FileCabinetRecord> records = collection
                .AsQueryable<FileCabinetRecord>()
                .Where(x => x.FirstName == firstName)
                .ToList();

            return new RecordDatabaseEnumerable(records);
        });

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByGender(string gender) => this.Memoized(gender, x =>
        {
            bool isValid = char.TryParse(gender, out char charGender);

            if (!isValid)
            {
                return new RecordDatabaseEnumerable(new List<FileCabinetRecord>());
            }

            var collection = MongoService.GetMongoCollection();

            List<FileCabinetRecord> records = collection
                .AsQueryable<FileCabinetRecord>()
                .Where(x => x.Gender == charGender)
                .ToList();

            return new RecordDatabaseEnumerable(records);
        });

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName) => this.Memoized(lastName, x =>
        {
            if (string.IsNullOrEmpty(lastName))
            {
                return new RecordDatabaseEnumerable(new List<FileCabinetRecord>());
            }

            var collection = MongoService.GetMongoCollection();

            List<FileCabinetRecord> records = collection
                .AsQueryable<FileCabinetRecord>()
                .Where(x => x.LastName == lastName)
                .ToList();

            return new RecordDatabaseEnumerable(records);
        });

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByPersonalRating(string personalRating) => this.Memoized(personalRating, x =>
        {
            bool isValid = short.TryParse(personalRating, out short shortPersonalRating);

            if (!isValid)
            {
                return new RecordDatabaseEnumerable(new List<FileCabinetRecord>());
            }

            var collection = MongoService.GetMongoCollection();

            List<FileCabinetRecord> records = collection
                .AsQueryable<FileCabinetRecord>()
                .Where(x => x.PersonalRating == shortPersonalRating)
                .ToList();

            return new RecordDatabaseEnumerable(records);
        });

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindBySalary(string salary) => this.Memoized(salary, x =>
        {
            bool isValid = decimal.TryParse(salary, out decimal decimalSalary);

            if (!isValid)
            {
                return new RecordDatabaseEnumerable(new List<FileCabinetRecord>());
            }

            var collection = MongoService.GetMongoCollection();

            List<FileCabinetRecord> records = collection
                .AsQueryable<FileCabinetRecord>()
                .Where(x => x.Salary == decimalSalary)
                .ToList();

            return new RecordDatabaseEnumerable(records);
        });

        /// <inheritdoc/>
        public FileCabinetRecord GetRecord(int id)
        {
            var collection = MongoService.GetMongoCollection();
            FileCabinetRecord record = collection.AsQueryable<FileCabinetRecord>().First(x => x.Id == id);
            return record;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            var records = MongoService.GetMongoCollection().AsQueryable<FileCabinetRecord>().ToList();

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public void GetStat()
        {
            var recordsCount = MongoService.GetMongoCollection().EstimatedDocumentCount();

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

                var collection = MongoService.GetMongoCollection();
                collection.InsertOne(record);

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

            var collection = MongoService.GetMongoCollection();
            collection.InsertMany(unloadRecords);
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

            var collection = MongoService.GetMongoCollection();

            foreach (var record in records)
            {
                collection.ReplaceOne(x => x.Id == record.Id, record);
            }

            Console.WriteLine($"Records updating completed.");
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
    }
}
