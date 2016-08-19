using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.BankBonusRepoAdapters.Interface;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.CartServices.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.BankBonus;
using TWNewEgg.Models.DomainModels.CartPayment;


namespace TWNewEgg.CartServices.CartPayment
{
    public class BankBonusPayGetService : IPayTypeGetService
    {
        private IPayTypeRepoAdapter _iPayTypeRepoAdapter;
        private IBankBonusRepoAdapter _iBankBonusRepoAdapter;

        public BankBonusPayGetService(IPayTypeRepoAdapter iPayTypeRepoAdapter, IBankBonusRepoAdapter iBankBonusRepoAdapter) 
        {
            this._iPayTypeRepoAdapter = iPayTypeRepoAdapter;
            this._iBankBonusRepoAdapter = iBankBonusRepoAdapter;
        }

        public IEnumerable<Models.DomainModels.CartPayment.DM_PayType> GetPayTypes(Models.DomainModels.ShoppingCartPayType.DM_ShoppingCart shoppingCart)
        {
            List<BankBonus> bankBonusCell = null;
            List<BankBonus_DM> bankBonusCell_DM = null;
            DM_CartPayTypeBankIDwithName cartTypeBank = new DM_CartPayTypeBankIDwithName();
            DM_PayType payTypeDM = new DM_PayType();
            PayType payType = new PayType();
            List<DM_PayType> payTypeList = new List<DM_PayType>();

            bankBonusCell = this._iBankBonusRepoAdapter.GetAllEffectiveBankBonus().ToList();
            if (bankBonusCell.Count > 0)
            {
                bankBonusCell_DM = ModelConverter.ConvertTo<List<BankBonus_DM>>(bankBonusCell);
            }
            if (bankBonusCell_DM.Count > 1)
            {
                bankBonusCell_DM = bankBonusCell_DM.OrderBy(x => x.Order).ToList();
            }
            if (bankBonusCell_DM.Count > 0)
            {
                foreach (var item in bankBonusCell_DM)
                {
                    payType = this._iPayTypeRepoAdapter.GetPayTypeByBankCode(item.BankCode);
                    if (payType != null)
                    {
                        payTypeDM = ModelConverter.ConvertTo<DM_PayType>(payType);
                        payTypeList.Add(payTypeDM);
                    }
                }
            }
            return payTypeList;
        }

        public IQueryable<Models.DomainModels.CartPayment.DM_PayType> GetAllPayTypes()
        {
            IQueryable<Models.DBModels.TWSQLDB.PayType> querySearch;
            IQueryable<Models.DomainModels.CartPayment.DM_PayType> queryResult;

            try
            {
                querySearch = this._iPayTypeRepoAdapter.getAll();
                queryResult = ModelConverter.ConvertTo<IQueryable<Models.DomainModels.CartPayment.DM_PayType>>(querySearch);

            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return queryResult;
        }
    }
}
