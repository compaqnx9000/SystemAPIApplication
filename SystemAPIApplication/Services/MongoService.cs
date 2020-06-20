using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            //_client = new MongoClient("mongodb://localhost:27017");
            _client = new MongoClient(conn);
        }

        //{
        //    "_id":{"$oid":"5e9cf04bf05700001d006b23"},
        //    "NuclearExplosionID":"test001",
        //    "OccurTime":{"$date":"2020-04-20T12:00:00.000Z"},
        //    "Lon":105.5,
        //    "Lat":35.4,
        //    "Alt":1000.0,
        //    "Yield":30.5,
        //    "Producer":"生产单位1",
        //    "DetectionType":"电磁",
        //    "ReportTime":{"$date":"2020-04-01T12:21:00.000Z"},
        //    "Nonce":"0cdb25e7-1ece-4de6-b7ed-1f535697672e"
        //}
        public List<BsonDocument> Query(string[] ids)
        {
            
            var collection = _client.GetDatabase(_config.Document).
                GetCollection<BsonDocument>(_config.Collection);
            //var collection = _client.GetDatabase("hb").GetCollection<BsonDocument>("hbmock");

            var filter = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument> filterDefinition1 = null;

            if (ids != null)
            {
                foreach (string id in ids)
                {
                    if (filterDefinition1 == null)
                        filterDefinition1 = filter.Eq("NuclearExplosionID", id);
                    else
                        filterDefinition1 = filterDefinition1 | filter.Eq("NuclearExplosionID", id);
                }

                FilterDefinition<BsonDocument> filterDefinition = new BsonDocument();
                if (filterDefinition1 != null)
                    filterDefinition = filterDefinition1;

                var docs = collection.Find(filterDefinition).ToList();
                return docs;
            }

            return null;

        }

        public List<BsonDocument> QueryAll()
        {
            var collection = _client.GetDatabase(_config.Document).
                GetCollection<BsonDocument>(_config.Collection);
            //var collection = _client.GetDatabase("hb").GetCollection<BsonDocument>("hbmock");

            //FilterDefinition<BsonDocument> filterDefinition = null;
            var filter = Builders<BsonDocument>.Filter;
            return collection.Find(filter.Empty).ToList();
        }

    }
}
