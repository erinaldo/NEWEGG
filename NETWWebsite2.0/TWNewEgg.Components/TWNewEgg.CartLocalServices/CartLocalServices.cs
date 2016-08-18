using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartLocalServices.Interface;
using TWNewEgg.SalesOrderGroupLocalRepoAdapters.Interface;
using TWNewEgg.SalesOrderItemLocalRepoAdapters.Interface;
using TWNewEgg.SalesOrderLocalRepoAdapters.Interface;

namespace TWNewEgg.CartLocalServices
{
    public class CartLocalServices : ICartLocalServices
    {
        private ISalesOrderGroupLocalRepoAdapters _iSalesOrderGroupLocalRepoAdapters;
        private ISalesOrderLocalRepoAdapters _iSalesOrderLocalRepoAdapters;
        private ISalesOrderItemLocalRepoAdapters _iSalesOrderItemLocalRepoAdapters;
        public CartLocalServices(ISalesOrderGroupLocalRepoAdapters iSalesOrderGroupLocalRepoAdapters,
            ISalesOrderLocalRepoAdapters iSalesOrderLocalRepoAdapters,
            ISalesOrderItemLocalRepoAdapters iSalesOrderItemLocalRepoAdapters)
        {
            this._iSalesOrderGroupLocalRepoAdapters = iSalesOrderGroupLocalRepoAdapters;
            this._iSalesOrderLocalRepoAdapters = iSalesOrderLocalRepoAdapters;
            this._iSalesOrderItemLocalRepoAdapters = iSalesOrderItemLocalRepoAdapters;
        }
        #region Test gateWay
        private string InsertTest()
        {
            string returnStr = "success";
            TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderGroup _data = new Models.DBModels.TWSQLDB.SalesOrderGroup();
            _data.ID = 0;
            _data.Note = "this is test";
            _data.OrderNum = 10;
            _data.Updated = 0;
            _data.Vaccunt = " ";
            _data.CreateDate = DateTime.Now;
            _data.CreateUser = "sys_test";
            try
            {
                this.insertSalesOrderGroupSQL(_data);
            }
            catch (Exception ex)
            {
                returnStr = ex.ToString();
            }

            return returnStr;
        }
        #endregion
        #region 寫入 local db (mysql)
        public void InsertIntoLocalDB(TWNewEgg.Models.DomainModels.Cart.DomainInsertSOGroupOrderItem _data)
        {
            TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderGroup insertToLocalSalesOrderGroup = this.BindLocalSalesOrderGroup(_data);
            List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrder> insertToLocalSalesOrder = this.BindLocalSalesOrder(_data);
            List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderItem> insertToLocalSalesItem = this.BindLocalSalesOrderItem(_data);

            //開始寫入 Mysql
            int insertMySqlToLocalSalesOrderGroupID = this.insertSalesOrderGroupSQL(insertToLocalSalesOrderGroup);
            this.insertSalesOrderSQL(insertToLocalSalesOrder, insertMySqlToLocalSalesOrderGroupID);
            this.insertSalesOrderSQLItem(insertToLocalSalesItem, insertMySqlToLocalSalesOrderGroupID);
        }
        #region insert data to mysql database
        private int insertSalesOrderGroupSQL(TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderGroup model)
        {
            int intResult = 0;
            int getIDNumber = 0;
            string getMaxIdSqlsqlCommand = "SELECT * FROM twsqldb.salesordergroup_getid limit 1;";
            intResult = this._iSalesOrderGroupLocalRepoAdapters.getData<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderGroup>(getMaxIdSqlsqlCommand).FirstOrDefault().ID;
            getIDNumber = intResult;
            getIDNumber++;
            string updateid = string.Format("update twsqldb.salesordergroup_getid set ID={0} where ID={1}", getIDNumber, intResult);
            this._iSalesOrderGroupLocalRepoAdapters.MysqlUpdate(updateid);
            model.ID = intResult;
            this._iSalesOrderGroupLocalRepoAdapters.MySQLCreate<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderGroup>(model, DAL.Model.MySQLModel.Table.SalesOrderGroup);
            return intResult;
        }
        private void insertSalesOrderSQL(List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrder> model, int salesOrderGroupId)
        {
            int i = 0;
            model.ForEach(p =>
            {
                p.SalesOrderGroupID = salesOrderGroupId;
                p.Code = salesOrderGroupId.ToString() + "_" + i.ToString();
                i++;
            });
            this._iSalesOrderLocalRepoAdapters.MySQLCreate<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrder>(model, TWNewEgg.DAL.Model.MySQLModel.Table.SalesOrder);
        }
        private void insertSalesOrderSQLItem(List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderItem> model, int salesOrderGroupId)
        {
            int i = 0;
            model.ForEach(p =>
            {
                p.Code = salesOrderGroupId.ToString() + "_" + i.ToString();
                i++;
                p.ActtkOut = salesOrderGroupId;
            });
            this._iSalesOrderItemLocalRepoAdapters.MySQLCreate<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderItem>(model, DAL.Model.MySQLModel.Table.SalesOrderItem);
        }
        #endregion
        #region map data from web to create the tables of salesordergroup, salesorder and salesorderitem
        private TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderGroup BindLocalSalesOrderGroup(TWNewEgg.Models.DomainModels.Cart.DomainInsertSOGroupOrderItem _data)
        {
            TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderGroup result = new Models.DBModels.TWSQLDB.SalesOrderGroup();
            TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderGroup _salesOrderGroup = new Models.DBModels.TWSQLDB.SalesOrderGroup();
            result.Note = _data.note.Replace("repdot", ",");
            result.PriceSum = (int)Math.Floor(0.5m + _data.pricesum);
            result.OrderNum = 0;
            result.CreateDate = DateTime.Now;
            result.Vaccunt = " ";
            result.CreateUser = " ";
            result.UpdateDate = DateTime.Now;
            result.UpdateUser = " ";
            result.Updated = 0;
            return result;
        }
        private List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrder> BindLocalSalesOrder(TWNewEgg.Models.DomainModels.Cart.DomainInsertSOGroupOrderItem _data)
        {

            List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrder> returnModelForSalesOrder = new List<Models.DBModels.TWSQLDB.SalesOrder>();
            List<string> itemIdList = _data.item_id.Replace("'", "").Split(',').Distinct().ToList();
            List<string> DelivTypeList = _data.salesorders_delivtype.Replace("'", "").Split(',').ToList();
            List<string> DelivDataList = _data.salesorders_delivdata.Replace("'", "").Split(',').ToList();
            List<string> NoteList = _data.salesorders_note.Split(',').ToList();
            int convertIntTemp = 0;

            for (int i = 0; i < itemIdList.Count; i++)
            {
                TWNewEgg.Models.DBModels.TWSQLDB.SalesOrder _saleOrder = new Models.DBModels.TWSQLDB.SalesOrder();
                _saleOrder.IDNO = _data.salesorder_idno.Replace("'", "");
                _saleOrder.Name = _data.salesorder_name.Replace("'", "");
                _saleOrder.AccountID = _data.salesorder_accountid;
                _saleOrder.TelDay = _data.salesorder_telday.Replace("'", "");
                _saleOrder.Mobile = _data.salesorder_mobile.Replace("'", "");
                _saleOrder.Email = _data.salesorder_email.Replace("'", "");
                _saleOrder.PayTypeID = _data.salesorder_paytypeid;
                _saleOrder.PayType = _data.salesorder_paytype;
                _saleOrder.StarvlDate = null;
                _saleOrder.CardHolder = string.Empty;
                _saleOrder.CardTelDay = string.Empty;
                _saleOrder.CardTelNight = string.Empty;
                _saleOrder.CardMobile = string.Empty;
                _saleOrder.CardLOC = _data.salesorder_cardloc.Replace("'", "");
                _saleOrder.CardZip = _data.salesorder_cardzip;
                _saleOrder.CardADDR = _data.salesorder_cardaddr.Replace("'", "");
                _saleOrder.CardNo = string.Empty; //_data.salesorder_cardno.Replace("'", "");
                _saleOrder.CardNochk = string.Empty;
                _saleOrder.CardType = _data.salesorder_cardtype.Replace("'", "");
                _saleOrder.CardBank = _data.salesorder_cardbank.Replace("'", "");
                _saleOrder.CardExpire = string.Empty;// _data.salesorder_cardexpire.Replace("'", "");
                _saleOrder.CardBirthday = _data.salesorder_cardbirthday;
                _saleOrder.InvoiceReceiver = _data.salesorder_recvname.Replace("'", ""); //_data.salesorder_invoreceiver.Replace("'", "");
                _saleOrder.InvoiceID = _data.salesorder_invoid.Replace("'", "");
                _saleOrder.InvoiceTitle = _data.salesorder_invotitle.Replace("'", "");
                _saleOrder.InvoiceLoc = _data.salesorder_delivloc.Replace("'", ""); //_data.salesorder_involoc.Replace("'", "");
                _saleOrder.InvoiceZip = _data.salesorder_invozip;
                _saleOrder.InvoiceAddr = _data.salesorder_delivaddr.Replace("'", ""); //_data.salesorder_invoaddr.Replace("'", "");
                _saleOrder.RecvName = _data.salesorder_recvname.Replace("'", "");
                _saleOrder.RecvEngName = _data.salesorder_recvengname.Replace("'", "");
                _saleOrder.RecvTelDay = _data.salesorder_recvtelday.Replace("'", "");
                _saleOrder.RecvTelNight = string.Empty;
                _saleOrder.RecvMobile = _data.salesorder_recvmobile.Replace("'", "");
                _saleOrder.DelivType = int.TryParse(DelivTypeList[i], out convertIntTemp) == true ? int.Parse(DelivTypeList[i]) : convertIntTemp;
                _saleOrder.DelivData = DelivDataList[i];
                _saleOrder.DelivLOC = _data.salesorder_delivloc.Replace("'", "");
                _saleOrder.DelivZip = _data.salesorder_delivzip;
                _saleOrder.DelivADDR = _data.salesorder_delivaddr.Replace("'", "");
                _saleOrder.DelivEngADDR = _data.salesorder_delivengaddr.Replace("'", "");
                _saleOrder.DelivHitNote = string.Empty;
                _saleOrder.ConfirmDate = null;
                _saleOrder.ConfirmNote = string.Empty;
                _saleOrder.AuthDate = _data.salesorder_authdate;
                _saleOrder.AuthCode = _data.salesorder_authcode.Replace("'", "");
                _saleOrder.AuthNote = _data.salesorder_authnote.Replace("'", "");
                _saleOrder.HpType = null;
                _saleOrder.RcptDate = null;
                _saleOrder.RcptNote = string.Empty;
                _saleOrder.Expire = null;
                _saleOrder.DateDEL = null;
                _saleOrder.CoServerName = _data.salesorder_coservername.Replace("'", "");
                _saleOrder.ServerName = _data.salesorder_servername.Replace("'", "");
                _saleOrder.ActCode = string.Empty;
                _saleOrder.Status = _data.salesorder_status;
                _saleOrder.StatusNote = string.Empty;
                _saleOrder.RemoteIP = _data.salesorder_remoteip.Replace("'", "");
                _saleOrder.Date = DateTime.UtcNow.AddHours(8);
                _saleOrder.Note = NoteList[i];
                _saleOrder.Note2 = string.Empty;
                _saleOrder.CreateUser = "sys_insert";
                _saleOrder.CreateDate = DateTime.UtcNow.AddHours(8);
                _saleOrder.Updated = 0;
                _saleOrder.UpdateUser = string.Empty;
                _saleOrder.UpdateDate = null;
                //_saleOrder.invoiceCarrierType = _data
                returnModelForSalesOrder.Add(_saleOrder);
            }
            return returnModelForSalesOrder;
        }
        private List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderItem> BindLocalSalesOrderItem(TWNewEgg.Models.DomainModels.Cart.DomainInsertSOGroupOrderItem _data)
        {
            List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderItem> salesOrderItemList = new List<Models.DBModels.TWSQLDB.SalesOrderItem>();
            List<string> itemIdList = _data.item_id.Replace("'", "").Split(',').ToList();
            List<string> itemIdListDis = itemIdList.Distinct().ToList();
            List<string> priceItemsList = _data.salesorderitems_price.Replace("'", "").Split(',').ToList();
            List<string> redmtkOutlist = _data.salesorderitems_redmtkout.Replace("'", "").Split(',').ToList();
            List<string> redmBLNList = _data.salesorderitems_redmbln.Replace("'", "").Split(',').ToList();
            List<string> redmfdbckList = _data.salesorderitems_redmfdbck.Replace("'", "").Split(',').ToList();
            List<string> wftkOutList = _data.salesorderitems_wftkout.Replace("'", "").Split(',').ToList();
            List<string> wfBLNList = _data.salesorderitems_wfbln.Replace("'", "").Split(',').ToList();
            List<string> actIDList = _data.salesorderitems_actid.Replace("'", "").Split(',').ToList();
            List<string> acttkOutList = _data.salesorderitems_acttkout.Replace("'", "").Split(',').ToList();
            List<string> displayPriceList = _data.salesorderitems_displayprice.Replace("'", "").Split(',').ToList();
            List<string> discountPriceList = _data.salesorderitems_discountprice.Replace("'", "").Split(',').ToList();
            List<string> shippingExpenseList = _data.salesorderitems_shippingexpense.Replace("'", "").Split(',').ToList();
            List<string> serviceExpenseList = _data.salesorderitems_serviceexpense.Replace("'", "").Split(',').ToList();
            List<string> taxList = _data.salesorderitems_tax.Replace("'", "").Split(',').ToList();
            List<string> itemPriceSumList = _data.salesorderitems_itempricesum.Replace("'", "").Split(',').ToList();
            List<string> installmentFeeList = _data.salesorderitems_installmentfee.Replace("'", "").Split(',').ToList();
            List<string> isNewList = _data.salesorderitems_isnew.Replace("'", "").Split(',').ToList();
            List<string> couponsList = _data.salesorderitems_coupons.Replace("'", "").Split(',').ToList();
            List<string> priceinstList = _data.salesorderitems_priceinst.Replace("'", "").Split(',').ToList();
            List<string> qtyList = _data.salesorderitems_qty.Replace("'", "").Split(',').ToList();
            List<string> pricecouponList = _data.salesorderitems_pricecoupon.Replace("'", "").Split(',').ToList();
            List<string> noteList = _data.salesorderitems_note.Split(',').ToList();

            //var itemItem = this._iItemRepoAdapter.GetItemList(itemIdListDis.Select(int.Parse).ToList());
            //var itemList = itemItem.Select(p => p.Value).ToList();
            int convertToIntTemp = 0;
            decimal convertToDecimalTemp = 0;
            int itemIdTemp = int.Parse(itemIdList.First());
            int soIndexTemp = 0;
            for (int i = 0; i < itemIdList.Count; i++)
            {
                TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderItem salesOrderItemTemp = new Models.DBModels.TWSQLDB.SalesOrderItem();
                salesOrderItemTemp.ItemID = int.TryParse(itemIdList[i], out convertToIntTemp) == true ? int.Parse(itemIdList[i]) : 0;


                if (salesOrderItemTemp.ItemID == itemIdTemp)
                {
                    salesOrderItemTemp.SalesorderCode = soIndexTemp.ToString();
                }
                else
                {
                    soIndexTemp++;
                    salesOrderItemTemp.SalesorderCode = soIndexTemp.ToString();
                    itemIdTemp = salesOrderItemTemp.ItemID;
                }
                salesOrderItemTemp.ItemlistID = 0;
                salesOrderItemTemp.ProductID = 0;// itemList.Where(p => p.ID == salesOrderItemTemp.ItemID).Select(p => p.ProductID).FirstOrDefault();
                salesOrderItemTemp.ProductlistID = 0;
                salesOrderItemTemp.Name = string.Empty;// itemList.Where(p => p.ID == salesOrderItemTemp.ItemID).Select(p => p.Name).FirstOrDefault().Replace("'", "");
                salesOrderItemTemp.Price = decimal.TryParse(priceItemsList[i], out convertToDecimalTemp) == true ? decimal.Parse(priceItemsList[i]) : 0;
                salesOrderItemTemp.Priceinst = decimal.TryParse(priceinstList[i], out convertToDecimalTemp) == true ? decimal.Parse(priceinstList[i]) : 0;
                salesOrderItemTemp.Qty = int.TryParse(qtyList[i], out convertToIntTemp) == true ? int.Parse(qtyList[i]) : 0;
                salesOrderItemTemp.Pricecoupon = decimal.TryParse(pricecouponList[i], out convertToDecimalTemp) == true ? decimal.Parse(pricecouponList[i]) : 0;
                salesOrderItemTemp.Coupons = string.Empty;//訂單成立後才進行寫入
                salesOrderItemTemp.RedmtkOut = int.TryParse(redmtkOutlist[i], out convertToIntTemp) == true ? int.Parse(redmtkOutlist[i]) : 0;
                salesOrderItemTemp.RedmBLN = int.TryParse(redmBLNList[i], out convertToIntTemp) == true ? int.Parse(redmBLNList[i]) : 0;
                salesOrderItemTemp.Redmfdbck = int.TryParse(redmfdbckList[i], out convertToIntTemp) == true ? int.Parse(redmfdbckList[i]) : 0;
                salesOrderItemTemp.Status = null;//沒再用
                salesOrderItemTemp.StatusNote = null;
                salesOrderItemTemp.Date = DateTime.Now;
                salesOrderItemTemp.Attribs = string.Empty;
                salesOrderItemTemp.Note = noteList[i];
                salesOrderItemTemp.WftkOut = int.TryParse(wftkOutList[i], out convertToIntTemp) == true ? int.Parse(wftkOutList[i]) : 0;
                salesOrderItemTemp.WfBLN = int.TryParse(wfBLNList[i], out convertToIntTemp) == true ? int.Parse(wfBLNList[i]) : 0;
                salesOrderItemTemp.AdjPrice = null;
                salesOrderItemTemp.ActID = actIDList[i].Replace("'", "");
                salesOrderItemTemp.ActtkOut = int.TryParse(acttkOutList[i], out convertToIntTemp) == true ? int.Parse(acttkOutList[i]) : 0;
                salesOrderItemTemp.ProdcutCostID = null;
                salesOrderItemTemp.CreateUser = string.Empty;
                salesOrderItemTemp.CreateDate = DateTime.Now;
                salesOrderItemTemp.Updated = 0;
                salesOrderItemTemp.UpdateDate = null;
                salesOrderItemTemp.UpdateUser = string.Empty;
                salesOrderItemTemp.DisplayPrice = decimal.TryParse(displayPriceList[i], out convertToDecimalTemp) == true ? decimal.Parse(displayPriceList[i]) : 0;
                salesOrderItemTemp.DiscountPrice = decimal.TryParse(discountPriceList[i], out convertToDecimalTemp) == true ? decimal.Parse(discountPriceList[i]) : 0;//decimal.Parse(discountPriceList[i]);
                salesOrderItemTemp.ShippingExpense = decimal.TryParse(shippingExpenseList[i], out convertToDecimalTemp) == true ? decimal.Parse(shippingExpenseList[i]) : 0; //decimal.Parse(shippingExpenseList[i]);
                salesOrderItemTemp.ServiceExpense = decimal.TryParse(serviceExpenseList[i], out convertToDecimalTemp) == true ? decimal.Parse(serviceExpenseList[i]) : 0;//decimal.Parse(serviceExpenseList[i]);
                salesOrderItemTemp.Tax = decimal.TryParse(taxList[i], out convertToDecimalTemp) == true ? decimal.Parse(taxList[i]) : 0;// decimal.Parse(taxList[i]);
                salesOrderItemTemp.WarehouseID = null;
                salesOrderItemTemp.ItemPriceSum = decimal.TryParse(itemPriceSumList[i], out convertToDecimalTemp) == true ? decimal.Parse(itemPriceSumList[i]) : 0;// decimal.Parse(itemPriceSumList[i]);
                salesOrderItemTemp.InstallmentFee = decimal.TryParse(installmentFeeList[i], out convertToDecimalTemp) == true ? decimal.Parse(installmentFeeList[i]) : 0; //decimal.Parse(installmentFeeList[i]);
                salesOrderItemTemp.IsNew = isNewList[i].Replace("'", "");
                salesOrderItemTemp.ApportionedAmount = 0;
                salesOrderItemTemp.SupplyShippingCharge = 0;
                //salesOrderItemTemp.ApportionedAmount,   滿額贈分攤金額, 訂單成立後才進行寫入
                //salesOrderItemTemp.SupplyShippingCharge, from  product coulmn 訂單成立後才進行寫入
                salesOrderItemList.Add(salesOrderItemTemp);


            }


            return salesOrderItemList;
        }
        #endregion
        #endregion
    }
}
