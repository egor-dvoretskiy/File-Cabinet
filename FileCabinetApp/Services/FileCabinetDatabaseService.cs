using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;
using FileCabinetApp.ServiceTools;
using Microsoft.Data.SqlClient;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Database service class.
    /// </summary>
    internal class FileCabinetDatabaseService : FileCabinetDictionary, IFileCabinetService
    {
        private const string ConnectionString = "Data Source=PC1-5514;Initial Catalog=FileCabinet;Integrated Security=True";

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetDatabaseService"/> class.
        /// </summary>
        public FileCabinetDatabaseService()
        {
            this.AcquireDatabaseConnection();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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

        private async void AcquireDatabaseConnection()
        {
            SqlConnection connection = new SqlConnection(ConnectionString);

            try
            {
                await connection.OpenAsync(); // Вывод информации о подключении

                Console.WriteLine("The connection is opened.");

                Console.WriteLine("Connection settings:");
                Console.WriteLine($"\tConnection string: {connection.ConnectionString}");
                Console.WriteLine($"\tDatabase: {connection.Database}");
                Console.WriteLine($"\tServer: {connection.DataSource}");
                Console.WriteLine($"\tServer version: {connection.ServerVersion}");
                Console.WriteLine($"\tStatus: {connection.State}");
                Console.WriteLine($"\tWorkstationld: {connection.WorkstationId}");
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine(sqlException.Message);
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    await connection.CloseAsync();
                    Console.WriteLine("The connection is closed.");
                }
            }

            Console.WriteLine("SqlConnection stopped.");
        }
    }
}
