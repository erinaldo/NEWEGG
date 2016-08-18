USE [TWBACKENDDB]
GO
/****** Object:  StoredProcedure [dbo].[UP_EC_GetSpexData]    Script Date: 2016/08/18 13:04:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UP_EC_GetSpexData]
	
	
AS
BEGIN
SELECT 
    DISTINCT POI.SellerOrderCode
   ,PO.SalesOrderCode
   ,PO.DelivType
   ,P.ID as ProductID
   ,P.Name as ProductName
   ,P.SellerProductID
   ,CT.receiver
   ,CT.RecvEngName
   ,CT.CardLocation+CT.CardAddr AS cart_locationAddr
   ,CT.DelivEngAddr
   ,CT.CardZipCode
   ,CT.CardLocation
   ,CT.Phone
   ,CT.CreateDate
,PO.ForwardNo
,POI.Qty
FROM TWBACKENDDB.dbo.purchaseorder PO WITH (NOLOCK) 
INNER JOIN TWBACKENDDB.dbo.purchaseorderitem POI WITH (NOLOCK) 
    ON PO.Code=POI.PurchaseOrderCode 
INNER JOIN TWBACKENDDB.dbo.cart CT WITH (NOLOCK) 
    ON PO.SalesOrderCode=CT.ID 
INNER JOIN TWSQLDB.dbo.product P WITH (NOLOCK) 
    ON P.ID=POI.ProductID WHERE (POI.SellerOrderCode !='' AND P.SellerProductID !='') AND (PO.DelivType=1 OR PO.DelivType=3)  
and (PO.ForwardNo is null or PO.ForwardNo ='') 
ORDER BY POI.SellerOrderCode

	
END



GO
