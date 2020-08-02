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
        List<MockBO> QueryMock(string[] ids);
        List<MockBO> QueryMockAll();
        RuleBo QueryRule(string name);

    }
}
