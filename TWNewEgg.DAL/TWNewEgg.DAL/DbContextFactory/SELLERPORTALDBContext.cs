using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;

namespace TWNewEgg.DAL.DbContextFactory
{
    public class SELLERPORTALDBContext<TContext> : IDbContextFactory<TContext>
        where TContext : DbContext
    {
        private string _ConnectionString = string.Empty;

        public SELLERPORTALDBContext(string connectionString)
        {
            this._ConnectionString = connectionString;
        }

        private TContext _dbContext;
        private TContext dbContext
        {
            get
            {
                if (this._dbContext == null)
                {
                    Type t = typeof(TContext);
                    this._dbContext =
                        (TContext)Activator.CreateInstance(t, this._ConnectionString);
                }
                return _dbContext;
            }
        }

        public TContext GetDbContext()
        {
            return this.dbContext;
        }
    }
}
