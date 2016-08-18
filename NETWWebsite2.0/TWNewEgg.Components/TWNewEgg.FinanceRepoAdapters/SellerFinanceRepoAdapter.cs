using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.FinanceRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DBModels.TWBACKENDDBExtModels;
using TWNewEgg.Models.DBModels.TWSQLDB;
using AutoMapper;

namespace TWNewEgg.FinanceRepoAdapters
{
    public class SellerFinanceRepoAdapter : ISellerFinanceRepoAdapter
    {
        log4net.ILog _logger;

        IRepository<Item> _itemRepo;
        IRepository<Seller> _sellerRepo;
        IRepository<Country> _countryRepo;
        IBackendRepository<ItemInStock_trans> _itemInStockTransRepo;
        IBackendRepository<Seller_FinanDetail> _sellerFinanDetailRepo;        

        IBackendRepository<Retgood> _retgoodRepo;
            
        public SellerFinanceRepoAdapter(IRepository<Item> itemRepo, IRepository<Seller> sellerRepo, IRepository<Country> countryRepo,
            IBackendRepository<ItemInStock_trans> itemInStockTransRepo, IBackendRepository<Seller_FinanDetail> sellerFinanDetailRepo, IBackendRepository<Retgood> retgoodRepo)
        {
            this._logger = log4net.LogManager.GetLogger(LoggerInfo.FinanceLog);

            this._itemRepo = itemRepo;
            this._sellerRepo = sellerRepo;
            this._countryRepo = countryRepo;

            this._itemInStockTransRepo = itemInStockTransRepo;
            this._sellerFinanDetailRepo = sellerFinanDetailRepo;
            this._retgoodRepo = retgoodRepo;
        }

        public void SaveFinanDetail(Seller_FinanDetail finanDetail)
        {
            Seller_FinanDetail info = this._sellerFinanDetailRepo.Get(x => x.OrderID == finanDetail.OrderID);
            if (info == null)
            {
                this._sellerFinanDetailRepo.Create(finanDetail);
            }
            //else if ((string.IsNullOrWhiteSpace(info.IsCheck) || info.IsCheck != "Y") && string.IsNullOrWhiteSpace(info.SettlementID))
            else if (string.IsNullOrWhiteSpace(info.IsCheck) || info.IsCheck != "Y")
            {
                Mapper.Map(finanDetail, info);

                this._sellerFinanDetailRepo.Update(info);
            }         
        }

        public Seller_FinanDetail GetFinanDetail(string orderID)
        {
            return this._sellerFinanDetailRepo.Get(x => x.OrderID == orderID);
        }

        public bool IsExistFinanDetail(string orderID)
        {
            IQueryable<Seller_FinanDetail> list = this._sellerFinanDetailRepo.GetAll().Where(x => x.OrderID == orderID);

            if (list.Count() == 0)
                return false;
            else
                return true;
        }
        
        public Seller GetSellerVendor(int storeID)
        {
            IQueryable<Seller> list = from item in this._itemRepo.GetAll().Where(x => x.ID == storeID)
                                      join seller in this._sellerRepo.GetAll()
                                      on item.SellerID equals seller.ID
                                      select seller;

            return list.FirstOrDefault();
        }

        public Seller GetSellerVendorForPurchaseOrder(int sellerID)
        {
            return this._sellerRepo.Get(x => x.ID == sellerID);
        }

        public string GetOrderVendorID(int storeID)
        {
            var itemInfo = (from item in this._itemRepo.GetAll().Where(x => x.ID == storeID)
                            join seller in this._sellerRepo.GetAll()
                            on item.SellerID equals seller.ID
                            join country in this._countryRepo.GetAll()
                            on seller.CountryID equals country.ID
                            select new
                            {
                                Seller = seller,
                                Country = country
                            }).FirstOrDefault();

            return GetVendorStringID(itemInfo.Seller, itemInfo.Country);
        }

        private string GetVendorStringID(Seller sellerData, Country countryData)
        {
            string vendorIDPrefix = ReturnVendorIDPrefix(sellerData.AccountType);
            string vendorString = "";

            if (vendorIDPrefix == "")
            {
                return vendorString;
            }

            if ((countryData.Name == "Taiwan" || countryData.Name == "台灣" || countryData.Name == "臺灣") && (sellerData.Identy == (int)Seller.IdentyType.國內廠商 || sellerData.Identy == null))
            {
                string VAT_NOtemp = sellerData.VAT_NO ?? "";
                //if (VAT_NOtemp.Length != 8)
                if (VAT_NOtemp.Length > 8)
                {
                    vendorString = vendorIDPrefix;
                }
                else
                {
                    vendorString = vendorIDPrefix + (VAT_NOtemp).PadLeft(8, '0');
                }
            }
            else
            {
                vendorString = vendorIDPrefix + sellerData.ID.ToString().PadLeft(8, '0');
            }
            return vendorString;
        }

        private string ReturnVendorIDPrefix(string type)
        {
            string returnWord = "";

            switch (type.Trim())
            {
                case "V":
                case "S":
                case "M":
                case "F":
                case "P":
                case "B":
                    returnWord = "W" + type.Trim();
                    break;
                default:
                    returnWord = "WS";
                    break;
            }
            return returnWord;
        }

        public IQueryable<ItemInStock_trans> GetItemInStockTrans(AccountsDocumentType.DocTypeEnum docType, string codeNumber)
        {
            IQueryable<ItemInStock_trans> list = null;

            switch (docType)
            {
                case AccountsDocumentType.DocTypeEnum.XI:
                    //LBO
                    list = this._itemInStockTransRepo.GetAll().Where(x => x.SOCode == codeNumber);
                    break;
                case AccountsDocumentType.DocTypeEnum.XIRMA:
                    //LBR
                    list = this._itemInStockTransRepo.GetAll().Where(x => x.POCode == codeNumber);
                    break;
            }

            return list;
        }
      
    }
}
