USE [TWBACKENDDB]
GO
/****** Object:  View [dbo].[ViewPOItemDetail]    Script Date: 2016/08/18 13:04:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[ViewPOItemDetail]
AS
SELECT     
    dbo.purchaseorderitem.Code as PurchaseOrderItemCode
   ,TWSQLDB.dbo.Product.ID as ProductID
   ,TWSQLDB.dbo.product.SellerID
   ,dbo.process.Qty as ProcessQty
   ,dbo.process.Price as ProcessPrice
   ,dbo.purchaseorderitem.Qty as PurchaseOrderItemQty
   ,dbo.process.ID as ProcessID
   ,dbo.cart.ID as CartID
   ,dbo.cart.ShipType
   ,dbo.purchaseorder.DelivType 
FROM         TWBACKENDDB.dbo.purchaseorderitem with (nolock) 
LEFT OUTER JOIN TWBACKENDDB.dbo.purchaseorder with (nolock)  
    ON dbo.purchaseorderitem.PurchaseOrderCode = dbo.purchaseorder.Code 
LEFT OUTER JOIN TWBACKENDDB.dbo.cart with (nolock) 
    ON dbo.purchaseorder.SalesOrderCode = dbo.cart.ID 
LEFT OUTER JOIN TWBACKENDDB.dbo.process
    ON dbo.cart.ID = dbo.process.CartID 
    AND dbo.purchaseorderitem.ProductID = dbo.process.ProductID 
LEFT OUTER JOIN TWSQLDB.dbo.product 
    ON dbo.purchaseorderitem.ProductID = TWSQLDB.dbo.product.ID




GO
