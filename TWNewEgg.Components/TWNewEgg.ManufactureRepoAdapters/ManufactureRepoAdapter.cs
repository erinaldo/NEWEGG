using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.ManufactureRepoAdapters.Interface;

namespace TWNewEgg.ManufactureRepoAdapters
{
    public class ManufactureRepoAdapter : IManufactureRepoAdapter
    {
        private IRepository<Manufacture> _ManuDb;

        public ManufactureRepoAdapter(IRepository<Manufacture> argManuDb)
        {
            this._ManuDb = argManuDb;
        }

        public IQueryable<Manufacture> GetAll()
        {
            IQueryable<Manufacture> queryResult = null;

            queryResult = this._ManuDb.GetAll();

            return queryResult;
        }

        public IQueryable<Manufacture> GetById(int argNumId)
        {
            IQueryable<Manufacture> queryResult = null;

            queryResult = this.GetAll().Where(x => x.ID == argNumId);

            return queryResult;
        }

        public bool Update(Manufacture argObjManufacture)
        {
            bool boolExec = false;

            try
            {
                this._ManuDb.Update(argObjManufacture);
                boolExec = true;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return boolExec;
        }

        public void Create(Manufacture argObjManufacture)
        {
            try
            {
                this._ManuDb.Create(argObjManufacture);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
        }
    }
}
