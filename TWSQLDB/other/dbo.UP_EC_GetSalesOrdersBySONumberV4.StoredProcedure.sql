USE [TWSQLDB]
GO
/****** Object:  StoredProcedure [dbo].[UP_EC_GetSalesOrdersBySONumberV4]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UP_EC_GetSalesOrdersBySONumberV4]
       @Code varchar(15) 
AS
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
      ,I.ItemPriceSum as SalesorderItem_ItemPriceSum
      ,I.Priceinst as SalesorderItem_Priceinst
      ,I.Qty as SalesorderItem_Qty
      ,I.Pricecoupon as SalesorderItem_Pricecoupon
      ,I.Coupons as SalesorderItem_Coupons
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
      ,I.DisplayPrice as SalesorderItem_DisplayPrice
      ,I.DiscountPrice as SalesorderItem_DiscountPrice  
	  ,I.ShippingExpense as SalesorderItem_ShippingExpense 
	  ,I.ServiceExpense as SalesorderItem_Serviceexpense 
	  ,I.Tax as SalesorderItem_Tax 
	  ,I.InstallmentFee as SalesorderItem_InstallmentFee
      ,I.IsNew as SalesorderItem_IsNew
	  ,I.ApportionedAmount as SalesorderItem_ApportionedAmount
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
    ON O.Code=I.SalesOrderCode 
INNER JOIN salesorderitemext E WITH (NOLOCK) 
    ON E.SalesOrderItemCode=I.Code 
WHERE 
    O.SalesOrderGroupID IN (
            SELECT 
                SalesOrderGroupID 
            FROM salesorder O2 WITH (NOLOCK) 
            WHERE 
                O2.Code=@Code
        ) 
ORDER BY O.Code,I.Code







GO
