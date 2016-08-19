using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.CartPayment;
using TWNewEgg.NECartPaymentRepoAdapters.Interface;

namespace TWNewEgg.NECartPaymentRepoAdapters
{
    public class NECartPaymentRepoAdapters : INECartPaymentRepoAdapters
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="cartType"></param>
        /// <returns></returns>
        public DM_CartPayment GetPayTerms(int accountId, int cartType)
        {
            DM_CartPayment cartPaymentModel = new DM_CartPayment();

            try
            {

            }
            catch (Exception ex)
            {

            }

            return cartPaymentModel;
        }
    }
}
