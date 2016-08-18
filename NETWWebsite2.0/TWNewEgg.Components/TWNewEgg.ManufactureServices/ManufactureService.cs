using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels;
using TWNewEgg.Models.DomainModels.Manufacture;
using TWNewEgg.ManufactureRepoAdapters.Interface;
using TWNewEgg.ManufactureRepoAdapters;
using TWNewEgg.ManufactureServices.Interface;
using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.ManufactureServices
{
    public class ManufactureService: IManufactureService
    {
        private IManufactureRepoAdapter _ManuRepo = null;

        public ManufactureService(IManufactureRepoAdapter argMenuRepo)
        {
            this._ManuRepo = argMenuRepo;
        }


        public List<Manufacture> GetAll()
        {
            List<Manufacture> listResult = null;
            List<Models.DBModels.TWSQLDB.Manufacture> listSearch = null;

            try
            {
                listSearch = this._ManuRepo.GetAll().ToList();
                if (listSearch != null && listSearch.Count > 0)
                {
                    listResult = ModelConverter.ConvertTo<List<Manufacture>>(listSearch);
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message ,ex);
            }

            return listResult;
        }

        public Manufacture GetById(int argNumId)
        {
            if (argNumId <= 0)
            {
                return null;
            }

            Manufacture objResult = null;
            Models.DBModels.TWSQLDB.Manufacture objSearch = null;

            try
            {
                objSearch = this._ManuRepo.GetAll().Where(x=>x.ID == argNumId).FirstOrDefault();
                if (objSearch != null)
                {
                    objResult = ModelConverter.ConvertTo<Manufacture>(objSearch);
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return objResult;
        }

        public void Create(Manufacture argObjManu)
        {
            if (argObjManu == null)
            {
                return;
            }

            TWNewEgg.Models.DBModels.TWSQLDB.Manufacture objManu = null;

            try
            {
                argObjManu.CreateDate = DateTime.Now;
                objManu = ModelConverter.ConvertTo<TWNewEgg.Models.DBModels.TWSQLDB.Manufacture>(argObjManu);

                this._ManuRepo.Create(objManu);
                
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
        }

        public bool Update(Manufacture argObjManu)
        {
            if (argObjManu == null)
            {
                return false;
            }

            TWNewEgg.Models.DBModels.TWSQLDB.Manufacture objManu = null;
            bool boolExec = false;

            try
            {
                argObjManu.CreateDate = DateTime.Now;
                objManu = ModelConverter.ConvertTo<TWNewEgg.Models.DBModels.TWSQLDB.Manufacture>(argObjManu);

                boolExec = this._ManuRepo.Update(objManu);

            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return boolExec;
        }
    }
}
