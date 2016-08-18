using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.CartServices.CartMachines;
using TWNewEgg.CartServices.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Cart;

namespace TWNewEgg.CartServices.CartStates
{
    public class CartInitialState : ICartState
    {
        private OPCCartMachine _cartMachine;

        private ISORepoAdapter _soRepoAdapter;
        private ISOGroupInfoService _soGroupInfoService;
        public CartInitialState(ISORepoAdapter soRepoAdapter, ISOGroupInfoService soGroupInfoService)
        {
            this._soRepoAdapter = soRepoAdapter;
            this._soGroupInfoService = soGroupInfoService;
        }

        public void Init(OPCCartMachine cartMachine)
        {
            this._cartMachine = cartMachine;
        }

        public void Pay(int orderStatus)
        {
            List<SOBase> soBases = this._cartMachine.soGroupInfo.SalesOrders.Select(x=>x.Main).ToList();
            foreach (SOBase soBase in soBases)
            {
                soBase.Status = orderStatus;
                SalesOrder so = ModelConverter.ConvertTo<SalesOrder>(soBase);
                this._soRepoAdapter.UpdateSO(so);
            }

            this._cartMachine.ChangeSOGroupInfo();
            this._cartMachine.ChangeState();
        }

        public void CheckPayment()
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public void TransactToBackend()
        {
            throw new NotImplementedException();
        }


        public void PayComplete()
        {
            throw new NotImplementedException();
        }
    }
}
