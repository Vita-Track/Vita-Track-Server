using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Vita_Track_Server.Models;

namespace Vita_Track_Server.Data
{
    public class MongoDBServices
    {
        private readonly IMongoCollection<DoctorModel> _doctorCollection;
        public MongoDBServices(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _doctorCollection = database.GetCollection<DoctorModel>(mongoDBSettings.Value.DoctorCollectionName);

        }
    }
}