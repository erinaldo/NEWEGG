using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.DAL.Interface
{
    public interface ITWSELLERPORTALDBRepository<TEntity> : IDisposable
        where TEntity : class
    {
        void Create(TEntity instance);

        void CreateMany(IEnumerable<TEntity> instance);

        void CreateRange(List<TEntity> instance);

        void Update(TEntity instance);

        void UpdateMany(IEnumerable<TEntity> instance);

        void UpdateRange(List<TEntity> instance);

        void Delete(TEntity instance);

        TEntity Get(Expression<Func<TEntity, bool>> predicate);

        IQueryable<TEntity> GetAll();

        System.Data.Entity.Database GetDatabase();

        DbContext GetContext();
    }
}
