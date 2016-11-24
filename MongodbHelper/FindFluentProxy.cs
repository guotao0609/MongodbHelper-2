using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MongodbHelper
{

    public class FindFluentProxy<TResult>
    {
        public FindFluentProxy(IFindFluent<TResult, TResult> findFluent)
        {
            this._findFluent = findFluent;
        }
        private IFindFluent<TResult, TResult> _findFluent { get; set; }

        public FindFluentProxy<TResult> Sort(Expression<Func<TResult, object>> sort)
        {
            _findFluent = this._findFluent.SortBy(sort);
            return this;
        }

        public FindFluentProxy<TResult> SortByDescending(Expression<Func<TResult, object>> sort)
        {
            _findFluent = this._findFluent.SortByDescending(sort);
            return this;
        }

        public FindFluentProxy<TResult> Skip(int skip)
        {
            _findFluent = this._findFluent.Skip(skip);
            return this;
        }

        public FindFluentProxy<TResult> Limit(int limit)
        {
            _findFluent = this._findFluent.Limit(limit);
            return this;
        }

        public TResult First()
        {
            return this._findFluent.First();
        }

        public TResult FirstOrDefault()
        {
            return this._findFluent.FirstOrDefault();
        }

        public Task<TResult> FirstAsync()
        {
            return this._findFluent.FirstAsync();
        }
        public Task<TResult> FirstOrDefaultAsync()
        {
            return this._findFluent.FirstOrDefaultAsync();
        }


        public TResult Single()
        {
            return this._findFluent.Single();
        }
        public TResult SingleOrDefault()
        {
            return this._findFluent.SingleOrDefault();
        }
        public Task<TResult> SingleAsync()
        {
            return this._findFluent.SingleAsync();
        }
        public Task<TResult> SingleOrDefaultAsync()
        {
            return this._findFluent.SingleOrDefaultAsync();
        }

        public IEnumerable<TResult> ToEnumerable()
        {
            return this._findFluent.ToEnumerable();
        }
        public List<TResult> ToList()
        {
            return this._findFluent.ToList();
        }
        public Task<List<TResult>> ToListAsync()
        {
            return this._findFluent.ToListAsync();
        }
    }
}
