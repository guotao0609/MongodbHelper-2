using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MongodbHelper
{
    public class MongodbAccess
    {
        protected IMongoDatabase MongodbAccessInstance;
        private static Dictionary<string, object> _collections = new Dictionary<string, object>();
        public MongodbAccess(string dbName, string connstring)
        {
            this.MongodbAccessInstance = MongodbAccessFactory.FactoryMongodbAccessInstance(dbName, connstring);
        }
        protected IMongoCollection<T> CurrentCollection<T>()
        {
            Type t = typeof(T);
            IMongoCollection<T> collection;
            if (_collections.ContainsKey(t.FullName))
            {
                collection = _collections[t.FullName] as IMongoCollection<T>;
                if (collection != null)
                    return collection;
            }
            var attrs = t.GetCustomAttributes(typeof(CollectionNameAttribute), true);
            collection = this.MongodbAccessInstance.GetCollection<T>(attrs.Length == 0 ? t.Name : (attrs[0] as CollectionNameAttribute).Value);
            lock ("Shop.MongodbHelper.MongodbAccess.CurrentCollection")
            {
                if (!_collections.Keys.Contains(t.FullName))
                    _collections.Add(t.FullName, collection);
            }
            return collection;
        }

        public virtual List<T> Query<T>(System.Linq.Expressions.Expression<Func<T, bool>> filter) where T : class,new()
        {
            return this.CurrentCollection<T>().Find<T>(filter).ToList();
        }

        public virtual Task<List<T>> QueryAsync<T>(System.Linq.Expressions.Expression<Func<T, bool>> filter) where T : class,new()
        {
            return this.CurrentCollection<T>().Find<T>(filter).ToListAsync();
        }
        public virtual void Insert<T>(T model) where T : class,new()
        {
            this.CurrentCollection<T>().InsertOne(model);
        }

        public virtual Task InsertAsync<T>(T model) where T : class,new()
        {
            return this.CurrentCollection<T>().InsertOneAsync(model);
        }
        public virtual void BatchInsert<T>(IEnumerable<T> array) where T : class,new()
        {
            this.CurrentCollection<T>().InsertMany(array);
        }
        public virtual Task BatchInsertAsync<T>(IEnumerable<T> array) where T : class,new()
        {
            return this.CurrentCollection<T>().InsertManyAsync(array);
        }
        public virtual long Update<T>(Dictionary<System.Linq.Expressions.Expression<Func<T, object>>, object> dic, System.Linq.Expressions.Expression<Func<T, bool>> filter) where T : class,new()
        {
            List<UpdateDefinition<T>> list = new List<UpdateDefinition<T>>();
            foreach (var item in dic)
            {
                list.Add(Builders<T>.Update.Set(item.Key, item.Value));
            }
            var updates = Builders<T>.Update.Combine(list).CurrentDate("lastModified");
            return this.CurrentCollection<T>().UpdateMany<T>(filter, updates).ModifiedCount;
        }
        public virtual void UpdateAsync<T>(T model, System.Linq.Expressions.Expression<Func<T, bool>> filter) where T : class,new()
        {
            this.CurrentCollection<T>().UpdateManyAsync<T>(filter, new ObjectUpdateDefinition<T>(model));
        }
        public virtual long QueryCount<T>(System.Linq.Expressions.Expression<Func<T, bool>> filter) where T : class,new()
        {
            return this.CurrentCollection<T>().Count<T>(filter);
        }
        public virtual Task<long> QueryCountAsync<T>(System.Linq.Expressions.Expression<Func<T, bool>> filter) where T : class,new()
        {
            return this.CurrentCollection<T>().CountAsync<T>(filter);
        }
        public virtual long Delete<T>(System.Linq.Expressions.Expression<Func<T, bool>> filter) where T : class,new()
        {
            return this.CurrentCollection<T>().DeleteMany<T>(filter).DeletedCount;
        }
        public virtual void DeleteAsync<T>(System.Linq.Expressions.Expression<Func<T, bool>> filter) where T : class,new()
        {
            this.CurrentCollection<T>().DeleteManyAsync<T>(filter);
        }
    }
}
