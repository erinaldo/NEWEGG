using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartServices.Interface;
using TWNewEgg.Models.DomainModels.CartPayment;

namespace TWNewEgg.CartServices.CartPayment
{
    public class NECartPaymentAdapter : ICartPaymentAdapter
    {
        private ICartPayment _cartNENormalCartPayment;

        /// <summary>
        /// 建構函式
        /// </summary>
        /// <param name="argNENormalCartPaymentRepo"></param>
        public NECartPaymentAdapter(ICartPayment argNENormalCartPaymentRepo)
        {
            this._cartNENormalCartPayment = argNENormalCartPaymentRepo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="cartType"></param>
        /// <returns></returns>
        public DM_CartPayment GetCartPayment(int accountId, int cartType)
        {
            DM_CartPayment cartPaymentModel = new DM_CartPayment();

            try
            {
                this._cartNENormalCartPayment.Initiate(accountId, cartType);

                cartPaymentModel = this._cartNENormalCartPayment.GetCartPayment();

            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return cartPaymentModel;
        }
    }
}
