using Autofac;
using salesordergroup.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.CartServices.Interface;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.SalesOrderItemRepoAdapters.Interface;
using TWNewEgg.SalesOrderRepoAdapters.Interface;

namespace TWNewEgg.CartServices.SOServices
{
    public class SalesOrderInfoService : ISalesOrderInfoService, IDisposable
    {
        private string MySQLRecordSwitch = System.Configuration.ConfigurationManager.AppSettings["MySQLRecord"] == null ? "false" : System.Configuration.ConfigurationManager.AppSettings["MySQLRecord"].ToString();
        //private string MySQLInsertRequest = System.Configuration.ConfigurationManager.AppSettings["MySQLInsertRequest"] == null ? "false" : System.Configuration.ConfigurationManager.AppSettings["MySQLInsertRequest"].ToString();
        private string BothOfMSSQLAndMySQL = System.Configuration.ConfigurationManager.AppSettings["BothOfMSSQLAndMySQL"] == null ? "false" : System.Configuration.ConfigurationManager.AppSettings["BothOfMSSQLAndMySQL"].ToString();

        private ILifetimeScope autofacScope;
        private ISORepoAdapter _soReadRepoAdapter;
        private IItemRepoAdapter _iItemRepoAdapter;
        private ISalesOrderGroupRepoAdapters _soGroupRepo;        
        private ISalesOrderItemRepoAdapters _soItemRepo;
        private ISalesOrderRepoAdapters _soRepo;

        private Lazy<ISalesOrderGroupRepoAdapters> _lazyISalesOrderGroupRepoAdapters;

        public SalesOrderInfoService(Lazy<ISalesOrderGroupRepoAdapters> lazyISalesOrderGroupRepoAdapters)
        {
            this.autofacScope = AutofacConfig.Container.BeginLifetimeScope();
            this._soReadRepoAdapter = (ISORepoAdapter)this.autofacScope.Resolve(typeof(ISORepoAdapter));
            this._iItemRepoAdapter = (IItemRepoAdapter)this.autofacScope.Resolve(typeof(IItemRepoAdapter));

            this._lazyISalesOrderGroupRepoAdapters = lazyISalesOrderGroupRepoAdapters;
        }
        private bool MySQLRecordSoInfo(DomainInsertSOGroupOrderItem _data)
        {
            //檢查對正式資料庫的連線狀態
            bool azureConnected = this._lazyISalesOrderGroupRepoAdapters.Value.isConnected();
            //連線異常，開始把資料寫入 MySQL
            if (azureConnected == false)
            {
                var SOInfoRepoServices = AutofacConfig.Container.Resolve<TWNewEgg.CartLocalServices.Interface.ICartLocalServices>();
                SOInfoRepoServices.InsertIntoLocalDB(_data);
            }
            return azureConnected;
        }

        private List<DemainInsertSalesOrdersBySellerOutput> insertSoinfoDatas(DomainInsertSOGroupOrderItem _data, string soFlowSelect)
        {
            List<DemainInsertSalesOrdersBySellerOutput> returnResult = new List<DemainInsertSalesOrdersBySellerOutput>();
            string soRepositoryServiceKey = "Temp";
            if (soFlowSelect == "New")
            {
                soRepositoryServiceKey = "Formal";
            }
            this._soGroupRepo = (ISalesOrderGroupRepoAdapters)this.autofacScope.ResolveKeyed(soRepositoryServiceKey, typeof(ISalesOrderGroupRepoAdapters));
            this._soRepo = (ISalesOrderRepoAdapters)this.autofacScope.ResolveKeyed(soRepositoryServiceKey, typeof(ISalesOrderRepoAdapters));
            this._soItemRepo = (ISalesOrderItemRepoAdapters)this.autofacScope.ResolveKeyed(soRepositoryServiceKey, typeof(ISalesOrderItemRepoAdapters));
            //建立 SalesOrderGroup model
            TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderGroup insertModelSalesOrderGroup = this.MapToSalesOrderGroup(_data);
            //建立 SalesOrder model
            List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrder> salesOrderList = this.MapToSaleOrder(_data);
            //建立 SalesOrderItems model
            List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderItem> salesOrderItemList = this.MapToSaleOrderItem(_data);
            // 開始寫入資料
            returnResult = this.insertSOInfoTable(insertModelSalesOrderGroup, salesOrderList, salesOrderItemList);
            //更新庫存
            if (soRepositoryServiceKey == "Formal")
            {
                this.UpdateItem(salesOrderItemList);
            }
            return returnResult;
        }
        /// <summary>
        /// 建立 salesOrderGroup, salesOrder, salesOrderItems
        /// </summary>
        /// <param name="_data"></param>
        /// <returns></returns>
        public List<DemainInsertSalesOrdersBySellerOutput> CreateSoInfo(DomainInsertSOGroupOrderItem _data, string soFlowSelect)
        {

            List<DemainInsertSalesOrdersBySellerOutput> returnResult = new List<DemainInsertSalesOrdersBySellerOutput>();
            //是否要同時寫入 MSSQL and MySQL
            if (BothOfMSSQLAndMySQL.ToLower() == "true")
            {
                //寫入 Mysql
                var SOInfoRepoServices = AutofacConfig.Container.Resolve<TWNewEgg.CartLocalServices.Interface.ICartLocalServices>();
                SOInfoRepoServices.InsertIntoLocalDB(_data);

                //寫入 MSSQL
                returnResult = this.insertSoinfoDatas(_data, soFlowSelect);
            }
            else
            {
                //不同時寫入 mssql and mysql
                //先判斷是否要檢查正式資料庫連線是否正常
                if (MySQLRecordSwitch.ToLower() == "true")
                {
                    //要檢查連線狀態
                    bool isMySQLRecord = this.MySQLRecordSoInfo(_data);
                    //正式資料庫連線不正常，不把資料寫入正式資料庫
                    if (isMySQLRecord == false)
                    {
                        throw new Exception("Service conncet error, 訂單成立失敗");
                    }
                    else
                    {
                        //連線正常，進行對正是資料庫的資料寫入
                        returnResult = this.insertSoinfoDatas(_data, soFlowSelect);
                    }
                }
                else
                {
                    //不判斷對資料庫連線是否正常，直接進行對正是資料庫的資料寫入
                    returnResult = this.insertSoinfoDatas(_data, soFlowSelect);
                }
            }

            return returnResult;
        }

        private void UpdateItem(List<SalesOrderItem> soItems)
        {
            List<Item> updateItems = new List<Item>();
            foreach (SalesOrderItem soItem in soItems)
            {
                Item item = this._iItemRepoAdapter.GetAll().Where(x => soItem.ItemID == x.ID).FirstOrDefault();
                if (item != null)
                {
                    item.QtyReg += 1;
                    item.Updated += 1;
                    item.UpdateUser = "sys_QtyReg+1";
                    updateItems.Add(item);
                }
            }
            this._iItemRepoAdapter.UpdateItemList(updateItems);
        }

        #region Map _data to SalesOrderGroup, SalesOrder and SalesOrderitems
        private TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderGroup MapToSalesOrderGroup(DomainInsertSOGroupOrderItem _data)
        {
            TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderGroup _salesOrderGroup = new Models.DBModels.TWSQLDB.SalesOrderGroup();
            _salesOrderGroup.Note = _data.note.Replace("repdot", ",");
            _salesOrderGroup.PriceSum = (int)Math.Floor(0.5m + _data.pricesum);
            _salesOrderGroup.OrderNum = this.orderNumCount(_data.salesorder_accountid);
            _salesOrderGroup.CreateDate = DateTime.Now;
            _salesOrderGroup.Vaccunt = " ";
            _salesOrderGroup.CreateUser = " ";
            _salesOrderGroup.UpdateDate = DateTime.Now;
            _salesOrderGroup.UpdateUser = " ";
            _salesOrderGroup.Updated = 0;
            return _salesOrderGroup;
        }
        private List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrder> MapToSaleOrder(DomainInsertSOGroupOrderItem _data)
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
        private List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderItem> MapToSaleOrderItem(DomainInsertSOGroupOrderItem _data)
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

            var itemItem = this._iItemRepoAdapter.GetItemList(itemIdListDis.Select(int.Parse).ToList());
            var itemList = itemItem.Select(p => p.Value).ToList();
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
                salesOrderItemTemp.ProductID = itemList.Where(p => p.ID == salesOrderItemTemp.ItemID).Select(p => p.ProductID).FirstOrDefault();
                salesOrderItemTemp.ProductlistID = 0;
                salesOrderItemTemp.Name = itemList.Where(p => p.ID == salesOrderItemTemp.ItemID).Select(p => p.Name).FirstOrDefault().Replace("'", "");
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
        private List<DemainInsertSalesOrdersBySellerOutput> insertSOInfoTable(TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderGroup insertModelSalesOrderGroup, List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrder> salesOrderList, List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderItem> salesOrderItemList)
        {
            List<DemainInsertSalesOrdersBySellerOutput> SOInfoReturnModel = new List<DemainInsertSalesOrdersBySellerOutput>();
            using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
            {
                try
                {
                    //建立 salesOrderGroup 取的 id
                    int SalesOrderGroupIdByInsert = this.insertIntoSalesOrderGroup(insertModelSalesOrderGroup);
                    //取 LBO 最大值並且判斷有幾張主單產生對應的 LBOpp
                    var NewSoList = this.computLBOOrLBS(salesOrderList.Count, "LBO");
                    //取 LBS 最大值並且判斷有幾張主單產生對應的 LBS
                    var NewSoItemsList = this.computLBOOrLBS(salesOrderItemList.Count, "LBS");
                    //把取號好的 LBO 回塞 model
                    salesOrderList = this.MapSOCode(salesOrderList, NewSoList, SalesOrderGroupIdByInsert);
                    //把取號好的 LBS 回塞 model
                    salesOrderItemList = this.MapSOItem(salesOrderItemList, salesOrderList.Select(p => p.Code).ToList(), NewSoItemsList);

                    this.insertIntoSalesOrder(salesOrderList);
                    this.insertIntoSalesOrderItems(salesOrderItemList);
                    SOInfoReturnModel = this.mapToSalesOrderBySellerOutput(salesOrderList, salesOrderItemList, SalesOrderGroupIdByInsert);
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    SOInfoReturnModel = null;
                    throw new Exception(ex.ToString());
                }
            }
            return SOInfoReturnModel;
        }


        #region 取 LBO 和 LBS 最大值的 Method
        /// <summary>
        /// 取 LBO or LBS max values
        /// </summary>
        /// <param name="record"></param>
        /// <param name="soType"></param>
        /// <returns></returns>
        private List<string> computLBOOrLBS(int record, string soType)
        {
            List<string> returnStrModel = new List<string>();
            switch (soType)
            {
                case "LBO":
                    {
                        returnStrModel = this.computeSalesOrderCodes(record, soType);
                        break;
                    }

                case "LBS":
                    {
                        returnStrModel = this.computeSalesOrderItems(record, soType);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return returnStrModel;
        }

        private List<string> computeSalesOrderItems(int count, string type)
        {
            string yyMMdd = type + DateTime.Now.ToString("yyMMdd");
            List<string> returnStrModel = new List<string>();
            var soItemsData = this._soItemRepo.GetAll().Where(p => p.Code.Substring(0, 9) == yyMMdd).ToList();
            if (soItemsData.Count != 0)
            {
                decimal intLBSCodeMax = (from p in soItemsData select Convert.ToDecimal(p.Code.Substring(9, p.Code.Length - 9))).Max();
                for (int i = 1; i <= count; i++)
                {
                    returnStrModel.Add(yyMMdd + string.Format("{0:000000}", intLBSCodeMax + i));
                }
            }
            else
            {
                for (int i = 1; i < count + 1; i++)
                {
                    returnStrModel.Add(yyMMdd + string.Format("{0:000000}", i));
                }
            }
            return returnStrModel;
        }
        private List<string> computeSalesOrderCodes(int count, string type)
        {
            string yyMMdd = type + DateTime.Now.ToString("yyMMdd");
            List<string> returnStrModel = new List<string>();
            var soData = this._soReadRepoAdapter.GetSOAllData().Where(p => p.Code.Substring(0, 9) == yyMMdd).ToList();//.Max(p => p.Code);
            if (soData.Count != 0)
            {
                decimal intLBSCodeMax = (from p in soData select Convert.ToDecimal(p.Code.Substring(9, p.Code.Length - 9))).Max();
                for (int i = 1; i <= count; i++)
                {
                    returnStrModel.Add(yyMMdd + string.Format("{0:000000}", intLBSCodeMax + i));
                }
            }
            else
            {
                for (int i = 1; i < count + 1; i++)
                {
                    returnStrModel.Add(yyMMdd + string.Format("{0:000000}", i));
                }
            }
            return returnStrModel;
        }
        #endregion
        private int insertIntoSalesOrderGroup(TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderGroup insertModel)
        {
            this._soGroupRepo.insertSalesordergroup(insertModel);
            return insertModel.ID;
        }
        private void insertIntoSalesOrder(List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrder> insertModel)
        {
            this._soRepo.CreateSalesOrders(insertModel);
        }
        private void insertIntoSalesOrderItems(List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderItem> insertModel)
        {
            this._soItemRepo.CreateSalesOrderItems(insertModel);
        }
        
        private int orderNumCount(int accountId = 0)
        {
            int resultInt = -1;
            if (accountId == 0)
            {
                return -1;
            }
            var salesorderData = this._soReadRepoAdapter.GetSOAllData();
            var salesOrderGroupData = this._soGroupRepo.GetAll();
            resultInt = (from p in salesorderData
                         join q in salesOrderGroupData on p.SalesOrderGroupID equals q.ID
                         where p.AccountID == accountId
                         select new
                         {
                             _orderNum = q.OrderNum,
                             _salesOrderGroupId = p.SalesOrderGroupID
                         }).OrderByDescending(r => r._salesOrderGroupId).FirstOrDefault()._orderNum;
            resultInt++;
            return resultInt;
        }
        
        #region 回填 LBO and LBS
        private List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrder> MapSOCode(List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrder> model, List<string> listNewSOCode, int salesOrderGroupID)
        {
            for (int i = 0; i < model.Count; i++)
            {
                model[i].Code = listNewSOCode[i];
                model[i].SalesOrderGroupID = salesOrderGroupID;
            }
            return model;
        }
        private List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderItem> MapSOItem(List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderItem> model, List<string> listNewSOCode, List<string> listNewSaleOrderCode)
        {

            List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderItem> result = new List<Models.DBModels.TWSQLDB.SalesOrderItem>();
            for (int i = 0; i < model.Count; i++)
            {
                model[i].Code = listNewSaleOrderCode[i];
                model[i].SalesorderCode = listNewSOCode[int.Parse(model[i].SalesorderCode)];
            }
            return model;
        }
        #endregion




        private List<DemainInsertSalesOrdersBySellerOutput> mapToSalesOrderBySellerOutput(List<SalesOrder> salesOrderList, List<SalesOrderItem> salesOrderItemTemp, int salesOrderGroupId)
        {
            List<DemainInsertSalesOrdersBySellerOutput> returnModel = new List<DemainInsertSalesOrdersBySellerOutput>();
            bool isSuccess = true;
            for (int i = 0; i < salesOrderItemTemp.Count; i++)
            {
                var salesOrderTemp = salesOrderList.Where(p => p.Code == salesOrderItemTemp[i].SalesorderCode).FirstOrDefault();
                if (salesOrderTemp == null)
                {
                    isSuccess = false;
                    break;
                }
                #region insert data to model
                DemainInsertSalesOrdersBySellerOutput _demainInsertSalesOrdersBySellerOutput = new DemainInsertSalesOrdersBySellerOutput();
                _demainInsertSalesOrdersBySellerOutput.salesorder_code = salesOrderItemTemp[i].SalesorderCode;
                _demainInsertSalesOrdersBySellerOutput.salesorder_salesordergroupid = salesOrderGroupId;
                _demainInsertSalesOrdersBySellerOutput.salesorder_idno = salesOrderTemp.IDNO;
                _demainInsertSalesOrdersBySellerOutput.salesorder_name = salesOrderTemp.Name;
                _demainInsertSalesOrdersBySellerOutput.salesorder_accountid = salesOrderTemp.AccountID;
                _demainInsertSalesOrdersBySellerOutput.salesorder_telday = salesOrderTemp.TelDay;
                _demainInsertSalesOrdersBySellerOutput.salesorder_telnight = salesOrderTemp.TelNight;
                _demainInsertSalesOrdersBySellerOutput.salesorder_mobile = salesOrderTemp.Mobile;
                _demainInsertSalesOrdersBySellerOutput.salesorder_email = salesOrderTemp.Email;
                _demainInsertSalesOrdersBySellerOutput.salesorder_paytypeid = salesOrderTemp.PayTypeID;
                _demainInsertSalesOrdersBySellerOutput.salesorder_paytype = salesOrderTemp.PayType;
                _demainInsertSalesOrdersBySellerOutput.salesorder_starvldate = salesOrderTemp.StarvlDate;
                _demainInsertSalesOrdersBySellerOutput.salesorder_cardholder = salesOrderTemp.CardHolder;
                _demainInsertSalesOrdersBySellerOutput.salesorder_cardtelday = salesOrderTemp.CardTelDay;
                _demainInsertSalesOrdersBySellerOutput.salesorder_cardtelnight = salesOrderTemp.CardTelNight;
                _demainInsertSalesOrdersBySellerOutput.salesorder_cardmobile = salesOrderTemp.CardMobile;
                _demainInsertSalesOrdersBySellerOutput.salesorder_cardloc = salesOrderTemp.CardLOC;
                _demainInsertSalesOrdersBySellerOutput.salesorder_cardzip = salesOrderTemp.CardZip;
                _demainInsertSalesOrdersBySellerOutput.salesorder_cardaddr = salesOrderTemp.CardADDR;
                _demainInsertSalesOrdersBySellerOutput.salesorder_cardno = salesOrderTemp.CardNo;
                _demainInsertSalesOrdersBySellerOutput.salesorder_cardnochk = salesOrderTemp.CardNochk;
                _demainInsertSalesOrdersBySellerOutput.salesorder_cardtype = salesOrderTemp.CardType;
                _demainInsertSalesOrdersBySellerOutput.salesorder_cardbank = salesOrderTemp.CardBank;
                _demainInsertSalesOrdersBySellerOutput.salesorder_cardexpire = salesOrderTemp.CardExpire;
                _demainInsertSalesOrdersBySellerOutput.salesorder_cardbirthday = salesOrderTemp.CardBirthday;
                _demainInsertSalesOrdersBySellerOutput.salesorder_invoreceiver = salesOrderTemp.RecvName;
                _demainInsertSalesOrdersBySellerOutput.salesorder_invoid = salesOrderTemp.InvoiceID;
                _demainInsertSalesOrdersBySellerOutput.salesorder_invotitle = salesOrderTemp.InvoiceTitle;
                _demainInsertSalesOrdersBySellerOutput.salesorder_involoc = salesOrderTemp.InvoiceLoc;
                _demainInsertSalesOrdersBySellerOutput.salesorder_invozip = salesOrderTemp.InvoiceZip;
                _demainInsertSalesOrdersBySellerOutput.salesorder_invoaddr = salesOrderTemp.InvoiceAddr;
                _demainInsertSalesOrdersBySellerOutput.salesorder_recvname = salesOrderTemp.RecvName;
                _demainInsertSalesOrdersBySellerOutput.salesorder_recvengname = salesOrderTemp.RecvEngName;
                _demainInsertSalesOrdersBySellerOutput.salesorder_recvtelday = salesOrderTemp.RecvTelDay;
                _demainInsertSalesOrdersBySellerOutput.salesorder_recvtelnight = salesOrderTemp.RecvTelNight;
                _demainInsertSalesOrdersBySellerOutput.salesorder_recvmobile = salesOrderTemp.RecvMobile;
                _demainInsertSalesOrdersBySellerOutput.salesorder_delivtype = salesOrderTemp.DelivType;
                _demainInsertSalesOrdersBySellerOutput.salesorder_delivdata = salesOrderTemp.DelivData;
                _demainInsertSalesOrdersBySellerOutput.salesorder_delivloc = salesOrderTemp.DelivLOC;
                _demainInsertSalesOrdersBySellerOutput.salesorder_delivzip = salesOrderTemp.DelivZip;
                _demainInsertSalesOrdersBySellerOutput.salesorder_delivaddr = salesOrderTemp.DelivADDR;
                _demainInsertSalesOrdersBySellerOutput.salesorder_delivengaddr = salesOrderTemp.DelivEngADDR;
                _demainInsertSalesOrdersBySellerOutput.salesorder_delivhitnote = salesOrderTemp.DelivHitNote;
                _demainInsertSalesOrdersBySellerOutput.salesorder_confirmdate = salesOrderTemp.ConfirmDate;
                _demainInsertSalesOrdersBySellerOutput.salesorder_confirmnote = salesOrderTemp.ConfirmNote;
                _demainInsertSalesOrdersBySellerOutput.salesorder_authdate = salesOrderTemp.AuthDate;
                _demainInsertSalesOrdersBySellerOutput.salesorder_authcode = salesOrderTemp.AuthCode;
                _demainInsertSalesOrdersBySellerOutput.salesorder_authnote = salesOrderTemp.AuthNote;
                _demainInsertSalesOrdersBySellerOutput.salesorder_hptype = salesOrderTemp.HpType;
                _demainInsertSalesOrdersBySellerOutput.salesorder_rcptdate = salesOrderTemp.RcptDate;
                _demainInsertSalesOrdersBySellerOutput.salesorder_rcptnote = salesOrderTemp.RcptNote;
                _demainInsertSalesOrdersBySellerOutput.salesorder_expire = salesOrderTemp.Expire;
                _demainInsertSalesOrdersBySellerOutput.salesorder_datedel = salesOrderTemp.DateDEL;
                _demainInsertSalesOrdersBySellerOutput.salesorder_coservername = salesOrderTemp.CoServerName;
                _demainInsertSalesOrdersBySellerOutput.salesorder_servername = salesOrderTemp.ServerName;
                _demainInsertSalesOrdersBySellerOutput.salesorder_actcode = salesOrderTemp.ActCode;
                _demainInsertSalesOrdersBySellerOutput.salesorder_status = Convert.ToByte(salesOrderTemp.Status);
                _demainInsertSalesOrdersBySellerOutput.salesorder_statusnote = salesOrderTemp.StatusNote;
                _demainInsertSalesOrdersBySellerOutput.salesorder_remoteip = salesOrderTemp.RemoteIP;
                _demainInsertSalesOrdersBySellerOutput.salesorder_date = salesOrderTemp.Date;
                _demainInsertSalesOrdersBySellerOutput.salesorder_note = salesOrderTemp.Note;
                _demainInsertSalesOrdersBySellerOutput.salesorder_note2 = salesOrderTemp.Note2;
                _demainInsertSalesOrdersBySellerOutput.salesorder_createuser = salesOrderTemp.CreateUser;
                _demainInsertSalesOrdersBySellerOutput.salesorder_createdate = salesOrderTemp.CreateDate;
                _demainInsertSalesOrdersBySellerOutput.salesorder_updated = salesOrderTemp.Updated;
                _demainInsertSalesOrdersBySellerOutput.salesorder_updateuser = salesOrderTemp.UpdateUser;
                _demainInsertSalesOrdersBySellerOutput.salesorder_updatedate = salesOrderTemp.UpdateDate;

                _demainInsertSalesOrdersBySellerOutput.salesorderitem_code = salesOrderItemTemp[i].Code;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_salesordercode = salesOrderItemTemp[i].SalesorderCode;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_itemid = salesOrderItemTemp[i].ItemID;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_itemlistid = salesOrderItemTemp[i].ItemlistID;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_productid = salesOrderItemTemp[i].ProductID;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_productlistid = salesOrderItemTemp[i].ProductlistID;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_name = salesOrderItemTemp[i].Name;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_price = salesOrderItemTemp[i].Price;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_displayprice = salesOrderItemTemp[i].DisplayPrice.Value;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_discountprice = salesOrderItemTemp[i].DiscountPrice.Value;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_shippingexpense = salesOrderItemTemp[i].ShippingExpense.Value;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_serviceexpense = salesOrderItemTemp[i].ServiceExpense.Value;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_tax = salesOrderItemTemp[i].Tax.Value;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_itempricesum = salesOrderItemTemp[i].ItemPriceSum.Value;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_installmentfee = salesOrderItemTemp[i].InstallmentFee;
                //insRateFees = salesOrderItemTemp[i].in
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_priceinst = salesOrderItemTemp[i].Priceinst;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_qty = salesOrderItemTemp[i].Qty;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_pricecoupon = salesOrderItemTemp[i].Pricecoupon;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_coupons = salesOrderItemTemp[i].Coupons;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_redmtkout = salesOrderItemTemp[i].RedmtkOut;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_redmbln = salesOrderItemTemp[i].Redmfdbck;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_redmfdbck = salesOrderItemTemp[i].Redmfdbck;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_status = salesOrderItemTemp[i].Status;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_statusnote = salesOrderItemTemp[i].StatusNote;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_date = salesOrderItemTemp[i].Date;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_attribs = salesOrderItemTemp[i].Attribs;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_note = salesOrderItemTemp[i].Note;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_wftkout = salesOrderItemTemp[i].WftkOut;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_wfbln = salesOrderItemTemp[i].WfBLN;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_adjprice = salesOrderItemTemp[i].AdjPrice;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_actid = salesOrderItemTemp[i].ActID;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_acttkout = salesOrderItemTemp[i].ActtkOut;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_isnew = salesOrderItemTemp[i].IsNew;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_apportionedamount = salesOrderItemTemp[i].ApportionedAmount;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_prodcutcostid = salesOrderItemTemp[i].ProdcutCostID;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_createuser = salesOrderItemTemp[i].CreateUser;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_createdate = salesOrderItemTemp[i].CreateDate;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_updated = salesOrderItemTemp[i].Updated;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_updatedate = salesOrderItemTemp[i].UpdateDate;
                _demainInsertSalesOrdersBySellerOutput.salesorderitem_updateuser = salesOrderItemTemp[i].UpdateUser;
                returnModel.Add(_demainInsertSalesOrdersBySellerOutput);
                #endregion
            }
            if (isSuccess == false)
            {
                returnModel = null;
            }
            return returnModel;
        }

        public void Dispose()
        {
            if (this.autofacScope != null)
            {
                this.autofacScope.Dispose();
            }
        }
    }
}
