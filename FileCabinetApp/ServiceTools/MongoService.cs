using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FileCabinetApp.ServiceTools
{
    /// <summary>
    /// Class for work with nosql mongodb.
    /// </summary>
    internal class MongoService
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
        private static MongoClient client = new MongoClient(ConnectionString);

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoService"/> class.
        /// </summary>
        internal MongoService()
        {
            // this.GetCollectionsNames(this.client);
            // this.CreateDatabase(this.client);
        }

        /// <summary>
        /// Gets the collection from mongo database object.
        /// </summary>
        /// <returns>Returns IMongoCollection interface.</returns>
        public static IMongoCollection<FileCabinetRecord> GetMongoCollection()
        {
            return GetMongoDatabase().GetCollection<FileCabinetRecord>("FileCabinetRecords");
        }

        /// <summary>
        /// Get the mongo database object.
        /// </summary>
        /// <returns>Returns IMongoDatabase interface.</returns>
        public static IMongoDatabase GetMongoDatabase()
        {
            return client.GetDatabase("FileCabinet");
        }

        /*private void GetCollectionsNames(MongoClient client)
        {
            using (var cursor = client.ListDatabases())
            {
                var dbs = cursor.ToList();
                foreach (var db in dbs)
                {
                    Console.WriteLine("Database {0}, collections:", db["name"]);
                    IMongoDatabase database = client.GetDatabase(db["name"].ToString());
                    using (var collCursor = database.ListCollections())
                    {
                        var colls = collCursor.ToList();
                        foreach (var coll in colls)
                        {
                            Console.WriteLine(coll["name"]);
                        }
                    }

                    Console.WriteLine();
                }
            }
        }

        private void CreateDatabase(MongoClient client)
        {
            var db = client.GetDatabase("FileCabinet");
            // db.CreateCollection("FileCabinetRecords");
            var cl = db.GetCollection<FileCabinetRecord>("FileCabinetRecords");
            cl.InsertOne(new FileCabinetRecord()
            {
                Id = 1,
                FirstName = "Egor",
                LastName = "Sdas",
                DateOfBirth = new DateTime(2009, 02, 01),
                PersonalRating = 6969,
                Salary = 23,
                Gender = 'M',
            });
        }*/
    }
}
