using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.BankBonusRepoAdapters.Interface;
using TWNewEgg.BankRepoAdapters.Interface;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.CartServices.Interface;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Bank;
using TWNewEgg.Models.DomainModels.BankBonus;
using TWNewEgg.Models.DomainModels.CartPayment;
using TWNewEgg.Models.DomainModels.ShoppingCartPayType;

namespace TWNewEgg.CartServices.CartPayment
{
    public class  NENormalCartPayment : NECartPayment
    {
        private DM_ShoppingCart ShoppingCart;
        private INEShoppingCartService _cartNEShoppingCartService;
        private IPaymentTermsGetService _cartPaymentTermsGetService;
        private IBeneficiaryPartyAdapter _beneficiaryPartyAdapter;
        private IBankRepoAdapter _bankRepoAdapter;
        private IBankBonusRepoAdapter _bankBonusRepoAdapter;

        public NENormalCartPayment(INEShoppingCartService cartNEShoppingCartService, 
            IPaymentTermsGetService cartPaymentTermsGetService, 
            IBeneficiaryPartyAdapter beneficiaryPartyAdapter,
            IBankRepoAdapter bankRepoAdapter,
            IBankBonusRepoAdapter bankBonusRepoAdapter)
        {
            this._cartNEShoppingCartService = cartNEShoppingCartService;
            this._cartPaymentTermsGetService = cartPaymentTermsGetService;
            this._beneficiaryPartyAdapter = beneficiaryPartyAdapter;
            this._bankRepoAdapter = bankRepoAdapter;
            this._bankBonusRepoAdapter = bankBonusRepoAdapter;
        }

        public override void Initiate(int cartID, int cartType)
        {
            List<DM_BeneficiaryParty> listBeneficiaryParty = new List<DM_BeneficiaryParty>();
            DM_BeneficiaryParty beneficiaryParty = new DM_BeneficiaryParty();
            List<DM_PayType> listPayTypeModel = new List<DM_PayType>();
            List<Bank_DM> listBank = new List<Bank_DM>();
            List<BankBonus_DM> listBankBonus = new List<BankBonus_DM>();
            // 依accountId,cartType找加入購物車的商品
            ShoppingCart = _cartNEShoppingCartService.GetShoppingCart(cartID, cartType);
            // 依Item獲取PaymentTerm
            List<DM_PaymentTerm> paymentTermList = _cartPaymentTermsGetService.GetPaymentTerms(ShoppingCart).ToList();
            // 依付款方式(PaymentTermID)去找paytype service
            foreach (var PayTerm in paymentTermList)
            {
                IPayTypeGetService payTypeGetService = (IPayTypeGetService)AutofacConfig.Container.ResolveKeyed(PayTerm.ID, typeof(IPayTypeGetService));
                
                IEnumerable<DM_PayType> payTypeModel = payTypeGetService.GetPayTypes(ShoppingCart);
                
                listPayTypeModel.AddRange(payTypeModel.ToList());
                foreach (var objPayType in payTypeModel.ToList())
                {
                    beneficiaryParty = GetBeneficiaryParty(objPayType.BeneficiaryPartyID);
                    if (beneficiaryParty != null)
                    {
                        listBeneficiaryParty.Add(beneficiaryParty);
                        Bank_DM bankResult = GetBank(beneficiaryParty);
                        listBank.Add(bankResult);
                    }
                    if (objPayType.PaymentTermID == PaymentTermID.信用卡紅利折抵 && beneficiaryParty!=null)
                    {
                        BankBonus_DM bankbonusResult = GetBankBnous(beneficiaryParty);
                        listBankBonus.Add(bankbonusResult);
                    }
                }
            }

            CartPayment.BeneficiaryParty = BeneficiaryPartys = listBeneficiaryParty;
            CartPayment.PaymentTerms = PayTerms = paymentTermList;
            CartPayment.PayTypes= PayTypes = listPayTypeModel;
            CartPayment.Bank = Banks = listBank;
            CartPayment.BankBonus = BankBonus = listBankBonus;
        }

        private DM_BeneficiaryParty GetBeneficiaryParty(string BeneficiaryPartyID)
        {
            DM_BeneficiaryParty beneficiaryParty = null;
            BeneficiaryParty beneficiaryPartySearch = null;
            try
            {
                beneficiaryPartySearch = this._beneficiaryPartyAdapter.GetAvailable().Where(x => x.ID == BeneficiaryPartyID).SingleOrDefault();
                if (beneficiaryPartySearch != null)
                {
                    beneficiaryParty = ModelConverter.ConvertTo<DM_BeneficiaryParty>(beneficiaryPartySearch);
                }
            }
            catch (SqlException ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
            catch (EntityException ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
            return beneficiaryParty;
        }

        private Bank_DM GetBank(DM_BeneficiaryParty beneficiaryParty)
        {
            Bank bankSearch;
            Bank_DM bankResult;
            try
            {
                bankSearch = this._bankRepoAdapter.GetAll().Where(x => x.Code == beneficiaryParty.BankCode).FirstOrDefault();
                bankResult = ModelConverter.ConvertTo<Bank_DM>(bankSearch);
            }
            catch (SqlException ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
            catch (EntityException ex)
            {
                throw new NotImplementedException(ex.Message,ex);
            }
            return bankResult;
        }

        private BankBonus_DM GetBankBnous(DM_BeneficiaryParty beneficiaryParty)
        {
            BankBonus bankbonusSearch;
            BankBonus_DM bankbonusResult;
            try
            {
                bankbonusSearch = this._bankBonusRepoAdapter.GetAllBankBonus().Where(x => x.BankCode == beneficiaryParty.BankCode).FirstOrDefault();
                bankbonusResult = ModelConverter.ConvertTo<BankBonus_DM>(bankbonusSearch);
            }
            catch (SqlException ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
            catch (EntityException ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
            return bankbonusResult;
        }
        
    }
}
