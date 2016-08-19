using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.PromotionRepoAdapters.Interface;
using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.PromotionRepoAdapters
{
    public class PromotionRecordsRepoAdapter: IPromotionRecordsRepoAdapter
    {
        IRepository<PromotionGiftRecords> _GiftRecordsRepo = null;
        public PromotionRecordsRepoAdapter(IRepository<PromotionGiftRecords> argGiftRecordsRepo)
        {
            this._GiftRecordsRepo = argGiftRecordsRepo;
        }
        public void CreatePromotionGiftRecords(PromotionGiftRecords argObjGiftRec)
        {
            if (argObjGiftRec == null)
            {
                return;
            }

            try
            {
                this._GiftRecordsRepo.Create(argObjGiftRec);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateRangePromotionGiftRecords(List<PromotionGiftRecords> argListGiftRec)
        {
            if (argListGiftRec == null || argListGiftRec.Count <= 0)
            {
                return;
            }

            try
            {
                this._GiftRecordsRepo.CreateRange(argListGiftRec);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdatePromotionGiftRecords(PromotionGiftRecords argObjGiftRec)
        {
            if (argObjGiftRec == null)
            {
                return false;
            }

            PromotionGiftRecords objUpdateRec = null;
            bool boolExec = false;

            objUpdateRec = this._GiftRecordsRepo.Get(x => x.PromotionGiftBasicID == argObjGiftRec.PromotionGiftBasicID && x.SalesOrderItemCode == argObjGiftRec.SalesOrderItemCode);
            if (objUpdateRec != null)
            {
                try
                {
                    // 前蓋後
                    ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftRecords, Models.DBModels.TWSQLDB.PromotionGiftRecords>(argObjGiftRec, objUpdateRec);
                    this._GiftRecordsRepo.Update(objUpdateRec);
                    boolExec = true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

            return boolExec;

        }

        public bool UpdateRangePromotionGiftRecords(List<PromotionGiftRecords> argListGiftRec)
        {
            if (argListGiftRec == null || argListGiftRec.Count == 0)
            {
                return false;
            }

            bool boolExec = false;
            argListGiftRec.ForEach(subGiftRec =>
            {
                PromotionGiftRecords objUpdateRec = null;
                objUpdateRec = this._GiftRecordsRepo.Get(x => x.PromotionGiftBasicID == subGiftRec.PromotionGiftBasicID && x.SalesOrderItemCode == subGiftRec.SalesOrderItemCode);
                if (objUpdateRec != null)
                {
                    try
                    {
                        // 前蓋後
                        ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftRecords, Models.DBModels.TWSQLDB.PromotionGiftRecords>(subGiftRec, objUpdateRec);
                        this._GiftRecordsRepo.Update(objUpdateRec);
                        boolExec = true;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }
            });

            return boolExec;
        }

        public IQueryable<PromotionGiftRecords> GetAllPromotionGiftRecords()
        {
            return this._GiftRecordsRepo.GetAll();
        }
    }
}
