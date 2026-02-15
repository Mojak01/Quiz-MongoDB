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
            _db = client.GetDatabase("MojtabaAkbari");
        }

        public IMongoCollection<QuestionPack> QuestionPacks =>
            _db.GetCollection<QuestionPack>("QuestionPacks");

        public IMongoCollection<Category> Categories =>
        _db.GetCollection<Category>("Categories");

        public async Task SavePacksAsync(List<QuestionPack> packs)
        {
            await QuestionPacks.DeleteManyAsync(_ => true);
            if (packs.Count > 0)
            {
                await QuestionPacks.InsertManyAsync(packs);
            }
        }

        public async Task<List<QuestionPack>> LoadPacksAsync()
        {
            return await QuestionPacks.Find(_ => true).ToListAsync();
        }


    }
}