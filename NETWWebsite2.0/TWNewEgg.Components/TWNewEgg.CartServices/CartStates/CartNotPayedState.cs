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
    public class CartNotPayedState: ICartState
    {
        private OPCCartMachine _cartMachine;

        private ISORepoAdapter _soRepoAdapter;
        public CartNotPayedState(ISORepoAdapter soRepoAdapter)
        {
            this._soRepoAdapter = soRepoAdapter;
        }

        public void Init(OPCCartMachine cartMachine)
        {
            this._cartMachine = cartMachine;
        }

        public void Pay(int orderStatus)
        {
            throw new NotImplementedException();
        }

        public void CheckPayment() { }

        public void Cancel()
        {
            List<SOBase> soBases = this._cartMachine.soGroupInfo.SalesOrders.Select(x => x.Main).ToList();
            foreach (SOBase soBase in soBases)
            {
                soBase.Status = (int)SalesOrder.status.付款失敗取消訂單;
                SalesOrder so = ModelConverter.ConvertTo<SalesOrder>(soBase);
                this._soRepoAdapter.UpdateSO(so);
            }

            this._cartMachine.ChangeSOGroupInfo();
            this._cartMachine.ChangeState();
        }

        public void TransactToBackend()
        {
            throw new NotImplementedException();
        }

        public void PayComplete()
        {
            List<SOBase> soBases = this._cartMachine.soGroupInfo.SalesOrders.Select(x => x.Main).ToList();
            foreach (SOBase soBase in soBases)
            {
                soBase.Status = (int)SalesOrder.status.付款成功;
                SalesOrder so = ModelConverter.ConvertTo<SalesOrder>(soBase);
                this._soRepoAdapter.UpdateSO(so);
            }

            this._cartMachine.ChangeSOGroupInfo();
            this._cartMachine.ChangeState();
        }
    }
}
