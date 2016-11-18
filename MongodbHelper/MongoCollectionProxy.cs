using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MongodbHelper
{
    public class MongoCollectionProxy<T>
    {
        public MongoCollectionProxy(IMongoCollection<T> mongoCollection)
        {
            this._mongoCollection = mongoCollection;
        }
        private IMongoCollection<T> _mongoCollection;
        private FindFluentProxy<T> _findFluentProxy;
        protected FindFluentProxy<T> FindFluentProxy
        {
            get
            {
                if (this._findFluentProxy == null)
                    this._findFluentProxy = new FindFluentProxy<T>(this._mongoCollection.Find<T>(this._expression == null ? (t => true) : this._expression));
                return this._findFluentProxy;
            }
        }
        private Expression<Func<T, bool>> _expression;
        private Expression<M> AndAlso<M>(Expression<M> left, Expression<M> right)
        {
            return Expression.Lambda<M>(Expression.AndAlso(left.Body, right.Body), left.Parameters);
        }
        public MongoCollectionProxy<T> Where(Expression<Func<T, bool>> where)
        {
            if (this._expression == null)
                this._expression = where;
            else
                this._expression = this.AndAlso<Func<T, bool>>(this._expression, where);
            return this;
        }
        public FindFluentProxy<T> Sort(Expression<Func<T, object>> sort)
        {
            return this.FindFluentProxy.Sort(sort);
        }
        public FindFluentProxy<T> SortByDescending(Expression<Func<T, object>> sort)
        {
            return this.FindFluentProxy.SortByDescending(sort);
        }

        public FindFluentProxy<T> Skip(int skip)
        {
            return this.FindFluentProxy.Skip(skip);
        }

        public FindFluentProxy<T> Take(int take)
        {
            return this.FindFluentProxy.Take(take);
        }

        public T First()
        {
            return this.FindFluentProxy.First();
        }

        public T FirstOrDefault()
        {
            return this.FindFluentProxy.FirstOrDefault();
        }

        public Task<T> FirstAsync()
        {
            return this.FindFluentProxy.FirstAsync();
        }
        public Task<T> FirstOrDefaultAsync()
        {
            return this.FindFluentProxy.FirstOrDefaultAsync();
        }


        public T Single()
        {
            return this.FindFluentProxy.Single();
        }
        public T SingleOrDefault()
        {
            return this.FindFluentProxy.SingleOrDefault();
        }
        public Task<T> SingleAsync()
        {
            return this.FindFluentProxy.SingleAsync();
        }
        public Task<T> SingleOrDefaultAsync()
        {
            return this.FindFluentProxy.SingleOrDefaultAsync();
        }

        public IEnumerable<T> ToEnumerable()
        {
            return this.FindFluentProxy.ToEnumerable();
        }
        public List<T> ToList()
        {
            return this.FindFluentProxy.ToList();
        }
        public Task<List<T>> ToListAsync()
        {
            return this.FindFluentProxy.ToListAsync();
        }
        public long Count()
        {
            return this._mongoCollection.Count<T>(this._expression == null ? (t => true) : this._expression);
        }
        public Task<long> CountAsync()
        {
            return this._mongoCollection.CountAsync<T>(this._expression == null ? (t => true) : this._expression);
        }
    }
}
