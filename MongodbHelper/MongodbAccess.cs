using MongoDB.Driver;
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
        protected IMongoCollection<T> CurrentCollection<T>() where T : CollectionEntityBase, new()
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
            if (attrs.Length == 0)
                throw new Exception("not found CollectionNameAttribute");
            if (string.IsNullOrEmpty((attrs[0] as CollectionNameAttribute).DatebaseName))
                throw new Exception("not found datebaseName");
            if (string.IsNullOrEmpty((attrs[0] as CollectionNameAttribute).CollectionName))
                throw new Exception("not found collectionName");
            collection = MongodbAccessFactory.FactoryMongodbAccessInstance((attrs[0] as CollectionNameAttribute).DatebaseName, this._connstring).GetCollection<T>((attrs[0] as CollectionNameAttribute).CollectionName);
            lock ("MongodbHelper.MongodbAccess.CurrentCollection")
            {
                if (!_collections.Keys.Contains(t.FullName))
                    _collections.Add(t.FullName, collection);
            }
            return collection;
        }

        public virtual M Query<T, M>(Func<MongoCollectionProxy<T>, M> func) where T : CollectionEntityBase, new()
        {
            return func(new MongoCollectionProxy<T>(this.CurrentCollection<T>()));
        }

        public virtual M QueryExt<T, M>(Func<IQueryable<T>, M> func) where T : CollectionEntityBase, new()
        {
            return func(this.CurrentCollection<T>().AsQueryable());
        }

        public virtual void Insert<T>(T model) where T : CollectionEntityBase, new()
        {
            this.CurrentCollection<T>().InsertOne(model);
        }

        public virtual Task InsertAsync<T>(T model) where T : CollectionEntityBase, new()
        {
            return this.CurrentCollection<T>().InsertOneAsync(model);
        }
        public virtual void BatchInsert<T>(IEnumerable<T> array) where T : CollectionEntityBase, new()
        {
            this.CurrentCollection<T>().InsertMany(array);
        }
        public virtual Task BatchInsertAsync<T>(IEnumerable<T> array) where T : CollectionEntityBase, new()
        {
            return this.CurrentCollection<T>().InsertManyAsync(array);
        }
        public virtual long Update<T>(Dictionary<Expression<Func<T, object>>, object> dic, Expression<Func<T, bool>> filter) where T : CollectionEntityBase, new()
        {
            List<UpdateDefinition<T>> list = new List<UpdateDefinition<T>>();
            foreach (var item in dic)
            {
                list.Add(Builders<T>.Update.Set(item.Key, item.Value));
            }
            var updates = Builders<T>.Update.Combine(list).CurrentDate("lastModified");
            return this.CurrentCollection<T>().UpdateMany<T>(filter, updates).ModifiedCount;
        }
        public virtual void UpdateAsync<T>(T model, Expression<Func<T, bool>> filter) where T : CollectionEntityBase, new()
        {
            this.CurrentCollection<T>().UpdateManyAsync<T>(filter, new ObjectUpdateDefinition<T>(model));
        }
        public virtual long Delete<T>(Expression<Func<T, bool>> filter) where T : CollectionEntityBase, new()
        {
            return this.CurrentCollection<T>().DeleteMany<T>(filter).DeletedCount;
        }
        public virtual void DeleteAsync<T>(Expression<Func<T, bool>> filter) where T : CollectionEntityBase, new()
        {
            this.CurrentCollection<T>().DeleteManyAsync<T>(filter);
        }
    }
}
