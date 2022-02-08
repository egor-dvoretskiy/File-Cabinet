using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;
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
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByBirthDate(string birthDate)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByGender(string gender)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByPersonalRating(string personalRating)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindBySalary(string salary)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public FileCabinetRecord GetRecord(int id)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public List<FileCabinetRecord> Select(string phrase, string memoizingKey, IRecordInputValidator inputValidator)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Update(List<FileCabinetRecord> records)
        {
            throw new NotImplementedException();
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
    }
}
