using FileSearcher.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FileSearcher.Data;

public class MongoContext
{
    private readonly IMongoCollection<MyFile>? _collection;
    public MongoContext()
    {
        var client = new MongoClient(
            "mongodb://localhost:27017"
        );
        var database = client.GetDatabase("local");
        _collection = database.GetCollection<MyFile>("files");
    }
    public async Task<List<MyFile>> GetFiles()
    {
        if (_collection == null)
        {
            return new List<MyFile>();
        }
        return await _collection.Find(x => x.DeletedDateTime == null).ToListAsync();
    }
    public MyFile GetFile(string id)
    {
        var objId = new ObjectId(id);
        return _collection.Find(x => x.Id == objId).FirstOrDefault();
    }
    public void Update(MyFile file)
    {
        _collection.FindOneAndReplace(x => x.Id == file.Id, file);
    }
    public List<MyFile> Search(string term)
    {
        var titleFilter = new BsonDocument { { "Title", new BsonDocument { { "$regex", term }, { "$options", "i" } } } };
        var tagsFilter = new BsonDocument { { "Tags", new BsonDocument { { "$regex", term }, { "$options", "i" } } } };
        var filter = Builders<MyFile>.Filter.Or(titleFilter, tagsFilter);
        return _collection.Find(filter).ToList();
    }
}
