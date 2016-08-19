using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels;
using System.Data.SqlClient;
using TWNewEgg.Models.DomainModels.DataMaintain;

namespace TWNewEgg.CartRepoAdapters
{
    public class CartRepoAdapter : ICartRepoAdapter
    {
        private IBackendRepository<Cart> _cart;
        private IBackendRepository<Process> _process;

        public CartRepoAdapter(IBackendRepository<Cart> cart, IBackendRepository<Process> process)
        {
            this._cart = cart;
            this._process = process;
        }

        public int GetGroupBuyNumber(int ItemID, DateTime BeginDate, DateTime EndDate)
        {
            List<Nullable<int>> qtyList = _cart.GetAll().Where(x => x.CreateDate >= BeginDate && x.CreateDate <= EndDate && (x.Status == (int)Cart.status.正常 || x.Status == (int)Cart.status.完成))
                .Join(this._process.GetAll().Where(x => x.StoreID == ItemID && x.Qty != null), 
                c => c.ID,
                p => p.CartID,
                (c,p) => p.Qty).ToList();

            int qty = qtyList.Sum(x=>x.Value);
            return qty;
        }

        public Cart GetCart(string ID)
        {
            return this._cart.Get(x => x.ID == ID);
        }

        public Cart GetCart(DataMaintainSearchCondition_DM dataMaintainSearchCondition_DM)
        {
            Cart CartData = new Cart();
            string SearchKey = dataMaintainSearchCondition_DM.SearchKey;
            switch (dataMaintainSearchCondition_DM.SearchType)
            {
                case (int)DataMaintainSearchCondition_DM.SearchTypeDescription.CartID:
                    CartData = _cart.GetAll().Where(p => p.ID == SearchKey).FirstOrDefault();
                    break;
                default:
                    CartData = _cart.GetAll().Where(p => p.ID == SearchKey).FirstOrDefault();
                    break;
            }
            return CartData;
        }

        public Cart Update(Cart cart)
        {
            this._cart.Update(cart);
            return cart;
        }
        public IQueryable<Process> GetProcessByCartID(string CartId)
        {
            IQueryable<Process> queryResult = null;
            try
            {
                queryResult = this._process.GetAll().Where(x => x.CartID == CartId);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return queryResult;
          
        }
       
    }
}
