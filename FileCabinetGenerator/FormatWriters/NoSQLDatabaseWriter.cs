using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp;
using FileCabinetApp.ServiceTools;
using MongoDB.Driver;

namespace FileCabinetGenerator.FormatWriters
{
    internal class NoSQLDatabaseWriter
    {
        internal const string ConnectionString = "mongodb://localhost:27017/FileCabinet;";
        //internal const string TableName = "FileCabinetRecords";
        internal const string TableName = "FileCabinetRecords";

        internal static void Write(FileCabinetRecord[] records)
        {
            var client = new MongoClient(ConnectionString);
            var collection = client.GetDatabase("FileCabinet").GetCollection<FileCabinetRecord>(TableName);
            collection.InsertMany(records);
        }
    }
}
