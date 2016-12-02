using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongodbHelper
{
    internal class MongodbAccessFactory
    {
        private static Dictionary<string, IMongoDatabase> _mongodbDic = new Dictionary<string, IMongoDatabase>();
        private static MongoClient _mongoClient;
        private MongodbAccessFactory() { }
        internal static IMongoDatabase FactoryMongodbAccessInstance(string dbName, string connstring)
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(CollectionEntityBase)))
            {
                BsonClassMap.RegisterClassMap<CollectionEntityBase>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdProperty(c => c.Primaryid)
                        .SetIdGenerator(StringObjectIdGenerator.Instance)
                        .SetSerializer(new StringSerializer(BsonType.ObjectId));
                });
                BsonSerializer.RegisterSerializer(typeof(DateTime), DateTimeSerializer.LocalInstance);
            }
            if (_mongodbDic.Count <= 0 || !_mongodbDic.Keys.Contains(dbName))
            {
                if (_mongoClient == null)
                    _mongoClient = new MongoClient(connstring);
                var database = _mongoClient.GetDatabase(dbName);
                _mongodbDic.Add(dbName, database);
            }
            return _mongodbDic[dbName];
        }
    }
}
