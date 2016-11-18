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

        public virtual List<T> QueryList(Expression<Func<T, bool>> filter)
        {
            return this.QueryBase<List<T>>(q => q.Where(filter).ToList());
        }
        public virtual List<T> Query(Func<MongoCollectionProxy<T>, List<T>> func)
        {
            return this.QueryBase<List<T>>(func);
        }
        protected virtual M QueryBase<M>(Func<MongoCollectionProxy<T>, M> func)
        {
            return this.QueryBase<T, M>(func);
        }
        protected virtual M QueryBase<E, M>(Func<MongoCollectionProxy<E>, M> func) where E : CollectionEntityBase, new()
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

        public virtual List<T> QueryExt(Func<IQueryable<T>, List<T>> func)
        {
            return this.QueryBaseExt<List<T>>(func);
        }
        protected virtual M QueryBaseExt<M>(Func<IQueryable<T>, M> func)
        {
            return this.QueryBaseExt<T, M>(func);
        }
        protected virtual M QueryBaseExt<E, M>(Func<IQueryable<E>, M> func) where E : CollectionEntityBase, new()
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

        public virtual long? QueryCount(Expression<Func<T, bool>> filter)
        {
            try
            {
                return this.DbInstance.QueryCount<T>(filter);
            }
            catch (Exception ex)
            {
                if (this.OnException != null)
                    this.OnException.Invoke(this, new BllEventArgs(ex));
            }
            return default(long?);
        }
        public virtual long? QueryCountExt(Func<IQueryable<T>, long> func)
        {
            return this.QueryBaseExt<long>(func);
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
