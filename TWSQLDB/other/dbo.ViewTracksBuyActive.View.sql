USE [TWSQLDB]
GO
/****** Object:  View [dbo].[ViewTracksBuyActive]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[ViewTracksBuyActive]
AS
SELECT     dbo.track.AccID, dbo.track.[Status], dbo.track.CreateDate, dbo.trackitem.[Status]  as TrackItemStatus, dbo.item.ID as ItemID,dbo.item.ProductID, dbo.item.Name as ItemName, dbo.item.PriceCash as ItemPrice, 
			dbo.item.PriceCard as ItemPriceCard,dbo.item.Layout, dbo.item.PhotoName, dbo.item.SaleType, dbo.item.PayType, dbo.item.DelvType, dbo.item.DateStart, 
                      dbo.item.DateEnd, dbo.item.DateDel,dbo.item.Coupon, dbo.item.Inst0rate, dbo.item.PriceCoupon as ItemPriceCoupon,dbo.item.SaleType AS ItemSaleType,  
                      dbo.item.Class,
					dbo.itemlist.ID, dbo.itemlist.ItemListID, dbo.itemlist.ItemListProductID, dbo.itemlist.Name as ItemListName, dbo.itemlist.Price as ItemListPrice, 
                       dbo.itemlist.ItemListOrder , 
                       dbo.itemlistgroup.ID as ItemListGroupID, dbo.itemlistgroup.Name as ItemListGroupName,dbo.itemlistgroup.[Type] as ItemListGroupType, dbo.itemlistgroup.ItemListGroupOrder, 
                      dbo.itemlistgroup.ItemListGroupRule,dbo.itemlistgroup.SelectedMin, dbo.itemlistgroup.SelectedMax,
                      dbo.fn_EC_GetSellingQty(dbo.item.Qty, dbo.item.Qtyreg, J.Qty,J.Qtyreg, dbo.item.QtyLimit) as ItemSellingQty,
                      K.SellerID as ItemSellerID,K.[Length] as ItemProductLength,K.[Width] as ItemProductWidth,K.Height as ItemProductHeight,K.[Weight] as ItemProductWeight,K.TradeTax as ItemTradeTax,
                      dbo.fn_EC_GetSellingQty(dbo.itemlist.Qty, dbo.itemlist.Qtyreg,  H.Qty,H.Qtyreg,dbo.itemlist.QtyLimit) as ItemListSellingQty,
                      L.SellerID as ItemListSellerID,L.[Length] as ItemListProductLength,L.[Width] as ItemListProductWidth,L.[Height] as ItemListProductHeight,L.[Weight] as ItemListProductWeight,L.TradeTax as ItemListTradeTax
FROM         dbo.track WITH (nolock) 
                      INNER JOIN dbo.item WITH (nolock) ON dbo.item.ID = dbo.track.ItemID
                      INNER JOIN dbo.itemstock J with (nolock) on J.ProductID=dbo.item.ProductID 
                      INNER JOIN dbo.product K with (nolock) on K.ID=J.ProductID
                      LEFT OUTER JOIN dbo.itemlistgroup WITH (nolock) ON dbo.itemlistgroup.ItemID = dbo.item.ID
                      LEFT OUTER JOIN dbo.itemlist WITH (nolock) ON dbo.itemlist.ItemID = dbo.item.ID and dbo.itemlist.ItemListGroupID=dbo.itemlistgroup.ID AND dbo.itemlist.[Status] = 0 
                      LEFT OUTER JOIN dbo.trackitem WITH (nolock) ON dbo.trackitem.TrackID = dbo.track.ID AND dbo.trackitem.ItemListID = dbo.itemlist.ID 
                      LEFT OUTER JOIN dbo.itemstock H with (nolock) on dbo.itemlist.ItemListProductID=H.ProductID
                      LEFT OUTER JOIN dbo.product L with (nolock) on L.ID=H.ProductID
WHERE     (DATEDIFF(d, dbo.track.CreateDate, GETDATE()) < 30) AND (dbo.item.[Status] in (0,60))




GO
