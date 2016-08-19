using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.VendorModels.DBModels;

namespace TWNewEgg.DAL.Repository
{
    public class TWSELLERPORTALDBRepository<TEntity> : ITWSELLERPORTALDBRepository<TEntity>
        where TEntity : class
    {
        protected TWSellerPortalDBContext _context
        {
            get;
            set;
        }

        public TWSELLERPORTALDBRepository(IDbContextFactory<TWSellerPortalDBContext> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("context");
            }
            this._context = factory.GetDbContext();
        }

        /// <summary>
        /// Creates the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <exception cref="System.ArgumentNullException">instance</exception>
        public void Create(TEntity instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                this._context.Set<TEntity>().Add(instance);
                this.SaveChanges();
            }
        }

        public void CreateMany(IEnumerable<TEntity> instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                foreach (TEntity ent in instance)
                {
                    this._context.Set<TEntity>().Add(ent);
                }

                this.SaveChanges();
            }
        }


        public void CreateRange(List<TEntity> instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                foreach (TEntity objSub in instance)
                {
                    this._context.Set<TEntity>().Add(objSub);
                }
                this.SaveChanges();
            }
        }

        /// <summary>
        /// Updates the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Update(TEntity instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                this._context.Entry(instance).State = EntityState.Modified;
                this.SaveChanges();
            }
        }

        /// <summary>
        /// Update Range of instance
        /// </summary>
        /// <param name="instance"></param>
        public void UpdateRange(List<TEntity> instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                foreach (TEntity subInstance in instance)
                {
                    this._context.Entry(subInstance).State = EntityState.Modified;
                }
                this.SaveChanges();
            }
        }

        /// <summary>
        /// Updates the specified instances.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void UpdateMany(IEnumerable<TEntity> instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                foreach (TEntity objSub in instance)
                {
                    this._context.Entry(objSub).State = EntityState.Modified;
                }
                this.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Delete(TEntity instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                this._context.Entry(instance).State = EntityState.Deleted;
                this.SaveChanges();
            }
        }

        /// <summary>
        /// Gets the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public virtual TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            var entity = this._context.Set<TEntity>().FirstOrDefault(predicate);
            return entity;
        }

        public System.Data.Entity.Database GetDatabase()
        {
            return this._context.Database;
        }

        public DbContext GetContext()
        {
            return this._context;
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetAll()
        {
            return this._context.Set<TEntity>().AsQueryable();
        }


        protected void SaveChanges()
        {
            this._context.SaveChanges();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._context != null)
                {
                    this._context.Dispose();
                    this._context = null;
                }
            }
        }
    }
}
