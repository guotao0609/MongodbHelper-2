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
        protected virtual IQueryable<TEntity> GetIQueryable()
        {
            return this.GetIQueryable<TEntity>();
        }
        protected virtual IQueryable<TOtherEntity> GetIQueryable<TOtherEntity>() where TOtherEntity : CollectionEntityBase, new()
        {
            return this.DbInstance.GetIQueryable<TOtherEntity>();
        }
        protected virtual TResult BaseQuery<TResult>(Func<MongoCollectionProxy<TEntity>, TResult> func)
        {
            return this.BaseQuery<TEntity, TResult>(func);
        }
        protected virtual TResult BaseQuery<TOtherEntity, TResult>(Func<MongoCollectionProxy<TOtherEntity>, TResult> func) where TOtherEntity : CollectionEntityBase, new()
        {
            try
            {
                return this.DbInstance.Query<TOtherEntity, TResult>(func);
            }
            catch (Exception ex)
            {
                if (this.OnException != null)
                    this.OnException.Invoke(this, new BllEventArgs(ex));
            }
            return default(TResult);
        }
        protected virtual TResult BaseQueryExt<TResult>(Func<IQueryable<TEntity>, TResult> func)
        {
            return this.BaseQueryExt<TEntity, TResult>(func);
        }
        protected virtual TResult BaseQueryExt<TOtherEntity, TResult>(Func<IQueryable<TOtherEntity>, TResult> func) where TOtherEntity : CollectionEntityBase, new()
        {
            try
            {
                return this.DbInstance.QueryExt<TOtherEntity, TResult>(func);
            }
            catch (Exception ex)
            {
                if (this.OnException != null)
                    this.OnException.Invoke(this, new BllEventArgs(ex));
            }
            return default(TResult);
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

        public virtual void BatchInsert(IEnumerable<TEntity> array)
        {
            this.BatchInsert<TEntity>(array);
        }
        public virtual void BatchInsert<TOtherEntity>(IEnumerable<TOtherEntity> array) where TOtherEntity : CollectionEntityBase, new()
        {
            this.DbInstance.BatchInsert<TOtherEntity>(array);
        }
        public virtual long Delete(Expression<Func<TEntity, bool>> filter)
        {
            return this.Delete<TEntity>(filter);
        }
        public virtual long Delete<TOtherEntity>(Expression<Func<TOtherEntity, bool>> filter) where TOtherEntity : CollectionEntityBase, new()
        {
            return this.DbInstance.Delete<TOtherEntity>(filter);
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
