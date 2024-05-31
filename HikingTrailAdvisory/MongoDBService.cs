using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace HikingTrailAdvisory
{
    public class MongoDBService
    {
        private readonly string _connectionString = "mongodb://localhost:27017";
        private readonly string _databaseName = "hikes";
        private readonly string _collectionName = "info";
        private readonly IMongoCollection<BsonDocument> _collection;

        public MongoDBService()
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_databaseName);
            _collection = database.GetCollection<BsonDocument>(_collectionName);
        }

        public void InsertDictionaryAsync(Dictionary<string, Hike> data)
        {
            var document = new BsonDocument(data);
            _collection.InsertOne(document);
            Console.WriteLine("Data inserted successfully!");
        }
    }
}
