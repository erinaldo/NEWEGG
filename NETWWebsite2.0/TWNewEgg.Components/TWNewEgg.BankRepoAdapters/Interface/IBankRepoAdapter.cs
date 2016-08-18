using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.BankRepoAdapters.Interface
{
    public interface IBankRepoAdapter
    {
        IQueryable<Bank> GetAll();
    }
}
