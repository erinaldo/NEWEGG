using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.GroupBuyServices.Interface;
using TWNewEgg.GroupBuy.Interface;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.GroupBuyRepoAdapters.Interface;
using TWNewEgg.Models.DomainModels.GroupBuy;
using TWNewEgg.ItemRepoAdapters.Interface;

namespace TWNewEgg.GroupBuyServices
{
    public class GroupBuyService : IGroupBuyService
    {
        IGetOldGroupBuyServices _getOldGroupBuyServices;
        IItemStockRepoAdapter _itemStockRepoAdapter;
        ICartRepoAdapter _cartRepoAdapter;
        IGroupBuyRepoAdapter _groupBuyRepoAdapter;

        public GroupBuyService(IGetOldGroupBuyServices getOldGroupBuyServices, IItemStockRepoAdapter itemStockRepoAdapter, ICartRepoAdapter cartRepoAdapter, IGroupBuyRepoAdapter groupBuyRepoAdapter)
        {
            this._getOldGroupBuyServices = getOldGroupBuyServices;
            this._itemStockRepoAdapter = itemStockRepoAdapter;
            this._cartRepoAdapter = cartRepoAdapter;
            this._groupBuyRepoAdapter = groupBuyRepoAdapter;
        }

        public List<GroupBuyViewInfo> QueryViewInfo(GroupBuyQueryCondition condition)
        {
            List<GroupBuyViewInfo> GroupBuyViewInfotemp = this._getOldGroupBuyServices.QueryViewInfo(condition);
            return GroupBuyViewInfotemp;
        }

        public Nullable<DateTime> GetEndDate(int GroupID, int ItemID)
        {
            Nullable<DateTime> EndDate = new Nullable<DateTime>();
            DateTime now = DateTime.UtcNow.AddHours(8);
            TWNewEgg.Models.DBModels.TWSQLDB.GroupBuy GroupBuytemp = this._groupBuyRepoAdapter.GetGroupBuyItem(GroupID, ItemID);
            TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty objItemSellingQty = null;

            if (GroupBuytemp == null) { return null; }

            //判斷時間
            if (now < GroupBuytemp.BeginDate || now > GroupBuytemp.EndDate)
            {
                return null;
            }
            //計算GroupBuy數量限制
            int Qty = this._cartRepoAdapter.GetGroupBuyNumber(GroupBuytemp.ItemID, GroupBuytemp.BeginDate, GroupBuytemp.EndDate);
            objItemSellingQty = this._itemStockRepoAdapter.GetItemSellingQtyByItemId(ItemID).FirstOrDefault();
            int StockQty = objItemSellingQty != null ? objItemSellingQty.SellingQty ?? 0 : 0;
            if ((GroupBuytemp.SalesOrderLimit > 0 && Qty >= GroupBuytemp.SalesOrderLimit) || StockQty == 0)
            {
                return null;
            }
            //判斷狀態
            if (CalculateStatus(GroupBuytemp, Qty) != GroupBuyStatus.StatusEnum.上檔中)
            {
                return null;
            }
            EndDate = GroupBuytemp.EndDate;
            return EndDate;
        }

        private GroupBuyStatus.StatusEnum CalculateStatus(TWNewEgg.Models.DBModels.TWSQLDB.GroupBuy groupBuy, int Qty)
        {
            GroupBuyStatus.StatusEnum result = GroupBuyStatus.StatusEnum.草稿;

            /*      IsWaitingForApprove IsReject    IsHide    IsApprove	

            草稿	    False			    False	    False     False		
            待審	    True			    False	    False     False		
            退回	    False			    True	    False     False		
            待上  	False			    False	    False     True		
            上檔	    False			    False	    False     True		
            售完	    False			    False	    False     True		
            下檔	    False			    False	    False     True		
            移除	    False			    False	    True      True		
             */

            if (groupBuy.IsWaitingForApprove)
            {
                result = GroupBuyStatus.StatusEnum.待審核;
            }
            else
            {
                if (groupBuy.IsReject)
                {
                    result = GroupBuyStatus.StatusEnum.退回;
                }
                else
                {
                    if (groupBuy.IsHide)
                    {
                        result = GroupBuyStatus.StatusEnum.已移除;
                    }
                    else
                    {
                        if (groupBuy.IsApprove)
                        {
                            var now = DateTime.UtcNow.AddHours(8);
                            if (now < groupBuy.BeginDate)
                            {
                                result = GroupBuyStatus.StatusEnum.待上檔;
                            }
                            else
                            {
                                if (now <= groupBuy.EndDate)
                                {
                                    int salesOrderRealCount = Qty;
                                    if (groupBuy.SalesOrderLimit > 0 && salesOrderRealCount >= groupBuy.SalesOrderLimit)
                                    {
                                        result = GroupBuyStatus.StatusEnum.已售完;
                                    }
                                    else
                                    {
                                        result = GroupBuyStatus.StatusEnum.上檔中;
                                    }
                                }
                                else
                                {
                                    result = GroupBuyStatus.StatusEnum.已下檔;
                                }
                            }
                        }
                        else
                        {
                            result = GroupBuyStatus.StatusEnum.草稿;
                        }
                    }
                }
            }

            return result;
        }
    }
}
