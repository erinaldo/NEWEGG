using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.BankInstallmentRepoAdapters.Interface;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.CartServices.Interface;
using TWNewEgg.ItemInstallmentRepoAdapters.Interface;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.Models.DomainModels.CartPayment;
using TWNewEgg.Models.DomainModels.Message;

namespace TWNewEgg.CartServices.CartPayment
{
    public class ItemPagePayTypeGetService : InstallmentPayTypeGetService, IItemPayTypeGetService
    {
        public ItemPagePayTypeGetService(IPayTypeRepoAdapter iPayTypeRepoAdapter,
            IBankInstallmentRepoAdapter iBankInstallmentRepoAdapter,
            IProductRepoAdapter iProductRepoAdapter,
            IItemRepoAdapter iItemRepoAdapter,
            IItemTopInstallmentRepoAdapter iItemTopInstallmentRepoAdapter,
            IItemDisplayPriceRepoAdapter iItemDisplayPriceRepoAdapter)
            : base(iPayTypeRepoAdapter, iBankInstallmentRepoAdapter, iProductRepoAdapter, iItemRepoAdapter, iItemTopInstallmentRepoAdapter, iItemDisplayPriceRepoAdapter) { }
        
        public List<TWNewEgg.Models.DomainModels.CartPayment.ItemPayType> GetPayTypesByItems(List<int> itemIds)
        {
            TWNewEgg.Models.DomainModels.CartPayment.ItemPayType objPayType = new ItemPayType();
            List<TWNewEgg.Models.DomainModels.CartPayment.ItemPayType> listItemPayType = null;
            ResponseMessage<List<DM_PayType>> GetAvailablePaytypesResult = new ResponseMessage<List<DM_PayType>>();

            GetAvailablePaytypesResult = GetInstallmentPayTypes(itemIds.ToList());
            foreach (var PayTypeItem in GetAvailablePaytypesResult.Data)
            {
                var itemPayTypeTemp = listItemPayType.Where(p => p.PayType0rateNum == PayTypeItem.Installment).FirstOrDefault();

                if (itemPayTypeTemp != null)
                {
                    itemPayTypeTemp.PayTypeBankStrForList = itemPayTypeTemp.PayTypeBankStrForList + "、" + PayTypeItem.BankList;
                }
                else
                {
                    TWNewEgg.Models.DomainModels.CartPayment.ItemPayType itemPayType = new Models.DomainModels.CartPayment.ItemPayType();
                    itemPayType.PayType0rateNum = PayTypeItem.PayType0rateNum.Value;
                    itemPayType.PayTypeBankStrForList = PayTypeItem.BankList;
                    itemPayType.Staging = this.SeparateStaging(PayTypeItem.Name);
                    if (PayTypeItem.InsRate == 0)
                    {
                        itemPayType.InsRate = "0利率";
                    }
                    else
                    {
                        itemPayType.InsRate = string.Format("{0:0%}", PayTypeItem.InsRate.Value);
                    }
                    listItemPayType.Add(itemPayType);
                }
            }
            listItemPayType = this.selectTheSameAndComputeCount(listItemPayType);
            
            return listItemPayType;
            
        }

        /// <summary>
        /// 期數
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private int SeparateStaging(string str)
        {
            int result = 0;
            if (str.IndexOf("期") >= 0)
            {
                int tempStaging = 0;
                int.TryParse(str.Substring(0, str.IndexOf("期")), out tempStaging);
                result = tempStaging;
            }
            return result;// result;
        }
        /// <summary>
        /// 算可用銀行數
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private List<TWNewEgg.Models.DomainModels.CartPayment.ItemPayType> selectTheSameAndComputeCount(List<TWNewEgg.Models.DomainModels.CartPayment.ItemPayType> model)
        {
            foreach (var item in model)
            {
                List<string> BankListTemp = item.PayTypeBankStrForList.Split('、').GroupBy(x => x).Select(x => x.Key).ToList();
                //item.PayTypeBankStrForList = string.Join("、", BankListTemp.ToArray());
                item.canUseBankCount = BankListTemp == null ? 0 : BankListTemp.Count;
            }
            model = model.OrderBy(p => p.Staging).ToList();
            return model;
        }
    }
}
