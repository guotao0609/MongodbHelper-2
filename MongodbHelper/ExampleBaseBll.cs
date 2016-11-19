using MongodbHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MongodbHelper
{
    public abstract class ExampleBaseBll<T> where T : CollectionEntityBase, new()
    {
        public ExampleBaseBll(MongodbAccess acc)
        {
            this.DbInstance = acc;
        }
        protected MongodbAccess DbInstance { get; set; }
        protected event EventHandler OnException;

        protected virtual M BaseQuery<M>(Func<MongoCollectionProxy<T>, M> func)
        {
            return this.BaseQuery<T, M>(func);
        }
        protected virtual M BaseQuery<E, M>(Func<MongoCollectionProxy<E>, M> func) where E : CollectionEntityBase, new()
        {
            try
            {
                return this.DbInstance.Query<E, M>(func);
            }
            catch (Exception ex)
            {
                if (this.OnException != null)
                    this.OnException.Invoke(this, new BllEventArgs(ex));
            }
            return default(M);
        }
        protected virtual M BaseQueryExt<M>(Func<IQueryable<T>, M> func)
        {
            return this.BaseQueryExt<T, M>(func);
        }
        protected virtual M BaseQueryExt<E, M>(Func<IQueryable<E>, M> func) where E : CollectionEntityBase, new()
        {
            try
            {
                return this.DbInstance.QueryExt<E, M>(func);
            }
            catch (Exception ex)
            {
                if (this.OnException != null)
                    this.OnException.Invoke(this, new BllEventArgs(ex));
            }
            return default(M);
        }

        
        public virtual List<T> Query(Expression<Func<T, bool>> filter)
        {
            return this.BaseQuery<List<T>>(q => q.Where(filter).ToList());
        }
        public virtual List<T> QueryExt(Func<IQueryable<T>, List<T>> func)
        {
            return this.BaseQueryExt<List<T>>(func);
        }

        public virtual long QueryCount(Expression<Func<T, bool>> filter)
        {
            return this.BaseQuery<long>(q => q.Where(filter).Count());
        }
        public virtual long? QueryCountExt(Func<IQueryable<T>, long> func)
        {
            return this.BaseQueryExt<long>(func);
        }
    }


    public class BllEventArgs : EventArgs
    {
        public BllEventArgs(Exception ex)
        {
            this.Ex = ex;
        }
        public Exception Ex { get; set; }
    }
}
