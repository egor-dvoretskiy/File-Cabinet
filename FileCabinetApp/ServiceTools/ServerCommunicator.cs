using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

#pragma warning disable SA1401 // Fields should be private
namespace FileCabinetApp.ServiceTools
{
    /// <summary>
    /// Class provides communication with server.
    /// </summary>
    internal static class ServerCommunicator
    {
        /// <summary>
        /// String to connect with server.
        /// </summary>
        internal const string ConnectionString = "Data Source=PC1-5514;Initial Catalog=FileCabinet;Integrated Security=True;TrustServerCertificate=True;";

        /// <summary>
        /// Name of the table on the server's side.
        /// </summary>
        internal const string TableName = "FileCabinetRecords";

        /// <summary>
        /// Instance of server connection.
        /// </summary>
        internal static SqlConnection ServerConnection = new SqlConnection(ConnectionString);

        /// <summary>
        /// Open server communication.
        /// </summary>
        internal static void OpenServerConnection()
        {
            try
            {
                ServerConnection.Open();

                Console.WriteLine($"-{Environment.NewLine}The connection to server('{ServerConnection.DataSource}') is opened.");
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
