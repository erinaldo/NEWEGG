USE [TWBACKENDDB]
GO
/****** Object:  View [dbo].[ViewProcessInfoView]    Script Date: 2016/08/18 13:04:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[ViewProcessInfoView]
AS
SELECT     
    dbo.purchaseorderitem.Code
   ,dbo.process.ID as ProductID
   ,TWSQLDB.dbo.product.SellerID
   ,dbo.process.Qty as ProcessQty
   ,dbo.process.Price
   ,dbo.purchaseorderitem.Qty as PurchaseOrderItemQty
   ,dbo.process.ID as ProcessID
   ,dbo.cart.ID as CartID
   ,dbo.cart.Shiptype
   ,dbo.purchaseorder.DelivType 
FROM         TWBACKENDDB.dbo.purchaseorderitem 
LEFT OUTER JOIN dbo.purchaseorder 
    ON TWBACKENDDB.dbo.purchaseorderitem.PurchaseOrderCode = TWBACKENDDB.dbo.Purchaseorder.Code 
LEFT OUTER JOIN TWBACKENDDB.dbo.cart 
    ON dbo.purchaseorder.SalesOrderCode = dbo.cart.ID 
LEFT OUTER JOIN TWBACKENDDB.dbo.process 
    ON TWBACKENDDB.dbo.cart.ID = TWBACKENDDB.dbo.process.CartID
    AND TWBACKENDDB.dbo.purchaseorderitem.ProductID = TWBACKENDDB.dbo.process.ProductID 
LEFT OUTER JOIN TWSQLDB.dbo.product 
    ON TWBACKENDDB.dbo.purchaseorderitem.ProductID = TWSQLDB.dbo.product.ID



GO
