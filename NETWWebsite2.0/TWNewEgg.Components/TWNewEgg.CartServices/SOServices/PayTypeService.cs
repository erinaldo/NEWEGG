using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.BankInstallmentRepoAdapters.Interface;
using TWNewEgg.BankRepoAdapters.Interface;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.CartServices.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.ItemInstallmentRepoAdapters.Interface;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.Models;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.DomainModels.CartPayment;
using TWNewEgg.Models.DomainModels.Message;



namespace TWNewEgg.CartServices.SOServices
{
    public class PayTypeService : IPayTypeService
    {
        private IBankInstallmentRepoAdapter _iBankInstallmentRepoAdapter;
        private IItemInstallmentRuleRepoAdapter _iItemInstallmentRuleRepoAdapter;
        private IItemTopInstallmentRepoAdapter _iItemTopInstallmentRepoAdapter;
        private IItemDisplayPriceRepoAdapter _iItemDisplayPriceRepoAdapter;
        private IItemRepoAdapter _iItemRepoAdapter;
        private IBankRepoAdapter _iBankRepoAdapter;
        private IPayTypeRepoAdapter _iPayTypeRepoAdapter;
        private IProductRepoAdapter _iProductRepoAdapter;



        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        #region Adapters

        public PayTypeService(IRepository<Models.DBModels.TWSQLDB.PayType> paytypeRepo,
            IBankInstallmentRepoAdapter iBankInstallmentRepoAdapter,
            IProductRepoAdapter iProductRepoAdapter,
            IItemInstallmentRuleRepoAdapter iItemInstallmentRuleRepoAdapter,
            IItemTopInstallmentRepoAdapter iItemTopInstallmentRepoAdapter,
            IItemDisplayPriceRepoAdapter iItemDisplayPriceRepoAdapter,
            IItemRepoAdapter iItemRepoAdapter,
            IPayTypeRepoAdapter iPayTypeRepoAdapter,
            IBankRepoAdapter iBankRepoAdapter)
        {
            this._iBankInstallmentRepoAdapter = iBankInstallmentRepoAdapter;
            this._iItemInstallmentRuleRepoAdapter = iItemInstallmentRuleRepoAdapter;
            this._iItemTopInstallmentRepoAdapter = iItemTopInstallmentRepoAdapter;
            this._iItemDisplayPriceRepoAdapter = iItemDisplayPriceRepoAdapter;
            this._iItemRepoAdapter = iItemRepoAdapter;
            this._iPayTypeRepoAdapter = iPayTypeRepoAdapter;
            this._iBankRepoAdapter = iBankRepoAdapter;
            this._iProductRepoAdapter = iProductRepoAdapter;
        }

        #endregion Adapters

        public Models.DBModels.TWSQLDB.PayType GetPayType(SOBase order)
        {
            var predicate = PredicateBuilder.True<Models.DBModels.TWSQLDB.PayType>();
            predicate = predicate.And(f => f.PayType0rateNum == order.PayType);

            throw new NotImplementedException();
        }
    }
}
