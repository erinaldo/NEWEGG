using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace TWNewEgg.DAL.Interface
{
    public interface IDbContextFactory<TContext> where TContext : DbContext
    {
        TContext GetDbContext();
    }
}
