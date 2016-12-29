using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongodbHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MongodbHelper
{
    public class MongodbAccess
    {
        protected string _connstring;
        private static Dictionary<string, object> _collections = new Dictionary<string, object>();
        private static Dictionary<string, IMongoDatabase> _databases = new Dictionary<string, IMongoDatabase>();
        public MongodbAccess(string connstring)
        {
            this._connstring = connstring;
        }
        protected IMongoCollection<TEntity> CurrentCollection<TEntity>() where TEntity : CollectionEntityBase, new()
        {
            Type t = typeof(TEntity);
            IMongoCollection<TEntity> collection;
            if (_collections.ContainsKey(t.FullName))
            {
                collection = _collections[t.FullName] as IMongoCollection<TEntity>;
                if (collection != null)
                    return collection;
            }
            string dbName, collectionName;
            var attrs = t.GetCustomAttributes(typeof(MappingInformationAttribute), true);
            if (attrs.Length == 0)
                throw new Exception("not found CollectionNameAttribute");
            var attr = attrs[0] as MappingInformationAttribute;
            if (attr == null)
                throw new Exception("CollectionNameAttribute mapping error");
            dbName = attr.DatebaseName;
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("not found datebaseName");
            collectionName = attr.CollectionName;
            if (string.IsNullOrEmpty(collectionName))
                throw new Exception("not found collectionName");
            if (!BsonClassMap.IsClassMapRegistered(t))
            {
                BsonClassMap.RegisterClassMap<TEntity>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }
            collection = this.CurrentDatabase(dbName).GetCollection<TEntity>(collectionName);
            lock ("MongodbHelper.MongodbAccess.CurrentCollection")
            {
                if (!_collections.Keys.Contains(t.FullName))
                    _collections.Add(t.FullName, collection);
            }
            return collection;
        }
        protected IMongoDatabase CurrentDatabase(string dbName = null)
        {
            IMongoDatabase database;
            if (_databases.ContainsKey(dbName))
            {
                database = _databases[dbName] as IMongoDatabase;
                if (database != null)
                    return database;
            }
            database = MongodbAccessFactory.FactoryMongodbAccessInstance(dbName, this._connstring);
            lock ("MongodbHelper.MongodbAccess.CurrentDatabase")
            {
                if (!_databases.Keys.Contains(dbName))
                    _databases.Add(dbName, database);
            }
            return database;
        }
        public void ChangeCollection<TEntity>(string dbName, string collectionName)
        {
            Type t = typeof(TEntity);
            var collection = this.CurrentDatabase(dbName).GetCollection<TEntity>(collectionName);
            if (_collections.ContainsKey(t.FullName))
                _collections[t.FullName] = collection;
            else
                _collections.Add(t.FullName, collection);
        }

        public virtual TResult Query<TEntity, TResult>(Func<MongoCollectionProxy<TEntity>, TResult> func) where TEntity : CollectionEntityBase, new()
        {
            return func(new MongoCollectionProxy<TEntity>(this.CurrentCollection<TEntity>()));
        }

        public virtual TResult QueryExt<TEntity, TResult>(Func<IQueryable<TEntity>, TResult> func) where TEntity : CollectionEntityBase, new()
        {
            return func(this.CurrentCollection<TEntity>().AsQueryable());
        }

        public virtual void Insert<TEntity>(TEntity model) where TEntity : CollectionEntityBase, new()
        {
            this.CurrentCollection<TEntity>().InsertOne(model);
        }

        public virtual Task InsertAsync<TEntity>(TEntity model) where TEntity : CollectionEntityBase, new()
        {
            return this.CurrentCollection<TEntity>().InsertOneAsync(model);
        }
        public virtual void BatchInsert<TEntity>(IEnumerable<TEntity> array) where TEntity : CollectionEntityBase, new()
        {
            this.CurrentCollection<TEntity>().InsertMany(array);
        }
        public virtual Task BatchInsertAsync<TEntity>(IEnumerable<TEntity> array) where TEntity : CollectionEntityBase, new()
        {
            return this.CurrentCollection<TEntity>().InsertManyAsync(array);
        }
        public virtual long Update<TEntity>(Dictionary<string, object> dic, Expression<Func<TEntity, bool>> filter) where TEntity : CollectionEntityBase, new()
        {
            List<UpdateDefinition<TEntity>> list = new List<UpdateDefinition<TEntity>>();
            foreach (var item in dic)
            {
                list.Add(Builders<TEntity>.Update.Set(item.Key, item.Value));
            }
            var updates = Builders<TEntity>.Update.Combine(list);
            return this.CurrentCollection<TEntity>().UpdateMany<TEntity>(filter, updates).ModifiedCount;
        }

        public virtual void UpdateAsync<TEntity>(Dictionary<string, object> dic, Expression<Func<TEntity, bool>> filter) where TEntity : CollectionEntityBase, new()
        {
            List<UpdateDefinition<TEntity>> list = new List<UpdateDefinition<TEntity>>();
            foreach (var item in dic)
            {
                list.Add(Builders<TEntity>.Update.Set(item.Key, item.Value));
            }
            var updates = Builders<TEntity>.Update.Combine(list);
            this.CurrentCollection<TEntity>().UpdateManyAsync<TEntity>(filter, updates);
        }
        public virtual long Delete<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : CollectionEntityBase, new()
        {
            return this.CurrentCollection<TEntity>().DeleteMany<TEntity>(filter).DeletedCount;
        }
        public virtual void DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : CollectionEntityBase, new()
        {
            this.CurrentCollection<TEntity>().DeleteManyAsync<TEntity>(filter);
        }

        public virtual IEnumerable<TResult> MapReduce<TEntity, TResult>(MapReduceOptionsProxy<TEntity, TResult> options) where TEntity : CollectionEntityBase, new()
        {
            if (string.IsNullOrEmpty(options.Map))
                throw new Exception("map is must");
            if (string.IsNullOrEmpty(options.Reduce))
                throw new Exception("reduce is must");
            var o = new MapReduceOptions<TEntity, TResult>();
            if (options.OutputEnum.Equals(MapReduceOutputOptionsEnum.Inline))
            {
                o.OutputOptions = MapReduceOutputOptions.Inline;
            }
            else
            {
                if (string.IsNullOrEmpty(options.DatabaseName) || string.IsNullOrEmpty(options.CollectionName))
                    throw new Exception("DatabaseName and CollectionName is must");
                if (options.OutputEnum.Equals(MapReduceOutputOptionsEnum.Merge))
                    o.OutputOptions = MapReduceOutputOptions.Reduce(options.CollectionName, options.DatabaseName);
                else if (options.OutputEnum.Equals(MapReduceOutputOptionsEnum.Reduce))
                    o.OutputOptions = MapReduceOutputOptions.Reduce(options.CollectionName, options.DatabaseName);
                else if (options.OutputEnum.Equals(MapReduceOutputOptionsEnum.Replace))
                    o.OutputOptions = MapReduceOutputOptions.Replace(options.CollectionName, options.DatabaseName);
            }
            o.BypassDocumentValidation = options.BypassDocumentValidation;
            if (!string.IsNullOrEmpty(options.Filter))
                o.Filter = options.Filter;
            if (!string.IsNullOrEmpty(options.Finalize))
                o.Finalize = options.Finalize;
            o.JavaScriptMode = options.JavaScriptMode;
            o.Limit = options.Limit;
            o.MaxTime = options.MaxTime;
            if (!string.IsNullOrEmpty(options.Sort))
                o.Sort = options.Sort;
            o.Verbose = options.Verbose;
            var r = this.CurrentCollection<TEntity>().MapReduce<TResult>(new MongoDB.Bson.BsonJavaScript(options.Map), new MongoDB.Bson.BsonJavaScript(options.Reduce), o);
            if (options.OutputEnum.Equals(MapReduceOutputOptionsEnum.Inline))
                return r.Current;
            else return null;
        }

        public virtual void MapReduce<TEntity>(MapReduceOptionsProxy<TEntity, TEntity> options) where TEntity : CollectionEntityBase, new()
        {
            this.MapReduce<TEntity, TEntity>(options);
        }

        public virtual IQueryable<TEntity> GetIQueryable<TEntity>() where TEntity : CollectionEntityBase, new()
        {
            return this.CurrentCollection<TEntity>().AsQueryable();
        }

        public virtual void CreateCollection(string databaseName, string collectionName)
        {
            this.CurrentDatabase(databaseName).CreateCollection(collectionName);
        }

        public virtual void CreateCollectionAsync(string databaseName, string collectionName)
        {
            this.CurrentDatabase(databaseName).CreateCollectionAsync(collectionName);
        }
        public virtual void DropCollection(string databaseName, string collectionName)
        {
            this.CurrentDatabase(databaseName).DropCollection(collectionName);
        }

        public virtual void DropCollectionAsync(string databaseName, string collectionName)
        {
            this.CurrentDatabase(databaseName).DropCollectionAsync(collectionName);
        }
    }
}
