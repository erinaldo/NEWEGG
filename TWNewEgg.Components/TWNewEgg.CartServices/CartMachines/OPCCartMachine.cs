using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartServices.Interface;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Cart;

namespace TWNewEgg.CartServices.CartMachines
{
    public class OPCCartMachine : ICartMachine
    {
        public SOGroupInfo soGroupInfo { get; private set; }

        //所有狀態
        private ICartState _cartInitialState;
        private ICartState _cartFailedState;
        private ICartState _cartCompletedState;
        private ICartState _cartNotPayedState;
        private ICartState _cartPayedState;

        //當前狀態
        private ICartState _currentState;

        private ISOGroupInfoService _soGroupInfoService;
        public OPCCartMachine(ISOGroupInfoService soGroupInfoService)
        {
            this._soGroupInfoService = soGroupInfoService;
        }

        public void InitialMachine(int soGroupId)
        {
            this.soGroupInfo = this._soGroupInfoService.GetSOGroupInfo(soGroupId);
            if (soGroupInfo == null)
            {
                throw new NullReferenceException("Can't find SalesorderGroup");
            }

            this.RegisterStates();
            this.ChangeState();
        }

        public void InitialMachine(SOGroupInfo soGroup)
        {
            this.soGroupInfo = soGroup;
            if (soGroupInfo == null)
            {
                throw new NullReferenceException("Can't find SalesorderGroup");
            }

            this.RegisterStates();
            this.ChangeState();
        }

        private void RegisterStates()
        {
            this._cartInitialState = (ICartState)AutofacConfig.Container.ResolveKeyed("Cart_Initial", typeof(ICartState));
            this._cartNotPayedState = (ICartState)AutofacConfig.Container.ResolveKeyed("Cart_NotPayed", typeof(ICartState));
            this._cartPayedState = (ICartState)AutofacConfig.Container.ResolveKeyed("Cart_Payed", typeof(ICartState));
            this._cartCompletedState = (ICartState)AutofacConfig.Container.ResolveKeyed("Cart_Completed", typeof(ICartState));
            this._cartFailedState = (ICartState)AutofacConfig.Container.ResolveKeyed("Cart_Failed", typeof(ICartState));
        }

        public void ChangeSOGroupInfo()
        {
            int soGroupId = this.soGroupInfo.Main.ID;
            this.soGroupInfo = this._soGroupInfoService.GetSOGroupInfo(soGroupId);
        }

        public void ChangeState()
        {
            switch (this.soGroupInfo.Status)
            {
                case SOGroupInfo.SOGroupStatus.Initial:
                    this._currentState = this._cartInitialState;
                    break;
                case SOGroupInfo.SOGroupStatus.NotPayed:
                    this._currentState = this._cartNotPayedState;
                    break;
                case SOGroupInfo.SOGroupStatus.Payed:
                    this._currentState = this._cartPayedState;
                    break;
                case SOGroupInfo.SOGroupStatus.Completed:
                    this._currentState = this._cartCompletedState;
                    break;
                case SOGroupInfo.SOGroupStatus.Failed:
                    this._currentState = this._cartFailedState;
                    break;
                default:
                    throw new Exception("Invalid Status");
            }

            this._currentState.Init(this);
        }

        public void Pay(int orderStatus)
        {
            this._currentState.Pay(orderStatus);
        }

        public void PayComplete()
        {
            this._currentState.PayComplete();
        }

        public void Cancel()
        {
            this._currentState.Cancel();
        }

        public void TransactToBackend()
        {
            this._currentState.TransactToBackend();
        }

        public void CheckPayment()
        {
            this._currentState.CheckPayment();
        }

        public decimal GetTotalPrice()
        {
            decimal totalPrice = 0.0m;

            foreach (var singleSO in this.soGroupInfo.SalesOrders)
            {
                foreach (var singleSOItem in singleSO.SOItems)
                {
                    totalPrice += singleSOItem.DisplayPrice.Value + (decimal)Math.Round(double.Parse(singleSOItem.InstallmentFee.ToString()), 0, MidpointRounding.AwayFromZero) - ((singleSOItem.Pricecoupon == null) ? 0 : singleSOItem.Pricecoupon.Value) - singleSOItem.ApportionedAmount;
                }
            }

            return totalPrice;
        }
    }
}
