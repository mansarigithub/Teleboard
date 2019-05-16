using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Teleboard.DataAccess.Context;

namespace Teleboard.Business.Core
{
    public abstract class BizBase<TEntity> where TEntity: class, new()
    {
        private ApplicationDbContext Context { get; set; }

        protected BizBase(ApplicationDbContext context)
        {
            Context= context;
        }

        #region Read
        public TEntity Find(params object[] keyValues)
        {
            return Context.Set<TEntity>().Find(keyValues);
        }

        public TEntity ReadSingle(Expression<Func<TEntity,bool>> predicate)
        {
            return Context.Set<TEntity>().Single(predicate);
        }

        public TEntity ReadSingleOrDefault(Expression<Func<TEntity,bool>> predicate)
        {
            return Context.Set<TEntity>().SingleOrDefault(predicate);
        }

        public IQueryable<TEntity> Read(bool noTracking = false)
        {
            return Context.Set<TEntity>().AsQueryable();
        }

        public IQueryable<TEntity> Read(Expression<Func<TEntity,bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate);
        }

        #endregion

        #region Any
        public bool Any(System.Linq.Expressions.Expression<Func<TEntity,bool>> predicate)
        {
            return Context.Set<TEntity>().Any(predicate);
        }
        #endregion

        #region Insert
        public TEntity Add(TEntity entity)
        {
            return Context.Set<TEntity>().Add(entity);
        }
        #endregion


        #region Remove
        public TEntity Remove(TEntity entity)
        {
            return Context.Set<TEntity>().Remove(entity);
        }
        #endregion

        #region Include
        public IQueryable<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> path)
        {
            return Context.Set<TEntity>().Include(path);
        }
        #endregion

        #region Save
        public int SaveChanges()
        {
            return Context.SaveChanges();
        }
        #endregion

    }
}
