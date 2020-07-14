using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemAPIApplication.bo;

namespace SystemAPIApplication.Services
{
    public interface IMongoService
    {
        List<BsonDocument> Query(string[] ids);
        List<BsonDocument> QueryAll();
        RuleBo QueryRule(string name);

    }
}
