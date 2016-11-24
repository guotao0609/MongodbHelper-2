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
            var attrs = t.GetCustomAttributes(typeof(MappingInformationAttribute), true);
            if (attrs.Length == 0)
                throw new Exception("not found CollectionNameAttribute");
            if (string.IsNullOrEmpty((attrs[0] as MappingInformationAttribute).DatebaseName))
                throw new Exception("not found datebaseName");
            if (string.IsNullOrEmpty((attrs[0] as MappingInformationAttribute).CollectionName))
                throw new Exception("not found collectionName");
            collection = MongodbAccessFactory.FactoryMongodbAccessInstance((attrs[0] as MappingInformationAttribute).DatebaseName, this._connstring).GetCollection<TEntity>((attrs[0] as MappingInformationAttribute).CollectionName);
            lock ("MongodbHelper.MongodbAccess.CurrentCollection")
            {
                if (!_collections.Keys.Contains(t.FullName))
                    _collections.Add(t.FullName, collection);
            }
            return collection;
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
                list.Add(Builders<TEntity>.Update.Set(item.Key, item.Value.ToString()));
            }
            var updates = Builders<TEntity>.Update.Combine(list);
            return this.CurrentCollection<TEntity>().UpdateMany<TEntity>(filter, updates).ModifiedCount;
        }

        public virtual void UpdateAsync<TEntity>(Dictionary<string, object> dic, Expression<Func<TEntity, bool>> filter) where TEntity : CollectionEntityBase, new()
        {
            List<UpdateDefinition<TEntity>> list = new List<UpdateDefinition<TEntity>>();
            foreach (var item in dic)
            {
                list.Add(Builders<TEntity>.Update.Set(item.Key, item.Value.ToString()));
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
    }
}
