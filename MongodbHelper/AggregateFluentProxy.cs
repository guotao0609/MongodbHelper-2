using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MongodbHelper
{
    public class AggregateFluentProxy<TResult>
    {
        private IAggregateFluent<TResult> _aggregateFluent;
        public AggregateFluentProxy(IAggregateFluent<TResult> aggregateFluent)
        {
            this._aggregateFluent = aggregateFluent;
        }
        public TResult First()
        {
            return this._aggregateFluent.First();
        }
        public Task<TResult> FirstAsync()
        {
            return this._aggregateFluent.FirstAsync();
        }
        public TResult FirstOrDefault()
        {
            return this._aggregateFluent.FirstOrDefault();
        }
        public Task<TResult> FirstOrDefaultAsync()
        {
            return this._aggregateFluent.FirstOrDefaultAsync();
        }
        public AggregateFluentProxy<TResult> Group<TNewResult>()
        {
            throw new Exception("Group undone");
        }
        public AggregateFluentProxy<TResult> Limit(int limit)
        {
            this._aggregateFluent = this._aggregateFluent.Limit(limit);
            return this;
        }
        public AggregateFluentProxy<TResult> Lookup<TNewResult>()
        {
            throw new Exception("Lookup undone");
        }
        public AggregateFluentProxy<TResult> Match(Expression<Func<TResult, bool>> filter)
        {
            this._aggregateFluent = this._aggregateFluent.Match(filter);
            return this;
        }
        public void Out(string collectionName)
        {
            this._aggregateFluent.Out(collectionName);
        }
        public TResult Single()
        {
            return this._aggregateFluent.Single();
        }
        public Task<TResult> SingleAsync()
        {
            return this._aggregateFluent.SingleAsync();
        }
        public TResult SingleOrDefault()
        {
            return this._aggregateFluent.SingleOrDefault();
        }
        public Task<TResult> SingleOrDefaultAsync()
        {
            return this._aggregateFluent.SingleOrDefaultAsync();
        }
        public AggregateFluentProxy<TResult> Skip(int skip)
        {
            this._aggregateFluent = this._aggregateFluent.Skip(skip);
            return this;
        }
        public AggregateFluentProxy<TResult> Sort(Expression<Func<TResult, object>> sort)
        {
            this._aggregateFluent = this._aggregateFluent.SortBy(sort);
            return this;
        }
        public AggregateFluentProxy<TResult> SortByDescending(Expression<Func<TResult, object>> sort)
        {
            this._aggregateFluent = this._aggregateFluent.SortByDescending(sort);
            return this;
        }
        public IEnumerable<TResult> ToEnumerable()
        {
            return this._aggregateFluent.ToEnumerable();
        }
        public List<TResult> ToList()
        {
            return this._aggregateFluent.ToList();
        }
        public Task<List<TResult>> ToListAsync()
        {
            return this._aggregateFluent.ToListAsync();
        }
        public void Unwind()
        {
            throw new Exception("Unwind undone");
        }
    }
}
