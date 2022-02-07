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
    internal class FileCabinetDatabaseService : FileCabinetDictionary, IFileCabinetService, IDisposable
    {
        private const string ConnectionString = "Data Source=PC1-5514;Initial Catalog=FileCabinet;Integrated Security=True;TrustServerCertificate=True;";
        private const string TableName = "FileCabinetRecords";
        private readonly IRecordValidator recordValidator;
        private SqlConnection serverConnection = new SqlConnection(ConnectionString);

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetDatabaseService"/> class.
        /// </summary>
        /// <param name="recordValidator">Validator for record.</param>
        public FileCabinetDatabaseService(IRecordValidator recordValidator)
        {
            this.recordValidator = recordValidator;

            this.CheckTablePresenceInDatabase();
        }

        /// <inheritdoc/>
        public bool CheckRecordPresence(int id)
        {
            throw new NotImplementedException();
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
            this.OpenServerConnection();
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();

            SqlCommand command = new SqlCommand();
            command.CommandText = $"SELECT * FROM {TableName}";
            command.Connection = this.serverConnection;

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

            this.CloseServerConnection();

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public void GetStat()
        {
            throw new NotImplementedException();
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

        /// <inheritdoc/>
        public void Dispose()
        {
            this.serverConnection.Dispose();
        }

        private void CheckTablePresenceInDatabase()
        {
            this.OpenServerConnection();
            try
            {
                Console.WriteLine($"Table '{TableName}' creating...");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = this.GetCreateTableCommand(TableName);
                cmd.Connection = this.serverConnection;
                _ = cmd.ExecuteNonQuery();
                Console.WriteLine($"Table created.");
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine(sqlException.Message);
            }

            this.CloseServerConnection();
        }

        private string GetCreateTableCommand(string tableName)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"CREATE TABLE {TableName} (");
            builder.Append("Id INT NOT NULL,");
            builder.Append("FirstName VARCHAR(120) NOT NULL,");
            builder.Append("LastName VARCHAR(120) NOT NULL,");
            builder.Append("DateOfBirth DATETIME NOT NULL,");
            builder.Append("PersonalRating SMALLINT NOT NULL,");
            builder.Append("Salary DECIMAL(18,0) NOT NULL,");
            builder.Append("Gender CHAR(1) NOT NULL)");

            return builder.ToString();
        }

        private void OpenServerConnection()
        {
            try
            {
                this.serverConnection.Open();

                Console.WriteLine($"-{Environment.NewLine}The connection to server('{this.serverConnection.DataSource}') is opened.");
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine(sqlException.Message);
            }
        }

        private void CloseServerConnection()
        {
            try
            {
                if (this.serverConnection.State == System.Data.ConnectionState.Open)
                {
                    this.serverConnection.Close();
                    Console.WriteLine($"The connection is closed.{Environment.NewLine}-");
                }
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine(sqlException.Message);
            }
        }
    }
}
