using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemAPIApplication.bo;

namespace SystemAPIApplication.Services
{
    public class MongoService : IMongoService
    {
        private MongoSetting _config;
        private MongoClient _client = null;
        public MongoService(IOptions<MongoSetting> setting)
        {
            _config = setting.Value;
            string conn = "mongodb://" + _config.IP + ":" + _config.Port;
            _client = new MongoClient(conn);
        }

        
        public List<MockBO> QueryMock(string[] ids)
        {
            IMongoCollection<MockBO> collection = _client.GetDatabase(_config.MockSetting.Database).
                GetCollection<MockBO>(_config.MockSetting.Collection);

            var filter = Builders<MockBO>.Filter;
            FilterDefinition<MockBO> filterDefinition1 = null;

            if (ids != null)
            {
                foreach (string id in ids)
                {
                    if (filterDefinition1 == null)
                        filterDefinition1 = filter.Eq("NuclearExplosionID", id);
                    else
                        filterDefinition1 = filterDefinition1 | filter.Eq("NuclearExplosionID", id);
                }

                FilterDefinition<MockBO> filterDefinition = null;
                if (filterDefinition1 != null)
                    filterDefinition = filterDefinition1;

                var docs = collection.Find(filterDefinition).ToList();
                return docs;
            }

            return null;

        }
       

        public List<MockBO> QueryMockAll()
        {
            IMongoCollection<MockBO> collection = _client.GetDatabase(_config.MockSetting.Database).
                GetCollection<MockBO>(_config.MockSetting.Collection);

            var filter = Builders<MockBO>.Filter;
            return collection.Find(filter.Empty).ToList();
        }

        

        public RuleBo QueryRule(string name)
        {
            var collection = _client.GetDatabase(_config.RuleSetting.Database)
                                   .GetCollection<BsonDocument>(_config.RuleSetting.Collection);
            var list = collection.Find(Builders<BsonDocument>.Filter.Eq("name", name)).ToList();
            foreach (var doc in list)
            {
                var bo = BsonSerializer.Deserialize<RuleBo>(doc);
                return bo;
            }
            return null;
        }

    }
}
