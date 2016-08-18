USE [TWSQLDB]
GO
/****** Object:  StoredProcedure [dbo].[UP_EC_GetSalesOrdersByAccID]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Aaron
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[UP_EC_GetSalesOrdersByAccID] 
       @accID int = NULL
      ,@nthRecord int = 1
      ,@span int = 999999 
AS
BEGIN
	
	SET NOCOUNT ON;
with codeOrder
as(
SELECT 
    Code
   ,dense_rank() OVER(
ORDER BY Code DESC)AS rowNumber 
FROM salesorder 
WHERE 
    AccountID=@accID

)

SELECT 
    O.*
   ,I.Code as SalesorderItem_Code
      ,I.SalesorderCode as SalesorderItem_SalesorderCode
      ,I.ItemID as SalesorderItem_ItemID
      ,I.ItemlistID as SalesorderItem_ItemlistID
      ,I.ProductID as SalesorderItem_ProductID
      ,I.ProductlistID as SalesorderItem_ProductlistID
      ,I.Name as SalesorderItem_Name
      ,I.Price as SalesorderItem_Price
      ,I.Priceinst as SalesorderItem_Priceinst
      ,I.Qty as SalesorderItem_Qty
      ,I.Pricecoupon as SalesorderItem_Pricecoupon
      ,I.RedmtkOut as SalesorderItem_RedmtkOut
      ,I.RedmBLN as SalesorderItem_RedmBLN
      ,I.Redmfdbck as SalesorderItem_Redmfdbck
      ,I.[Status] as SalesorderItem_Status
      ,I.StatusNote as SalesorderItem_StatusNote
      ,I.[Date] as SalesorderItem_Date
      ,I.Attribs as SalesorderItem_Attribs
      ,I.Note as SalesorderItem_Note
      ,I.WftkOut as SalesorderItem_WftkOut
      ,I.WfBLN as SalesorderItem_WfBLN
      ,I.AdjPrice as SalesorderItem_AdjPrice
      ,I.ActID as SalesorderItem_ActID
      ,I.ActtkOut as SalesorderItem_ActtkOut
      ,I.ProdcutCostID as SalesorderItem_ProdcutCostID
      ,I.CreateUser as SalesorderItem_CreateUser
      ,I.CreateDate as SalesorderItem_CreateDate
      ,I.Updated as SalesorderItem_Updated
      ,I.UpdateDate as SalesorderItem_UpdateDate
      ,I.UpdateUser as SalesorderItem_UpdateUser
      ,I.ShippingExpense as SalesorderItem_ShippingExpense
   ,E.[ID] as SalesorderItemExt_ID
      ,E.[SalesorderitemCode] as SalesorderItemExt_SalesorderitemCode
      ,E.[PsProductID] as SalesorderItemExt_PsProductID
      ,E.[PsmProductID] as SalesorderItemExt_PsmProductID
      ,E.[PsoriPrice] as SalesorderItemExt_PsoriPrice
      ,E.[PsSellcatID] as SalesorderItemExt_PsSellcatID
      ,E.[PsAttribName] as SalesorderItemExt_PsAttribName
      ,E.[PsModelNO] as SalesorderItemExt_PsModelNO
      ,E.[PsCost] as SalesorderItemExt_PsCost
      ,E.[Psfvf] as SalesorderItemExt_Psfvf
FROM salesorder O WITH (NOLOCK) 
INNER JOIN salesorderitem I WITH (NOLOCK) 
    ON O.Code=I.SalesorderCode 
INNER JOIN SalesorderItemExt E WITH (NOLOCK) 
    ON E.SalesorderItemCode=I.Code 
WHERE 
    O.Code IN ( 
            SELECT 
                Code 
            FROM codeOrder 
            WHERE 
                rowNumber BETWEEN @nthRecord 
                AND (@nthRecord+@span-1) 
        ) 
ORDER BY O.Code DESC,I.Code
END




GO
