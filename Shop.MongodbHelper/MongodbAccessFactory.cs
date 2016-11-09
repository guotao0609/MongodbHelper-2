using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MongodbHelper
{
    internal class MongodbAccessFactory
    {
        private static Dictionary<string, IMongoDatabase> _mongodbDic = new Dictionary<string, IMongoDatabase>();
        private MongodbAccessFactory() { }
        internal static IMongoDatabase FactoryMongodbAccessInstance(string dbName,string connstring)
        {
            if (_mongodbDic.Count <= 0 || !_mongodbDic.Keys.Contains(dbName))
            {
                var client = new MongoClient(connstring);
                var database = client.GetDatabase(dbName);
                _mongodbDic.Add(dbName, database);
            }
            return _mongodbDic[dbName];
        }
    }
}
