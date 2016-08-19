using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels;

namespace TWNewEgg.CartRepoAdapters
{
    public class DBSOInfoRepoAdapter : IDBSOInfoRepoAdapter
    {
        IBackendRepository<Cart> _Cart;
        IRepository<SalesOrder> _salesOrder;
        IBackendRepository<PurchaseOrder> _po;
        IBackendRepository<PurchaseOrderitemTWBACK> _poitem;
        IBackendRepository<Retgood> _retgood;
        IBackendRepository<refund2c> _refund2c;
        IRepository<SalesOrderItem> _soItem;

        public DBSOInfoRepoAdapter(IRepository<SalesOrder> salesOrder, IBackendRepository<PurchaseOrder> po, IBackendRepository<Cart> _Cart, IBackendRepository<PurchaseOrderitemTWBACK> _poitem, IBackendRepository<Retgood> _retgood, IBackendRepository<refund2c> _refund2c, IRepository<SalesOrderItem> _soItem)
        {   
            this._salesOrder = salesOrder;
            this._po = po;
            this._Cart = _Cart;
            this._retgood = _retgood;
            this._refund2c = _refund2c;
            this._poitem = _poitem;
            this._soItem=_soItem;
        }

        public DbSOInfo GetDBSOInfo(string SOCode)
        {
            DbSOInfo SOInfo = new DbSOInfo();
            var po = this._po.Get(x => x.SalesorderCode == SOCode);
            var CartDetail = this._Cart.Get(x => x.ID == SOCode);
            var so = this._salesOrder.Get(x => x.Code == SOCode);
            var Refund=this._refund2c.Get(x => x.CartID == SOCode);
            var Retgood= this._retgood.Get(x => x.CartID == SOCode);

            SOInfo.PO = po;
            SOInfo.Main = so;
            if (CartDetail != null)
            {
                if (so.Status == (int)SalesOrder.status.付款成功 || so.Status == (int)SalesOrder.status.未付款) 
                {
                    if (CartDetail.Status != (int)Cart.status.退貨 && CartDetail.Status != (int)Cart.status.取消)
                    {
                        switch (CartDetail.DelvStatus)
                        {
                            case (int)Cart.cartstatus.配達:
                                SOInfo.Status = DbSOInfo.SOInfoStatus.已送達;
                                break;
                            case (int)Cart.cartstatus.已出貨:
                                SOInfo.Status = DbSOInfo.SOInfoStatus.配送中;
                                break;
                            case (int)Cart.cartstatus.待出貨:
                                // SOInfo.Status = DBSOInfo.SOInfoStatus.待出貨; //這裡指tracking no 1押上了，還在美國本土
                                break;
                            case (int)Cart.cartstatus.已成立:
                                SOInfo.Status = DbSOInfo.SOInfoStatus.訂單成立; //初始
                                break;
                            case (int)Cart.cartstatus.空運中:
                                SOInfo.Status = DbSOInfo.SOInfoStatus.空運中;
                                break;
                        }
                        if (po != null)
                        {
                            var POItem=this._poitem.Get(x => x.PurchaseorderCode == po.Code);
                            if (!string.IsNullOrEmpty(po.ForwardNO))
                            {
                                SOInfo.Status = DbSOInfo.SOInfoStatus.空運中;
                            }
                            else if (!string.IsNullOrEmpty(POItem.SellerorderCode))
                            {
                                SOInfo.Status = DbSOInfo.SOInfoStatus.採購單確認中;
                            }
                            else if (!string.IsNullOrEmpty(po.DELIVNO))
                            {
                                // 回押tracking#1或為自營商品,訂單成立
                                SOInfo.Status = DbSOInfo.SOInfoStatus.訂單成立;
                                if (CartDetail.RefSONumber != 0)
                                {
                                    SOInfo.Status = DbSOInfo.SOInfoStatus.待出貨; //到WMS
                                }
                            }
                        }
                    }
                    else 
                    {
                        switch (CartDetail.Status)
                        {
                            case (int)Cart.status.取消:
                                    SOInfo.Status = DbSOInfo.SOInfoStatus.退款中;
                                break;
                            case (int)Cart.status.退貨:
                                if (Retgood != null && Retgood.Status == (int)Retgood.status.完成退貨)
                                {
                                    SOInfo.Status = DbSOInfo.SOInfoStatus.已退貨;
                                }
                                else if (Refund.Status == (int)refund2c.status.退貨異常) 
                                {
                                    SOInfo.Status = DbSOInfo.SOInfoStatus.已退貨;
                                }
                                else
                                {
                                    SOInfo.Status = DbSOInfo.SOInfoStatus.退貨中;
                                }
                                break;
                            case (int)Cart.status.被動取消:
                                    SOInfo.Status = DbSOInfo.SOInfoStatus.退款中;
                                break;
                        }
                    }
                }
            }
            else 
            {
                if (so.RecvName != null)
                {
                    switch (so.Status) 
                    {
                        case (int)SalesOrder.status.付款成功:
                            SOInfo.Status = DbSOInfo.SOInfoStatus.已付款;
                            break;
                        case (int)SalesOrder.status.付款失敗取消訂單:
                            SOInfo.Status = DbSOInfo.SOInfoStatus.失敗;
                            break;
                        case (int)SalesOrder.status.付款成功拋單失敗:
                            SOInfo.Status = DbSOInfo.SOInfoStatus.已付款;
                            break;
                        case (int)SalesOrder.status.中信分期處理中:
                        case (int)SalesOrder.status.聯邦分期處理中:
                        case (int)SalesOrder.status.聯信分期處理中:
                        case (int)SalesOrder.status.系統TWPAY處理中:
                        case (int)SalesOrder.status.台新分期處理中:
                        case (int)SalesOrder.status.聯信一次處理中:
                        case (int)SalesOrder.status.中信紅利處理中:
                        case (int)SalesOrder.status.聯邦紅利處理中:
                        case (int)SalesOrder.status.聯信紅利處理中:
                        case (int)SalesOrder.status.台新紅利處理中:
                        case (int)SalesOrder.status.超商付款處理中:
                        case (int)SalesOrder.status.歐付寶WebATM處理中:
                        case (int)SalesOrder.status.歐付寶儲值支付處理中:
                        case (int)SalesOrder.status.歐付寶線下ATM處理中:
                        case (int)SalesOrder.status.歐付寶分期處理中:
                        case (int)SalesOrder.status.NCCC處理中:
                            SOInfo.Status = DbSOInfo.SOInfoStatus.未付款; 
                            break;
                    }
                }
                else 
                {
                    SOInfo.Status = DbSOInfo.SOInfoStatus.初始;
                }
            }
            return SOInfo;
        }
    }
}
