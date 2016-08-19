using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.PromotionRepoAdapters.Interface;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.Models.DomainModels.Redeem;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.DAL;
using TWNewEgg.Framework.AutoMapper;
//using TWNewEgg.DB;
//using TWNewEgg.DB.TWSQLDB.Models;

namespace TWNewEgg.Redeem.Service.PromotionGiftService
{
    public class PromotionGiftRecordRepository: IPromotionGiftRecordService
    {
        private IPromotionRecordsRepoAdapter _PromotionGiftRecRepo = null;
        private ISORepoAdapter _SoRepo = null;
        public PromotionGiftRecordRepository(IPromotionRecordsRepoAdapter argPromotionGiftRecRepo, ISORepoAdapter argSoRepo)
        {
            this._PromotionGiftRecRepo = argPromotionGiftRecRepo;
            this._SoRepo = argSoRepo;
        }
        /// <summary>
        /// 新增PromotionGiftRecord
        /// </summary>
        /// <param name="argObjGiftRecord">欲新增的PromotionGiftRecord物件</param>
        /// <returns>新增成功:true, 新增失敗:false</returns>
        public bool CreatePromotionGiftRecord(Models.DomainModels.Redeem.PromotionGiftRecords argObjGiftRecord)
        {
            bool boolExec = false;
            Models.DBModels.TWSQLDB.PromotionGiftRecords objDbGiftRec = null;

            if (argObjGiftRecord == null)
                return false;

            try
            {
                objDbGiftRec = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftRecords>(argObjGiftRecord);
                this._PromotionGiftRecRepo.CreatePromotionGiftRecords(objDbGiftRec);
                boolExec = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return boolExec;
        }

        /// <summary>
        /// 新增List內的所有PromotionGiftRecord
        /// </summary>
        /// <param name="argListGiftRecord">List of PromotionGiftRecord</param>
        /// <returns>新增成功:true, 新增失敗:false</returns>
        public bool CreateRangePromotionGiftRecord(List<Models.DomainModels.Redeem.PromotionGiftRecords> argListGiftRecord)
        {
            if (argListGiftRecord == null || argListGiftRecord.Count <= 0)
                return false;

            bool boolExec = false;
            List<Models.DBModels.TWSQLDB.PromotionGiftRecords> listDbGiftRec = null;
            Models.DBModels.TWSQLDB.PromotionGiftRecords objDbGiftRec = null;

            listDbGiftRec = new List<Models.DBModels.TWSQLDB.PromotionGiftRecords>();
            try
            {
                foreach (Models.DomainModels.Redeem.PromotionGiftRecords objSubRecord in argListGiftRecord)
                {
                    objDbGiftRec = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftRecords>(objSubRecord);
                    listDbGiftRec.Add(objDbGiftRec);
                }

                this._PromotionGiftRecRepo.CreateRangePromotionGiftRecords(listDbGiftRec);
                boolExec = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return boolExec;
        }

        /// <summary>
        /// 修改PromotionGiftRecord
        /// </summary>
        /// <param name="argObjGiftRecord">PromotionGiftRecord物件</param>
        /// <returns>修改成功:true, 修改失敗:false</returns>
        public bool UpdatePromotionGiftRecord(Models.DomainModels.Redeem.PromotionGiftRecords argObjGiftRecord)
        {
            if (argObjGiftRecord == null || argObjGiftRecord.SalesOrderItemCode.Length <= 0)
            {
                return false;
            }

            bool boolExec = false;
            Models.DBModels.TWSQLDB.PromotionGiftRecords objDbGiftRecord = null;
            
            //objGiftRecord = oDb.PromotionGiftRecords.Where(x => x.SalesOrderItemCode == argObjGiftRecord.SalesOrderItemCode).FirstOrDefault();
            try
            {
                objDbGiftRecord = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftRecords>(argObjGiftRecord);
                objDbGiftRecord.UpdateDate = DateTime.Now;
                boolExec = this._PromotionGiftRecRepo.UpdatePromotionGiftRecords(objDbGiftRecord);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return boolExec;
        }

        /// <summary>
        /// 修改List裡所有的PromotionGiftRecord
        /// </summary>
        /// <param name="argListGiftRecords">List of PromotionGiftRecord</param>
        /// <returns>修改成功:true, 修改失敗:false</returns>
        public bool UpdatePromotionGiftRecordByList(List<Models.DomainModels.Redeem.PromotionGiftRecords> argListGiftRecords)
        {
            if (argListGiftRecords == null || argListGiftRecords.Count <= 0)
            {
                return false;
            }

            List<Models.DBModels.TWSQLDB.PromotionGiftRecords> listDbUpdateGiftRecords = null;
            Models.DBModels.TWSQLDB.PromotionGiftRecords objDbUpdateGiftRecord = null;
            bool boolExec = false;

            if (argListGiftRecords == null || argListGiftRecords.Count <= 0)
            {
                return false;
            }

            listDbUpdateGiftRecords = new List<Models.DBModels.TWSQLDB.PromotionGiftRecords>();
            try
            {
                foreach (Models.DomainModels.Redeem.PromotionGiftRecords objSubRec in argListGiftRecords)
                {
                    objDbUpdateGiftRecord = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftRecords>(objSubRec);
                    listDbUpdateGiftRecords.Add(objDbUpdateGiftRecord);
                }

                boolExec = this._PromotionGiftRecRepo.UpdateRangePromotionGiftRecords(listDbUpdateGiftRecords);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return boolExec;

        }

        /// <summary>
        /// 根據購物車ID, 取得相關PromotionGiftRecord
        /// </summary>
        /// <param name="argSalesOrderGroupId">購物車ID</param>
        /// <returns>null或是PromotionGiftRecord List</returns>
        public List<Models.DomainModels.Redeem.PromotionGiftRecords> GetGiftRecordsBySalesOrderGroupId(int argSalesOrderGroupId)
        {
            if (argSalesOrderGroupId <= 0)
            {
                return null;
            }
            List<string> listSalesOrderCode = null;
            List<string> listSalesOrderItemCode = null;
            List<Models.DomainModels.Redeem.PromotionGiftRecords> listGiftRecords = null;
            List<Models.DBModels.TWSQLDB.PromotionGiftRecords> listDbGiftRecords = null;

            // 取得SalesOrderGroup的SalesOrderCode
            //listSalesOrderCode = oDb.SalesOrder.Where(x => x.SalesOrderGroupID == argSalesOrderGroupId).Select(x => x.Code).ToList();
            listSalesOrderCode = this._SoRepo.GetSOs(argSalesOrderGroupId).Select(x => x.Code).ToList();
            if (listSalesOrderCode != null && listSalesOrderCode.Count > 0)
            {
                // 取得SalesOrderItem, 並且排除服務費:12453 與國際運費:12451
                //listSalesOrderItemCode = oDb.SalesOrderItem.Where(x => listSalesOrderCode.Contains(x.SalesorderCode) && x.ItemID != 12453 & x.ItemID != 12451).Select(x => x.Code).ToList();
                listSalesOrderItemCode = this._SoRepo.GetSOItemsByCodes(listSalesOrderCode).Where(x => x.ItemID != 12453 & x.ItemID != 12451).Select(x => x.Code).ToList();
                if (listSalesOrderItemCode != null)
                {
                    // 根據SOI取得相關的PromotionGiftRecords
                    //listDbGiftRecords = oDb.PromotionGiftRecords.Where(x => listSalesOrderItemCode.Contains(x.SalesOrderItemCode)).ToList();
                    listDbGiftRecords = this._PromotionGiftRecRepo.GetAllPromotionGiftRecords().Where(x => listSalesOrderItemCode.Contains(x.SalesOrderItemCode)).ToList();
                    if (listDbGiftRecords != null && listDbGiftRecords.Count > 0)
                    {
                        try
                        {
                            listGiftRecords = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.PromotionGiftRecords>>(listDbGiftRecords);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }

            // 釋放所有記憶體
            listSalesOrderCode = null;
            listSalesOrderItemCode = null;
            listDbGiftRecords = null;

            return listGiftRecords;
        }

        /// <summary>
        /// 根據SalesOrderItemCode List, 取得相關PromotionGiftRecord
        /// </summary>
        /// <param name="argListSalesOrderItemCode">SalesOrderItemCode List</param>
        /// <returns>null或是PromotionGiftRecord List</returns>
        public List<Models.DomainModels.Redeem.PromotionGiftRecords> GetGiftRecordsByListSalesOrderItemCode(List<string> argListSalesOrderItemCode)
        {
            if (argListSalesOrderItemCode == null || argListSalesOrderItemCode.Count <= 0)
            {
                return null;
            }

            List<Models.DomainModels.Redeem.PromotionGiftRecords> listGiftRecords = null;
            List<Models.DBModels.TWSQLDB.PromotionGiftRecords> listDbGiftRec = null;

            //listGiftRecords = oDb.PromotionGiftRecords.Where(x => argListSalesOrderItemCode.Contains(x.SalesOrderItemCode)).ToList();
            listDbGiftRec = this._PromotionGiftRecRepo.GetAllPromotionGiftRecords().Where(x => argListSalesOrderItemCode.Contains(x.SalesOrderItemCode)).ToList();
            if (listDbGiftRec != null && listDbGiftRec.Count > 0)
            {
                try
                {
                    listGiftRecords = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.PromotionGiftRecords>>(listDbGiftRec);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return listGiftRecords;
        }

        /// <summary>
        /// 根據SalesOrderItemCode取得PromotionGiftRecord
        /// </summary>
        /// <param name="argStrSalesOrderItemCode">SalesOrderItemCode</param>
        /// <returns>null或是PromotionGiftRecord物件</returns>
        public Models.DomainModels.Redeem.PromotionGiftRecords GetGiftRecordsBySalesOrderItemCode(string argStrSalesOrderItemCode)
        {
            if (String.IsNullOrEmpty(argStrSalesOrderItemCode))
            {
                return null;
            }

            Models.DomainModels.Redeem.PromotionGiftRecords objGiftRecord = null;
            Models.DBModels.TWSQLDB.PromotionGiftRecords objDbGiftRec = null;

            //objGiftRecord = oDb.PromotionGiftRecords.Where(x => x.SalesOrderItemCode == argStrSalesOrderItemCode).FirstOrDefault();
            objDbGiftRec = this._PromotionGiftRecRepo.GetAllPromotionGiftRecords().Where(x => x.SalesOrderItemCode == argStrSalesOrderItemCode).FirstOrDefault();
            if (objDbGiftRec != null)
            {
                objGiftRecord = ModelConverter.ConvertTo<Models.DomainModels.Redeem.PromotionGiftRecords>(objDbGiftRec);
            }

            return objGiftRecord;
        }
    }
}
