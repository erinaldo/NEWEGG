using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewegg.DelvTypePaymentTermRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewegg.DelvTypePaymentTermRepoAdapters
{
    public class DelvTypePaymentTermRepoAdapter : IDelvTypePaymentTermRepoAdapter
    {
        private IRepository<DeliveryType_PaymentTerm> _DelvTypePaymentTermRepo;

        /// <summary>
        /// 建構函式
        /// </summary>
        /// <param name="argDelvTypePaymentTermRepo"></param>
        public DelvTypePaymentTermRepoAdapter(IRepository<DeliveryType_PaymentTerm> argDelvTypePaymentTermRepo)
        {
            this._DelvTypePaymentTermRepo = argDelvTypePaymentTermRepo;
        }

        /// <summary>
        /// 用多筆交易模式獲取PaymentTermID的資料(各交易模式的PaymentTermID交集)
        /// </summary>
        /// <param name="listDelvType">多筆交易模式</param>
        /// <returns>list PaymentTerm ID</returns>
        public List<string> GetlistIntersectPaymentTermID_PaymentTerm(List<int> listDelvType)
        {
            List<string> listPaymentTermID = new List<string>();

            if (listDelvType == null)
            {
                return listPaymentTermID;
            }

            //依每個交易模式查詢PaymentTermID交集
            foreach (var DelvType in listDelvType)
            {
                //第一次獲取
                if (listPaymentTermID.Count == 0)
                {
                    listPaymentTermID = this._DelvTypePaymentTermRepo.GetAll().Where(x => x.DelvType == DelvType).Select(x => x.PaymentTermID).ToList();
                }
                else
                {
                    var first = this._DelvTypePaymentTermRepo.GetAll().Where(x => x.DelvType == DelvType).Select(x => x.PaymentTermID).ToList();

                    //前一個與現在的進行交集
                    var secound = listPaymentTermID.Intersect(first);

                    listPaymentTermID = new List<string>();

                    //取代前一個PaymentTermID交集
                    foreach (string data in secound)
                    {
                        listPaymentTermID.Add(data);
                    }
                }
            }

            return listPaymentTermID;
        }
    }
}
