using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.CartServices.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DomainModels.CartPayment;

namespace TWNewEgg.CartServices.CartPayment
{
    public class TelegraphicTransferGetService : IPayTypeGetService
    {
        private IPayTypeRepoAdapter _iPayTypeRepoAdapter;

        public TelegraphicTransferGetService(IPayTypeRepoAdapter iPayTypeRepoAdapter) 
        {
            this._iPayTypeRepoAdapter = iPayTypeRepoAdapter;
        }
        public IEnumerable<Models.DomainModels.CartPayment.DM_PayType> GetPayTypes(Models.DomainModels.ShoppingCartPayType.DM_ShoppingCart shoppingCart)
        {
            IEnumerable<Models.DomainModels.CartPayment.DM_PayType> result;
            List<Models.DBModels.TWSQLDB.PayType> querySearch;

            querySearch = this._iPayTypeRepoAdapter.getAll().Where(x => x.PaymentTermID == PaymentTermID.電匯 && x.Status == 0).OrderBy(x => x.ChooseOrder).ToList();
            result = ModelConverter.ConvertTo<IEnumerable<Models.DomainModels.CartPayment.DM_PayType>>(querySearch);

            return result;
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
