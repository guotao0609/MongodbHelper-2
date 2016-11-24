using MongodbHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MongodbHelper
{
    public abstract class ExampleBaseBll<TEntity> where TEntity : CollectionEntityBase, new()
    {
        public ExampleBaseBll(MongodbAccess acc)
        {
            this.DbInstance = acc;
        }
        protected MongodbAccess DbInstance { get; set; }
        protected event EventHandler OnException;

        protected virtual M BaseQuery<M>(Func<MongoCollectionProxy<TEntity>, M> func)
        {
            return this.BaseQuery<TEntity, M>(func);
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
        protected virtual M BaseQueryExt<M>(Func<IQueryable<TEntity>, M> func)
        {
            return this.BaseQueryExt<TEntity, M>(func);
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

        
        public virtual List<TEntity> Query(Expression<Func<TEntity, bool>> filter)
        {
            return this.BaseQuery<List<TEntity>>(q => q.Where(filter).ToList());
        }
        public virtual List<TEntity> QueryExt(Func<IQueryable<TEntity>, List<TEntity>> func)
        {
            return this.BaseQueryExt<List<TEntity>>(func);
        }

        public virtual long QueryCount(Expression<Func<TEntity, bool>> filter)
        {
            return this.BaseQuery<long>(q => q.Where(filter).Count());
        }
        public virtual long? QueryCountExt(Func<IQueryable<TEntity>, long> func)
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
