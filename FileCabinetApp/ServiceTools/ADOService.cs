using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
namespace FileCabinetApp.ServiceTools
{
    /// <summary>
    /// Class provides communication with server.
    /// </summary>
    internal static class ADOService
    {
        /// <summary>
        /// Name of the table on the server's side.
        /// </summary>
        internal static readonly string TableName = ConfigurationManager.AppSettings.Get("DatabaseTableName") ?? "FileCabinetRecords";

        /// <summary>
        /// String to connect with server.
        /// </summary>
        internal static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;

        /// <summary>
        /// Instance of server connection.
        /// </summary>
        internal static readonly SqlConnection ServerConnection = new SqlConnection(ConnectionString);

        /// <summary>
        /// Open server communication.
        /// </summary>
        internal static void OpenServerConnection()
        {
            try
            {
                ServerConnection.Open();

                // Console.WriteLine($"-{Environment.NewLine}The connection to server('{ServerConnection.DataSource}') is opened.");
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine(sqlException.Message);
            }
        }

        /// <summary>
        /// Close server connection.
        /// </summary>
        internal static void CloseServerConnection()
        {
            try
            {
                if (ServerConnection.State == System.Data.ConnectionState.Open)
                {
                    ServerConnection.Close();

                    // Console.WriteLine($"The connection is closed.{Environment.NewLine}-");
                }
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine(sqlException.Message);
            }
        }

        /// <summary>
        /// Gets data reader by command.
        /// </summary>
        /// <param name="inputCommand">Input command to interact with database.</param>
        /// <returns>Sql Data Reader.</returns>
        internal static SqlDataReader GetSqlDataReader(string inputCommand)
        {
            ADOService.OpenServerConnection();
            SqlCommand command = new SqlCommand();
            command.CommandText = inputCommand;
            command.Connection = ADOService.ServerConnection;
            var reader = command.ExecuteReader();
            ADOService.CloseServerConnection();

            return reader;
        }

        /// <summary>
        /// Check table presence in database.
        /// </summary>
        internal static void CheckTablePresenceInDatabase()
        {
            ADOService.OpenServerConnection();
            try
            {
                Console.WriteLine($"Table '{ADOService.TableName}' creating...");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = ADOService.GetCreateTableCommand(ADOService.TableName);
                cmd.Connection = ADOService.ServerConnection;
                _ = cmd.ExecuteNonQuery();
                Console.WriteLine($"Table created.");
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine(sqlException.Message);
            }
            catch (InvalidOperationException invalidOperationException)
            {
                Console.WriteLine(invalidOperationException.Message);
            }

            ADOService.CloseServerConnection();
        }

        private static string GetCreateTableCommand(string tableName)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"CREATE TABLE {ADOService.TableName} (");
            builder.Append("Id INT NOT NULL PRIMARY KEY,");
            builder.Append("FirstName VARCHAR(120) NOT NULL,");
            builder.Append("LastName VARCHAR(120) NOT NULL,");
            builder.Append("DateOfBirth DATE NOT NULL,");
            builder.Append("PersonalRating SMALLINT NOT NULL,");
            builder.Append("Salary DECIMAL(18,3) NOT NULL,");
            builder.Append("Gender CHAR(1) NOT NULL)");

            return builder.ToString();
        }
    }
}
