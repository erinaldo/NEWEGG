using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.BankRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.BankRepoAdapters
{
    public class BankRepoAdapter : IBankRepoAdapter
    {
        private IRepository<Bank> _bank;

        public BankRepoAdapter(IRepository<Bank> bank)
        {
            this._bank = bank;
        }

        public IQueryable<Bank> GetAll()
        {
            return this._bank.GetAll();
        }
    }
}
