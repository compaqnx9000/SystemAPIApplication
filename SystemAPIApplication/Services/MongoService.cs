using Microsoft.Extensions.Configuration;
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
        public IConfiguration Configuration { get; }
        private MongoClient _client = null;
        private MongoClient _otherClient = null;


        public MongoService(IConfiguration configuration)
        {
            Configuration = configuration;

            string test = Configuration["test:name"];
            Console.WriteLine(test);
            

            // 自己的库
            string conn = "mongodb://" + Configuration["MongoSetting:Ip"] + ":" + Configuration["MongoSetting:Port"];
            _client = new MongoClient(conn);

            // 别人的库
            string otherConn = "mongodb://" + Configuration["MongoOtherSetting:Ip"] + ":" + Configuration["MongoOtherSetting:Port"];
            _otherClient = new MongoClient(otherConn);
        }

        
        public List<MockBO> QueryMock(string[] ids)
        {
            IMongoCollection<MockBO> collection = _otherClient.GetDatabase(Configuration["MongoOtherSetting:MockSetting:Database"]).
                GetCollection<MockBO>(Configuration["MongoOtherSetting:MockSetting:Collection"]);

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
            Console.WriteLine("看看"+Configuration["MongoOtherSetting:MockSetting:Database"]);
            IMongoCollection<MockBO> collection = _otherClient.GetDatabase(Configuration["MongoOtherSetting:MockSetting:Database"]).
                GetCollection<MockBO>(Configuration["MongoOtherSetting:MockSetting:Collection"]);

            //IMongoCollection<MockBO> collection = _client.GetDatabase("hb").
            //   GetCollection<MockBO>("hbmock");

            var filter = Builders<MockBO>.Filter;
            return collection.Find(filter.Empty).ToList();
        }

        

        public RuleBo QueryRule(string name)
        {
            var collection = _client.GetDatabase(Configuration["MongoSetting:RuleSetting:Database"])
                                   .GetCollection<BsonDocument>(Configuration["MongoSetting:RuleSetting:Collection"]);
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
