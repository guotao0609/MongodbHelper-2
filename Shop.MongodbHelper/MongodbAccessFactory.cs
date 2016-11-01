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
        private static string _mongodbConnString = "mongodb://localhost:27017";// ConfigurationManager.AppSettings["MongodbConnString"];
        private static Dictionary<string, IMongoDatabase> _mongodbDic = new Dictionary<string, IMongoDatabase>();
        private MongodbAccessFactory() { }
        internal static IMongoDatabase FactoryMongodbAccessInstance(string dbName)
        {
            if (_mongodbDic.Count <= 0 || !_mongodbDic.Keys.Contains(dbName))
            {
                var client = new MongoClient(_mongodbConnString);
                var database = client.GetDatabase(dbName);
                _mongodbDic.Add(dbName, database);
            }
            return _mongodbDic[dbName];
        }
    }
}
