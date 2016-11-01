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
        public MongodbAccess(string dbName)
        {
            this.MongodbAccessInstance = MongodbAccessFactory.FactoryMongodbAccessInstance(dbName);
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
            lock ("Shop.MongodbLibrary.MongodbAccess.MongodbAccessBase.GetCollection")
            {
                if (!_collections.Keys.Contains(t.FullName))
                    _collections.Add(t.FullName, collection);
            }
            return collection;
        }

        public virtual List<T> Query<T>(System.Linq.Expressions.Expression<Func<T, bool>> where) where T : class,new()
        {
            return this.CurrentCollection<T>().Find<T>(where).ToList();
        }

        public virtual Task<List<T>> QueryAsync<T>( System.Linq.Expressions.Expression<Func<T, bool>> where) where T : class,new()
        {
            return this.CurrentCollection<T>().Find<T>(where).ToListAsync();
        }
        public virtual void Insert<T>( T model) where T : class,new()
        {
            this.CurrentCollection<T>().InsertOne(model);
        }

        public virtual Task InsertAsync<T>( T model) where T : class,new()
        {
            return this.CurrentCollection<T>().InsertOneAsync(model);
        }
        public virtual void BatchInsert<T>( IEnumerable<T> array) where T : class,new()
        {
            this.CurrentCollection<T>().InsertMany(array);
        }
        public virtual Task BatchInsertAsync<T>( IEnumerable<T> array) where T : class,new()
        {
            return this.CurrentCollection<T>().InsertManyAsync(array);
        }
        public virtual long Update<T>( T model, System.Linq.Expressions.Expression<Func<T, bool>> where) where T : class,new()
        {
            return this.CurrentCollection<T>().UpdateMany<T>(where, new ObjectUpdateDefinition<T>(model)).ModifiedCount;
        }
        public virtual void UpdateAsync<T>( T model, System.Linq.Expressions.Expression<Func<T, bool>> where) where T : class,new()
        {
            this.CurrentCollection<T>().UpdateManyAsync<T>(where, new ObjectUpdateDefinition<T>(model));
        }
        public virtual long QueryCount<T>( System.Linq.Expressions.Expression<Func<T, bool>> where) where T : class,new()
        {
            return this.CurrentCollection<T>().Count<T>(where);
        }
        public virtual Task<long> QueryCountAsync<T>( System.Linq.Expressions.Expression<Func<T, bool>> where) where T : class,new()
        {
            return this.CurrentCollection<T>().CountAsync<T>(where);
        }
        public virtual long Delete<T>( System.Linq.Expressions.Expression<Func<T, bool>> where) where T : class,new()
        {
            return this.CurrentCollection<T>().DeleteMany<T>(where).DeletedCount;
        }
        public virtual void DeleteAsync<T>( System.Linq.Expressions.Expression<Func<T, bool>> where) where T : class,new()
        {
            this.CurrentCollection<T>().DeleteManyAsync<T>(where);
        }
    }
}
