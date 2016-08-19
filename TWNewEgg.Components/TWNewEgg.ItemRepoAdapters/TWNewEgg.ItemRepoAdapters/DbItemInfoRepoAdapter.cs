using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels;
using TWNewEgg.SellerRepoAdapters.Interface;

namespace TWNewEgg.ItemRepoAdapters
{
    public class DbItemInfoRepoAdapter : IDbItemInfoRepoAdapter
    {
        private IItemRepoAdapter _itemRepoAdapter;
        private IProductRepoAdapter _productRepoAdapter;
        private ISellerRepoAdapter _sellerRepoAdapter;

        public DbItemInfoRepoAdapter(IItemRepoAdapter itemRepoAdapter, IProductRepoAdapter productRepoAdapter, ISellerRepoAdapter sellerRepoAdapter)
        {
            this._itemRepoAdapter = itemRepoAdapter;
            this._productRepoAdapter = productRepoAdapter;
            this._sellerRepoAdapter = sellerRepoAdapter;
        }

        public IQueryable<DbItemInfo> GetDbItemInfos()
        {
            IQueryable<Item> items = this._itemRepoAdapter.GetAll();
            IQueryable<Product> products = this._productRepoAdapter.GetAll();
            IQueryable<Seller> sellers = this._sellerRepoAdapter.GetAll();
            IQueryable<DbItemInfo> query = items
                .Join(products,
                    i => i.ProductID,
                    p => p.ID,
                    (i, p) => new
                    {
                        item = i,
                        product = p
                    })
                .Join(sellers,
                    ip => ip.product.SellerID,
                    s => s.ID, (ip, s) => new DbItemInfo
                    {
                        item = ip.item,
                        product = ip.product,
                        seller = s
                    });
            return query;
        }
    }
}
