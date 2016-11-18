using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MongodbHelper
{

    public class FindFluentProxy<T>
    {
        public FindFluentProxy(IFindFluent<T, T> findFluent)
        {
            this._findFluent = findFluent;
        }
        private IFindFluent<T, T> _findFluent { get; set; }

        public FindFluentProxy<T> Sort(Expression<Func<T, object>> sort)
        {
            _findFluent = this._findFluent.SortBy(sort);
            return this;
        }

        public FindFluentProxy<T> SortByDescending(Expression<Func<T, object>> sort)
        {
            _findFluent = this._findFluent.SortByDescending(sort);
            return this;
        }

        public FindFluentProxy<T> Skip(int skip)
        {
            _findFluent = this._findFluent.Skip(skip);
            return this;
        }

        public FindFluentProxy<T> Take(int take)
        {
            _findFluent = this._findFluent.Limit(take);
            return this;
        }

        public T First()
        {
            return this._findFluent.First();
        }

        public T FirstOrDefault()
        {
            return this._findFluent.FirstOrDefault();
        }

        public Task<T> FirstAsync()
        {
            return this._findFluent.FirstAsync();
        }
        public Task<T> FirstOrDefaultAsync()
        {
            return this._findFluent.FirstOrDefaultAsync();
        }


        public T Single()
        {
            return this._findFluent.Single();
        }
        public T SingleOrDefault()
        {
            return this._findFluent.SingleOrDefault();
        }
        public Task<T> SingleAsync()
        {
            return this._findFluent.SingleAsync();
        }
        public Task<T> SingleOrDefaultAsync()
        {
            return this._findFluent.SingleOrDefaultAsync();
        }

        public IEnumerable<T> ToEnumerable()
        {
            return this._findFluent.ToEnumerable();
        }
        public List<T> ToList()
        {
            return this._findFluent.ToList();
        }
        public Task<List<T>> ToListAsync()
        {
            return this._findFluent.ToListAsync();
        }
        public long Count()
        {
            return this._findFluent.Count();
        }
        public Task<long> CountAsync()
        {
            return this._findFluent.CountAsync();
        }
    }
}
