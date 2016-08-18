using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.HiTrustRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.HiTrustRepoAdapters
{
    public class HiTrustRepoAdapter : IHiTrustRepoAdapter
    {
        private IRepository<HiTrust> _hiTrustRepo;
        private IRepository<HiTrustQuery> _hiTrustQueryRepo;
        private IRepository<HiTrustTrans> _hiTrustTransRepo;
        private IRepository<HiTrustQueryLog> _hiTrustQueryLogRepo;
        private IRepository<HiTrustTransLog> _hiTrustTransLogRepo;
        public HiTrustRepoAdapter(IRepository<HiTrust> hiTrustRepo, IRepository<HiTrustQuery> hiTrustQueryRepo, IRepository<HiTrustTrans> hiTrustTransRepo, IRepository<HiTrustQueryLog> hiTrustQueryLogRepo, IRepository<HiTrustTransLog> hiTrustTransLogRepo)
        {
            this._hiTrustRepo = hiTrustRepo;
            this._hiTrustQueryRepo = hiTrustQueryRepo;
            this._hiTrustTransRepo = hiTrustTransRepo;
            this._hiTrustQueryLogRepo = hiTrustQueryLogRepo;
            this._hiTrustTransLogRepo = hiTrustTransLogRepo;
        }

        public IQueryable<HiTrust> GetAllHiTrustSetting()
        {
            return this._hiTrustRepo.GetAll();
        }

        public IQueryable<HiTrustTrans> GetAllHiTrustTrans()
        {
            return this._hiTrustTransRepo.GetAll();
        }

        public IQueryable<HiTrustQuery> GetAllHiTrustQuery()
        {
            return this._hiTrustQueryRepo.GetAll();
        }

        public HiTrustTrans UpdateHiTrustTrans(HiTrustTrans HiTrustTransData)
        {
            try
            {
                //檢查是否有MerConfigName
                if (string.IsNullOrEmpty(HiTrustTransData.MerConfigName))
                {
                    throw new Exception("MerConfigName is Null!!!");
                }
                //檢查是否有ordernumber
                if (string.IsNullOrEmpty(HiTrustTransData.ordernumber))
                {
                    throw new Exception("ordernumber is Null!!!");
                }

                //檢查key(MerConfigName, ordernumber)是否已經存在
                //HiTrustTrans HiData = new HiTrustTrans();
                //HiData = _hiTrustTransRepo.Get(x => x.MerConfigName == HiTrustTransData.MerConfigName && x.ordernumber == HiTrustTransData.ordernumber);
                if (!_hiTrustTransRepo.GetAll().Any(x => x.ordernumber == HiTrustTransData.ordernumber))
                {
                    HiTrustTransData.UpdateDate = DateTime.Now;
                    HiTrustTransData.CreateUser = HiTrustTransData.UpdateUser;
                    _hiTrustTransRepo.Create(HiTrustTransData);
                }
                else
                {
                    //AutoMapper.Mapper.Map(HiTrustTransData, HiData);
                    //ModelConverter.ConvertTo<HiTrustTrans, HiTrustTrans>(HiTrustTransData, HiData);
                    HiTrustTransData.UpdateDate = DateTime.Now;
                    _hiTrustTransRepo.Update(HiTrustTransData);
                }

                return HiTrustTransData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HiTrustQuery UpdateHiTrustQuery(HiTrustQuery HiTrustQueryData)
        {
            try
            {
                //檢查是否有ordernumber
                if (string.IsNullOrEmpty(HiTrustQueryData.ordernumber))
                {
                    throw new Exception("ordernumber is Null!!!");
                }

                //檢查key(MerConfigName, ordernumber)是否已經存在
                //HiTrustQuery HiData = new HiTrustQuery();
                //HiData = _hiTrustQueryRepo.Get(x => x.MerConfigName == HiTrustQueryData.MerConfigName && x.ordernumber == HiTrustQueryData.ordernumber);
                if (!_hiTrustQueryRepo.GetAll().Any(x => x.ordernumber == HiTrustQueryData.ordernumber))
                {
                    HiTrustQueryData.UpdateDate = DateTime.Now;
                    HiTrustQueryData.CreateUser = HiTrustQueryData.UpdateUser;
                    _hiTrustQueryRepo.Create(HiTrustQueryData);
                }
                else
                {
                    //AutoMapper.Mapper.Map(HiTrustQueryData, HiData);
                    //ModelConverter.ConvertTo<HiTrustQuery, HiTrustQuery>(HiTrustQueryData, HiData);
                    HiTrustQueryData.UpdateDate = DateTime.Now;
                    _hiTrustQueryRepo.Update(HiTrustQueryData);
                }

                return HiTrustQueryData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HiTrustTransLog UpdateHiTrustTransLog(HiTrustTransLog HiTrustTransLogData)
        {
            try
            {
                //檢查key(MerConfigName, ordernumber)是否已經存在
                HiTrustTransLogData.UpdateDate = DateTime.Now;
                HiTrustTransLogData.CreateUser = HiTrustTransLogData.UpdateUser;
                _hiTrustTransLogRepo.Create(HiTrustTransLogData);

                return HiTrustTransLogData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HiTrustQueryLog UpdateHiTrustQueryLog(HiTrustQueryLog HiTrustQueryLogData)
        {
            try
            {
                //檢查key(MerConfigName, ordernumber)是否已經存在
                HiTrustQueryLogData.UpdateDate = DateTime.Now;
                HiTrustQueryLogData.CreateUser = HiTrustQueryLogData.UpdateUser;
                _hiTrustQueryLogRepo.Create(HiTrustQueryLogData);

                return HiTrustQueryLogData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
