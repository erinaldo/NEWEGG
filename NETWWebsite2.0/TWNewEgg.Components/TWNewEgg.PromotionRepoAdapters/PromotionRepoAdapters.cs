using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.PromotionRepoAdapters.Interface;
using TWNewEgg.Framework.AutoMapper;
using System.Data.Entity;

namespace TWNewEgg.PromotionRepoAdapters
{
    public class PromotionRepoAdapter : IPromotionRepoAdapter
    {
        private IRepository<PromotionGiftBasic> _promotionRepo;
        private IRepository<PromotionGiftBlackList> _promotionGiftBlackListRepo;
        private IRepository<PromotionGiftWhiteList> _promotionGiftWhiteListRepo;
        private IRepository<PromotionGiftInterval> _promotionGiftIntervalRepo;
        private IRepository<PromotionGiftRecords> _PromotionGiftRecRepo;

        public PromotionRepoAdapter(IRepository<PromotionGiftBasic> promotionRepo, IRepository<PromotionGiftBlackList> promotionBlackListRepo, IRepository<PromotionGiftWhiteList> promotionGiftWhiteListRepo, IRepository<PromotionGiftInterval> promotionGiftIntervalRepo, IRepository<PromotionGiftRecords> argPromotionGiftRecRepo)
        {
            this._promotionRepo = promotionRepo;
            this._promotionGiftBlackListRepo = promotionBlackListRepo;
            this._promotionGiftWhiteListRepo = promotionGiftWhiteListRepo;
            this._promotionGiftIntervalRepo = promotionGiftIntervalRepo;
            this._PromotionGiftRecRepo = argPromotionGiftRecRepo;
        }

        public Database GetDatabase()
        {
            return this._promotionRepo.GetDatabase();
        }

        #region PromotionGiftBasic優惠活動基本資料

        public void CreatePromotionGiftBasic(Models.DBModels.TWSQLDB.PromotionGiftBasic argObjGiftBasic)
        {
            if (argObjGiftBasic == null)
            {
                return;
        }

            try
        {
                this._promotionRepo.Create(argObjGiftBasic);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdatePromotionGiftBasic(Models.DBModels.TWSQLDB.PromotionGiftBasic argObjGiftBasic)
        {
            if (argObjGiftBasic == null)
            {
                return false;
            }

            Models.DBModels.TWSQLDB.PromotionGiftBasic objUpdateBasic = null;
            bool boolExec = false;

            objUpdateBasic = this._promotionRepo.GetAll().Where(x => x.ID == argObjGiftBasic.ID).FirstOrDefault();
            if (objUpdateBasic != null)
            {
                try
                {
                    ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftBasic, Models.DBModels.TWSQLDB.PromotionGiftBasic>(argObjGiftBasic, objUpdateBasic);
                    this._promotionRepo.Update(objUpdateBasic);
                    boolExec = true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return boolExec;
        }

        public IQueryable<PromotionGiftBasic> GetAllPromotionGiftBasic()
        {
            return this._promotionRepo.GetAll();
        }

        public IQueryable<PromotionGiftBasic> GetPromotionGiftBasicByDate(PromotionGiftBasic.UsedStatus usedStatus)
        {
            DateTime dt = DateTime.UtcNow.AddHours(8);
            IQueryable<PromotionGiftBasic> result = this._promotionRepo
                .GetAll()
                .Where(x =>
                    x.Status == (int)usedStatus
                    && x.StartDate <= dt && x.EndDate > dt);

            return result;
        }

        public IQueryable<PromotionGiftBasic> GetPromotionGiftBasicByCategoryID(int categoryId)
        {
            DateTime dt = DateTime.UtcNow.AddHours(8);
            string cidString = ";" + categoryId.ToString() + ";";
            // 取得時間內且categoryId符合活動條件的優惠活動資訊(Categories為0則表示全館皆可使用該活動優惠)
            IQueryable<PromotionGiftBasic> result = this._promotionRepo
                .GetAll()
                .Where(x =>
                    x.Status == (int)PromotionGiftBasic.UsedStatus.Used
                    && x.StartDate <= dt && x.EndDate > dt
                    && ((";" + x.Categories + ";").Contains(cidString) || (";" + x.Categories + ";").Contains(";0;")));

            return result;
        }

        #endregion        

        #region PromotionGiftBlackList優惠活動黑名單

        public void CreatePromotionGiftBlackList(Models.DBModels.TWSQLDB.PromotionGiftBlackList argObjGiftBlackList)
        {
            if (argObjGiftBlackList == null)
            {
                return;
            }

            try
            {
                this._promotionGiftBlackListRepo.Create(argObjGiftBlackList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateRangePromotionGiftBlackList(List<Models.DBModels.TWSQLDB.PromotionGiftBlackList> argListGiftBlackList)
        {
            if (argListGiftBlackList == null || argListGiftBlackList.Count <= 0)
            {
                return;
            }

            try
            {
                this._promotionGiftBlackListRepo.CreateRange(argListGiftBlackList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateRangePromotionGiftBlackList(List<Models.DBModels.TWSQLDB.PromotionGiftBlackList> argListGiftBlackList)
        {
            if (argListGiftBlackList == null || argListGiftBlackList.Count <= 0)
            {
                return false;
            }

            bool boolExec = false;
            List<Models.DBModels.TWSQLDB.PromotionGiftBlackList> listUpdateBlack = null;
            List<int> listNumId = null;

            listNumId = argListGiftBlackList.Select(x => x.ID).ToList();

            listUpdateBlack = this._promotionGiftBlackListRepo.GetAll().Where(x => listNumId.Contains(x.ID)).ToList();

            if (listUpdateBlack != null && listUpdateBlack.Count > 0)
            {
                try
                {
                    ModelConverter.ConvertTo<List<Models.DBModels.TWSQLDB.PromotionGiftBlackList>, List<Models.DBModels.TWSQLDB.PromotionGiftBlackList>>(argListGiftBlackList, listUpdateBlack);
                    this._promotionGiftBlackListRepo.UpdateRange(listUpdateBlack);
                    boolExec = true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return boolExec;
        }

        /// <summary>
        /// 根據PromotionBasicId刪除黑名單
        /// </summary>
        /// <param name="argNumBasicId"></param>
        /// <returns></returns>
        public bool DeletePromotionGiftBlackListByBasicId(int argNumBasicId)
        {
            if (argNumBasicId <= 0)
            {
                return false;
            }

            int numDeleteRow = 0;
            int numEffectRow = 0;
            bool boolExec = false;
            Database objDb = null;

            objDb = this._promotionGiftBlackListRepo.GetDatabase();

            try
            {
                numDeleteRow = objDb.SqlQuery<int>("Select COUNT(DISTINCT ID) From PromotionGiftBlackList Where PromotionGiftBasicID=" + argNumBasicId.ToString()).Single();
                if (numDeleteRow == 0)
                {
                    boolExec = true;
                }
                else
                {
                    numEffectRow = objDb.ExecuteSqlCommand("Delete PromotionGiftBlackList Where PromotionGiftBasicID=" + argNumBasicId.ToString());
                    if (numDeleteRow == numEffectRow)
                    {
                        boolExec = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return boolExec;
        }

        public bool UpdatePromotionGiftBlackList(Models.DBModels.TWSQLDB.PromotionGiftBlackList argObjGiftBlackList)
        {
            if (argObjGiftBlackList == null)
            {
                return false;
            }

            Models.DBModels.TWSQLDB.PromotionGiftBlackList objUpdateBlackList = null;
            bool boolExec = false;

            objUpdateBlackList = this._promotionGiftBlackListRepo.GetAll().Where(x => x.ID == argObjGiftBlackList.ID).FirstOrDefault();
            if (objUpdateBlackList != null)
            {
                try
                {
                    ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftBlackList, Models.DBModels.TWSQLDB.PromotionGiftBlackList>(argObjGiftBlackList, objUpdateBlackList);
                    this._promotionGiftBlackListRepo.Update(objUpdateBlackList);
                    boolExec = true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return boolExec;
        }

        public IQueryable<PromotionGiftBlackList> GetAllPromotionGiftBlackList()
        {
            return this._promotionGiftBlackListRepo.GetAll();
        }

        public IQueryable<PromotionGiftBlackList> GetPromotionGiftBlackList(int basicID)
        {
            IQueryable<PromotionGiftBlackList> result = this._promotionGiftBlackListRepo
                .GetAll()
                .Where(x => x.PromotionGiftBasicID == basicID);

            return result;
        }

        public IQueryable<PromotionGiftBlackList> GetPromotionGiftBlackList(List<int> basicIDList)
        {
            IQueryable<PromotionGiftBlackList> result = this._promotionGiftBlackListRepo
                .GetAll()
                .Where(x => basicIDList.Contains(x.PromotionGiftBasicID));

            return result;
        }
        #endregion

        #region PromotionGiftWhiteList優惠活動白名單

        public void CreatePromotionGiftWhiteList(Models.DBModels.TWSQLDB.PromotionGiftWhiteList argObjGiftWhiteList)
        {
            if (argObjGiftWhiteList == null)
            {
                return;
            }

            try
            {
                this._promotionGiftWhiteListRepo.Create(argObjGiftWhiteList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateRangePromotionGiftWhiteList(List<Models.DBModels.TWSQLDB.PromotionGiftWhiteList> argListGiftWhiteList)
        {
            if (argListGiftWhiteList == null || argListGiftWhiteList.Count <= 0)
            {
                return;
            }

            try
            {
                this._promotionGiftWhiteListRepo.CreateRange(argListGiftWhiteList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateRangePromotionGiftWhiteList(List<Models.DBModels.TWSQLDB.PromotionGiftWhiteList> argListGiftWhiteList)
        {
            if (argListGiftWhiteList == null || argListGiftWhiteList.Count <= 0)
            {
                return false;
            }

            bool boolExec = false;
            List<Models.DBModels.TWSQLDB.PromotionGiftWhiteList> listUpdateWhite = null;
            List<int> listNumId = null;

            listNumId = argListGiftWhiteList.Select(x => x.ID).ToList();

            listUpdateWhite = this._promotionGiftWhiteListRepo.GetAll().Where(x => listNumId.Contains(x.ID)).ToList();

            if (listUpdateWhite != null && listUpdateWhite.Count > 0)
            {
                try
                {
                    ModelConverter.ConvertTo<List<Models.DBModels.TWSQLDB.PromotionGiftWhiteList>, List<Models.DBModels.TWSQLDB.PromotionGiftWhiteList>>(argListGiftWhiteList, listUpdateWhite);
                    this._promotionGiftWhiteListRepo.UpdateRange(listUpdateWhite);
                    boolExec = true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return boolExec;
        }

        public bool DeletePromotionGiftWhiteListByBasicId(int argNumBasicId)
        {
            if (argNumBasicId <= 0)
            {
                return false;
            }

            int numDeleteRow = 0;
            int numEffectRow = 0;
            bool boolExec = false;
            Database objDb = null;

            objDb = this._promotionGiftWhiteListRepo.GetDatabase();

            try
            {
                numDeleteRow = objDb.SqlQuery<int>("Select COUNT(DISTINCT ID) From PromotionGiftWhiteList Where PromotionGiftBasicID=" + argNumBasicId.ToString()).Single();
                if (numDeleteRow == 0)
                {
                    boolExec = true;
                }
                else
                {
                    numEffectRow = objDb.ExecuteSqlCommand("Delete PromotionGiftWhiteList Where PromotionGiftBasicID=" + argNumBasicId.ToString());
                    if (numDeleteRow == numEffectRow)
                    {
                        boolExec = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return boolExec;
        }

        public bool UpdatePromotionGiftWhiteList(Models.DBModels.TWSQLDB.PromotionGiftWhiteList argObjGiftWhiteList)
        {
            if (argObjGiftWhiteList == null)
            {
                return false;
            }

            Models.DBModels.TWSQLDB.PromotionGiftWhiteList objUpdateWhiteList = null;
            bool boolExec = false;

            objUpdateWhiteList = this._promotionGiftWhiteListRepo.GetAll().Where(x => x.ID == argObjGiftWhiteList.ID).FirstOrDefault();
            if (objUpdateWhiteList != null)
            {
                try
                {
                    ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftWhiteList, Models.DBModels.TWSQLDB.PromotionGiftWhiteList>(argObjGiftWhiteList, objUpdateWhiteList);
                    this._promotionGiftWhiteListRepo.Update(objUpdateWhiteList);
                    boolExec = true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return boolExec;
        }

        public IQueryable<PromotionGiftWhiteList> GetAllPromotionGiftWhiteList()
        {
            return this._promotionGiftWhiteListRepo.GetAll();
        }

        public IQueryable<PromotionGiftWhiteList> GetPromotionGiftWhiteList(int basicID)
        {
            IQueryable<PromotionGiftWhiteList> result = this._promotionGiftWhiteListRepo
                .GetAll()
                .Where(x => x.PromotionGiftBasicID == basicID);

            return result;
        }

        public IQueryable<PromotionGiftWhiteList> GetPromotionGiftWhiteList(List<int> basicIDList)
        {
            IQueryable<PromotionGiftWhiteList> result = this._promotionGiftWhiteListRepo
                .GetAll()
                .Where(x => basicIDList.Contains(x.PromotionGiftBasicID));

            return result;
        }
        #endregion

        #region PromotionGiftInterval優惠活動金額間距
        
        public void CreatePromotionGiftInterval(Models.DBModels.TWSQLDB.PromotionGiftInterval argObjGiftInterval)
        {
            if (argObjGiftInterval == null)
            {
                return;
            }

            try
            {
                this._promotionGiftIntervalRepo.Create(argObjGiftInterval);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 新增優惠活動金額間距
        /// </summary>
        /// <param name="argListGiftInterval"></param>
        public void CreateRangePromotionGiftInterval(List<Models.DBModels.TWSQLDB.PromotionGiftInterval> argListGiftInterval)
        {
            if (argListGiftInterval == null || argListGiftInterval.Count <= 0)
            {
                return;
            }

            try
            {
                this._promotionGiftIntervalRepo.CreateRange(argListGiftInterval);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 修改優惠活動金額間距
        /// </summary>
        /// <param name="argListGiftInterval"></param>
        public bool UpdateRangePromotionGiftInterval(List<Models.DBModels.TWSQLDB.PromotionGiftInterval> argListGiftInterval)
        {
            if (argListGiftInterval == null || argListGiftInterval.Count <= 0)
            {
                return false;
            }

            bool boolExec = false;
            List<Models.DBModels.TWSQLDB.PromotionGiftInterval> listUpdateInterval = null;
            List<int> listNumId = null;

            listNumId = argListGiftInterval.Select(x => x.ID).ToList();

            listUpdateInterval = this._promotionGiftIntervalRepo.GetAll().Where(x => listNumId.Contains(x.ID)).ToList();

            if (listUpdateInterval != null && listUpdateInterval.Count > 0)
            {
                try
                {
                    ModelConverter.ConvertTo<List<Models.DBModels.TWSQLDB.PromotionGiftInterval>, List<Models.DBModels.TWSQLDB.PromotionGiftInterval>>(argListGiftInterval, listUpdateInterval);
                    this._promotionGiftIntervalRepo.UpdateRange(listUpdateInterval);
                    boolExec = true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return boolExec;
        }

        public bool UpdatePromotionGiftInterval(Models.DBModels.TWSQLDB.PromotionGiftInterval argObjGiftInterval)
        {
            if (argObjGiftInterval == null)
            {
                return false;
            }

            Models.DBModels.TWSQLDB.PromotionGiftInterval objUpdateInterval = null;
            bool boolExec = false;

            objUpdateInterval = this._promotionGiftIntervalRepo.GetAll().Where(x => x.ID == argObjGiftInterval.ID).FirstOrDefault();
            if (objUpdateInterval != null)
            {
                try
                {
                    ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftInterval, Models.DBModels.TWSQLDB.PromotionGiftInterval>(argObjGiftInterval, objUpdateInterval);
                    this._promotionGiftIntervalRepo.Update(objUpdateInterval);
                    boolExec = true;
                }
                catch (Exception ex)
                {
                    throw new NotImplementedException(ex.Message, ex);
                }
            }

            return boolExec;
        }
        public bool DeletePromotionGiftInterval(Models.DBModels.TWSQLDB.PromotionGiftInterval argObjGiftInterval)
        {
            bool boolExec = false;
            if (argObjGiftInterval == null)
            {
                return false;
            }
            try
            {
                this._promotionGiftIntervalRepo.Delete(argObjGiftInterval);
                boolExec = true;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
            return boolExec;
        }
        public IQueryable<PromotionGiftInterval> GetAllPromotionGiftInterval()
        {
            return this._promotionGiftIntervalRepo.GetAll();
        }

        public IQueryable<PromotionGiftInterval> GetPromotionGiftInterval(int basicID)
        {
            IQueryable<PromotionGiftInterval> result = this._promotionGiftIntervalRepo
                .GetAll()
                .Where(x => x.PromotionGiftBasicID == basicID);

            return result;
        }

        public IQueryable<PromotionGiftInterval> GetPromotionGiftInterval(List<int> basicIDList)
        {
            IQueryable<PromotionGiftInterval> result = this._promotionGiftIntervalRepo
                .GetAll()
                .Where(x => basicIDList.Contains(x.PromotionGiftBasicID));

            return result;
        }

        #endregion

    }
}
