using FileCabinetApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace FileCabinetGenerator.FormatWriters
{
    internal static class DatabaseWriter
    {
        internal const string ConnectionString = "Data Source=PC1-5514;Initial Catalog=FileCabinet;Integrated Security=True;TrustServerCertificate=True;";
        //internal const string TableName = "FileCabinetRecords";
        internal const string TableName = "table1";

        internal static void Write(FileCabinetRecord[] records)
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            for (int i = 0; i < records.Length; i++)
            {
                var record = records[i];
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = $"INSERT INTO {TableName} (Id,FirstName,LastName,DateOfBirth,PersonalRating,Salary,Gender) VALUES ({record.Id},'{record.FirstName}','{record.LastName}','{record.DateOfBirth.ToString("yyyy-MM-dd")}',{record.PersonalRating},{record.Salary.ToString().Replace(',', '.')},'{record.Gender}');";
                _ = cmd.ExecuteNonQuery();
            }

            connection.Close();
        }
    }
}
