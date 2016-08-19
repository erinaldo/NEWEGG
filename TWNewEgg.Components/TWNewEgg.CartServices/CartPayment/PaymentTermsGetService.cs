using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewegg.DelvTypePaymentTermRepoAdapters.Interface;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.CartServices.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.Models.DBModels;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.CartPayment;
using TWNewEgg.Models.DomainModels.Message;
using TWNewEgg.Models.DomainModels.ShoppingCartPayType;

namespace TWNewEgg.CartServices.CartPayment
{
    public class PaymentTermsGetService : IPaymentTermsGetService
    {
        private IDelvTypePaymentTermRepoAdapter _DelvTypePaymentTermRepo;
        private IItemRepoAdapter _ItemRepoAdapterRepo;
        private IPaymentTermRepoAdapter _PaymentTermRepoAdapter;

        public PaymentTermsGetService(
            IDelvTypePaymentTermRepoAdapter argDelvTypePaymentTermRepo,
            IItemRepoAdapter argItemRepoAdapterRepo,
            IPaymentTermRepoAdapter argPaymentTermRepo
            )
        {
            this._DelvTypePaymentTermRepo = argDelvTypePaymentTermRepo;
            this._ItemRepoAdapterRepo = argItemRepoAdapterRepo;
            this._PaymentTermRepoAdapter = argPaymentTermRepo;
        }

        /// <summary>
        /// 依各購物車的賣場編號查詢可用的付款方式
        /// </summary>
        /// <param name="shoppingCart"></param>
        /// <returns></returns>
        public IEnumerable<DM_PaymentTerm> GetPaymentTerms(DM_ShoppingCart shoppingCart)
        {
            List<int> listDelvType = new List<int>();
            List<string> listPaymentTermID = new List<string>();
            List<PaymentTerm> listPaymentTermModel = new List<PaymentTerm>();
            List<DM_PaymentTerm> listDM_PaymentTermModel = new List<DM_PaymentTerm>();

            if (shoppingCart != null || shoppingCart.Items != null)
            {
                //依多筆賣場編號查詢多筆交易模式(Group by 不重複)
                listDelvType = this._ItemRepoAdapterRepo.GetListGroupByItemDelvType(shoppingCart.Items);   
            }

            if (listDelvType != null)
            {
                //用多筆交易模式獲取PaymentTermID的資料(各交易模式的PaymentTermID交集)
                listPaymentTermID = _DelvTypePaymentTermRepo.GetlistIntersectPaymentTermID_PaymentTerm(listDelvType);
            }

            if (listPaymentTermID != null)
            {
                //使用PaymentTermID獲得PaymentTerm資料
                listPaymentTermModel = _PaymentTermRepoAdapter.GetPaymentTermByID(listPaymentTermID).ToList();
            }

            if (listPaymentTermModel != null)
            {
                //DB PaymentTerm 轉 DM_PaymentTerm
                listDM_PaymentTermModel = ModelConverter.ConvertTo<List<DM_PaymentTerm>>(listPaymentTermModel);
            }

            return listDM_PaymentTermModel;
        }
    }
}
