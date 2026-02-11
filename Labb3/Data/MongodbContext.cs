using MongoDB.Driver;
using Labb3.Models;

namespace Labb3.Data
{
    public class MongoDbContext
    {
        IMongoDatabase _db;

        public MongoDbContext()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _db = client.GetDatabase("QuizDb");
        }

        public IMongoCollection<QuestionPack> QuestionPacks =>
            _db.GetCollection<QuestionPack>("QuestionPacks");
    }
}