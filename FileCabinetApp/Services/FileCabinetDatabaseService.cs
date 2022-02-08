using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.ConditionWords;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Iterators;
using FileCabinetApp.ServiceTools;
using Microsoft.Data.SqlClient;

#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
namespace FileCabinetApp.Services
{
    /// <summary>
    /// Database service class.
    /// </summary>
    internal class FileCabinetDatabaseService : FileCabinetDictionary, IFileCabinetService
    {
        private readonly IRecordValidator recordValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetDatabaseService"/> class.
        /// </summary>
        /// <param name="recordValidator">Validator for record.</param>
        public FileCabinetDatabaseService(IRecordValidator recordValidator)
        {
            this.recordValidator = recordValidator;

            ServerCommunicator.CheckTablePresenceInDatabase();
        }

        /// <inheritdoc/>
        public bool CheckRecordPresence(int id)
        {
            ServerCommunicator.OpenServerConnection();

            SqlCommand command = new SqlCommand();
            command.CommandText = $"SELECT * FROM {ServerCommunicator.TableName} WHERE Id={id}";
            command.Connection = ServerCommunicator.ServerConnection;

            var reader = command.ExecuteReader();
            bool hasRecords = reader.HasRows;

            ServerCommunicator.CloseServerConnection();

            return hasRecords;
        }

        /// <inheritdoc/>
        public void CreateRecord(FileCabinetRecord record)
        {
            try
            {
                bool isValid = this.recordValidator.ValidateParameters(record);

                if (!isValid)
                {
                    Console.WriteLine($"Record validation failed.");
                    return;
                }

                record.Id = this.GetUniqueId();

                ServerCommunicator.OpenServerConnection();
                SqlCommand command = new SqlCommand();
                command.Connection = ServerCommunicator.ServerConnection;
                command.CommandText = this.GetInsertCommandWithRecord(record);
                _ = command.ExecuteNonQuery();
                ServerCommunicator.CloseServerConnection();

                int position = this.GetRecordsCount() - 1;

                this.AddRecordToDictionaries(record, position);

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

            ServerCommunicator.OpenServerConnection();

            for (int i = 0; i < ids.Count; i++)
            {
                SqlCommand command = new SqlCommand();
                command.CommandText = $"DELETE FROM {ServerCommunicator.TableName} WHERE Id={ids[i]}";
                command.Connection = ServerCommunicator.ServerConnection;
                _ = command.ExecuteNonQuery();
            }

            ServerCommunicator.CloseServerConnection();

            Console.WriteLine($"Deleted {ids.Count} record(s).");
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByBirthDate(string birthDate)
        {
            string command = $"SELECT * FROM {ServerCommunicator.TableName} WHERE DateOfBirth='{birthDate}'";
            var records = this.FindBySqlCommand(command);

            return new RecordDatabaseEnumerable(records);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            string command = $"SELECT * FROM {ServerCommunicator.TableName} WHERE FirstName='{firstName}'";
            var records = this.FindBySqlCommand(command);

            return new RecordDatabaseEnumerable(records);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByGender(string gender)
        {
            string command = $"SELECT * FROM {ServerCommunicator.TableName} WHERE Gender='{gender}'";
            var records = this.FindBySqlCommand(command);

            return new RecordDatabaseEnumerable(records);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            string command = $"SELECT * FROM {ServerCommunicator.TableName} WHERE LastName='{lastName}'";
            var records = this.FindBySqlCommand(command);

            return new RecordDatabaseEnumerable(records);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByPersonalRating(string personalRating)
        {
            string command = $"SELECT * FROM {ServerCommunicator.TableName} WHERE PersonalRating='{personalRating}'";
            var records = this.FindBySqlCommand(command);

            return new RecordDatabaseEnumerable(records);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindBySalary(string salary)
        {
            string command = $"SELECT * FROM {ServerCommunicator.TableName} WHERE Salary='{salary}'";
            var records = this.FindBySqlCommand(command);

            return new RecordDatabaseEnumerable(records);
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetRecord(int id)
        {
            ServerCommunicator.OpenServerConnection();
            SqlCommand command = new SqlCommand();
            command.Connection = ServerCommunicator.ServerConnection;
            command.CommandText = $"SELECT * FROM {ServerCommunicator.TableName}";
            var reader = command.ExecuteReader();

            reader.Read();
            var record = this.GetRecordByParseSqlDataReader(reader);

            if (record == null)
            {
                throw new ArgumentNullException("Invalid record.");
            }

            return record;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            ServerCommunicator.OpenServerConnection();
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();

            SqlCommand command = new SqlCommand();
            command.CommandText = $"SELECT * FROM {ServerCommunicator.TableName}";
            command.Connection = ServerCommunicator.ServerConnection;

            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var record = this.GetRecordByParseSqlDataReader(reader);

                    if (record != null)
                    {
                        records.Add(record);
                    }
                }
            }

            ServerCommunicator.CloseServerConnection();

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public void GetStat()
        {
            int count = this.GetRecordsCount();

            Console.WriteLine($"Stored {count} record(s).");
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
            MemoizerService.RefreshMemoizer();

            ServerCommunicator.OpenServerConnection();

            for (int i = 0; i < records.Count; i++)
            {
                SqlCommand command = new SqlCommand();
                command.CommandText = this.GetUpdateCommandWithRecord(records[i]);
                command.Connection = ServerCommunicator.ServerConnection;
                _ = command.ExecuteNonQuery();
            }

            ServerCommunicator.CloseServerConnection();

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

        private int GetRecordsCount()
        {
            ServerCommunicator.OpenServerConnection();
            SqlCommand command = new SqlCommand();
            command.CommandText = $"SELECT COUNT(*) FROM {ServerCommunicator.TableName};";
            command.Connection = ServerCommunicator.ServerConnection;
            var count = command.ExecuteScalar();
            ServerCommunicator.CloseServerConnection();

            return (int)count;
        }

        private string GetInsertCommandWithRecord(FileCabinetRecord record)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"INSERT INTO {ServerCommunicator.TableName} (Id,FirstName,LastName,DateOfBirth,PersonalRating,Salary,Gender) VALUES (");
            builder.Append($"{record.Id},");
            builder.Append($"'{record.FirstName}',");
            builder.Append($"'{record.LastName}',");
            builder.Append($"'{record.DateOfBirth.ToString("yyyy-MM-dd")}',");
            builder.Append($"{record.PersonalRating},");
            builder.Append($"{record.Salary.ToString().Replace(',', '.')},");
            builder.Append($"'{record.Gender}');");

            return builder.ToString();
        }

        private string GetUpdateCommandWithRecord(FileCabinetRecord record)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"UPDATE {ServerCommunicator.TableName} SET ");
            builder.Append($"FirstName='{record.FirstName}',");
            builder.Append($"LastName='{record.LastName}',");
            builder.Append($"DateOfBirth='{record.DateOfBirth.ToString("yyyy-MM-dd")}',");
            builder.Append($"PersonalRating={record.PersonalRating},");
            builder.Append($"Salary={record.Salary.ToString().Replace(',', '.')},");
            builder.Append($"Gender='{record.Gender}' ");
            builder.Append($"WHERE Id={record.Id};");

            return builder.ToString();
        }

        private FileCabinetRecord? GetRecordByParseSqlDataReader(SqlDataReader reader)
        {
            object id = reader["Id"];
            object firstName = reader["FirstName"];
            object lastName = reader["LastName"];
            object birthDate = reader["DateOfBirth"];
            object personalRating = reader["PersonalRating"];
            object salary = reader["Salary"];
            object gender = reader["Gender"];

            FileCabinetRecord record = new FileCabinetRecord()
            {
                Id = (int)id,
                FirstName = (string)firstName,
                LastName = (string)lastName,
                DateOfBirth = (DateTime)birthDate,
                PersonalRating = (short)personalRating,
                Salary = (decimal)salary,
                Gender = Convert.ToChar(gender),
            };

            bool isValid = this.recordValidator.ValidateParameters(record);

            if (isValid)
            {
                return record;
            }

            return null;
        }

        private List<FileCabinetRecord> FindBySqlCommand(string input)
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();

            ServerCommunicator.OpenServerConnection();

            SqlCommand command = new SqlCommand();
            command.Connection = ServerCommunicator.ServerConnection;
            command.CommandText = input;
            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var record = this.GetRecordByParseSqlDataReader(reader);

                    if (record != null)
                    {
                        records.Add(record);
                    }
                }
            }

            ServerCommunicator.CloseServerConnection();

            return records;
        }
    }
}
