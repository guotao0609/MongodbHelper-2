using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MongodbHelper
{
    public class MongoCollectionProxy<TDocument>
    {
        public MongoCollectionProxy(IMongoCollection<TDocument> mongoCollection)
        {
            this._mongoCollection = mongoCollection;
        }
        private IMongoCollection<TDocument> _mongoCollection;
        private FindFluentProxy<TDocument> _findFluentProxy;
        protected FindFluentProxy<TDocument> FindFluentProxy
        {
            get
            {
                if (this._findFluentProxy == null)
                    this._findFluentProxy = new FindFluentProxy<TDocument>(this._mongoCollection.Find<TDocument>(this._expression == null ? (t => true) : this._expression));
                return this._findFluentProxy;
            }
        }
        private Expression<Func<TDocument, bool>> _expression;
        private Expression<Func<TDocument, bool>> And(Expression<Func<TDocument, bool>> expr1, Expression<Func<TDocument, bool>> expr2)
        {
            var paramExpr = Expression.Parameter(typeof(TDocument));
            var exprBody = Expression.And(expr1.Body, expr2.Body);
            exprBody = (BinaryExpression)new ParameterVisitor(paramExpr).Visit(exprBody);
            return Expression.Lambda<Func<TDocument, bool>>(exprBody, paramExpr);
        }
        public MongoCollectionProxy<TDocument> Where(Expression<Func<TDocument, bool>> where)
        {
            if (this._expression == null)
                this._expression = where;
            else
                this._expression = this.And(this._expression, where);
            return this;
        }
        public FindFluentProxy<TDocument> Sort(Expression<Func<TDocument, object>> sort)
        {
            return this.FindFluentProxy.Sort(sort);
        }
        public FindFluentProxy<TDocument> SortByDescending(Expression<Func<TDocument, object>> sort)
        {
            return this.FindFluentProxy.SortByDescending(sort);
        }

        public AggregateFluentProxy<TDocument> Aggregate()
        {
            return new AggregateFluentProxy<TDocument>(this._mongoCollection.Aggregate());
        }

        public FindFluentProxy<TDocument> Skip(int skip)
        {
            return this.FindFluentProxy.Skip(skip);
        }

        public FindFluentProxy<TDocument> Limit(int limit)
        {
            return this.FindFluentProxy.Limit(limit);
        }

        public TDocument First()
        {
            return this.FindFluentProxy.First();
        }

        public TDocument FirstOrDefault()
        {
            return this.FindFluentProxy.FirstOrDefault();
        }

        public Task<TDocument> FirstAsync()
        {
            return this.FindFluentProxy.FirstAsync();
        }
        public Task<TDocument> FirstOrDefaultAsync()
        {
            return this.FindFluentProxy.FirstOrDefaultAsync();
        }


        public TDocument Single()
        {
            return this.FindFluentProxy.Single();
        }
        public TDocument SingleOrDefault()
        {
            return this.FindFluentProxy.SingleOrDefault();
        }
        public Task<TDocument> SingleAsync()
        {
            return this.FindFluentProxy.SingleAsync();
        }
        public Task<TDocument> SingleOrDefaultAsync()
        {
            return this.FindFluentProxy.SingleOrDefaultAsync();
        }

        public IEnumerable<TDocument> ToEnumerable()
        {
            return this.FindFluentProxy.ToEnumerable();
        }
        public List<TDocument> ToList()
        {
            return this.FindFluentProxy.ToList();
        }
        public Task<List<TDocument>> ToListAsync()
        {
            return this.FindFluentProxy.ToListAsync();
        }
        public long Count()
        {
            return this._mongoCollection.Count<TDocument>(this._expression == null ? (t => true) : this._expression);
        }
        public Task<long> CountAsync()
        {
            return this._mongoCollection.CountAsync<TDocument>(this._expression == null ? (t => true) : this._expression);
        }
    }
}
