USE [TWSQLDB]
GO
/****** Object:  StoredProcedure [dbo].[UP_EC_GetItemByQty]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO










CREATE PROCEDURE [dbo].[UP_EC_GetItemByQty] 
      @Page int = 1 
     ,@Categoryid int = 1 
     ,@BrandId int = 0 
     ,@OrderByType char(10) 
     ,@OrderBy char(4) 
AS
SET NOCOUNT ON
BEGIN

if(@OrderByType='Qty' and @BrandId=0)
begin

	SELECT TOP 
    (20)  D.ID 
   ,D.Name 
   ,D.Spechead 
   ,D.Specdetail 
   ,D.Photoname 
   ,D.PriceCash 
   ,D.ProductID 
   ,D.ProductFK 
   ,D.Rating 
   ,D.TotalReviews   
   ,D.SellingQty 
FROM ( 
        SELECT 
            A.ID AS ID
           ,A.Name AS Name
           ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
           ,A.Spechead AS Spechead 
           ,A.Specdetail AS SpecDetail 
           ,A.Photoname AS PhotoName 
           ,A.Pricecash AS PriceCash 
           ,P.ID AS ProductID 
           ,P.fk AS ProductFK
           ,PS.Rating AS Rating 
           ,PS.TotalReViews AS TotalReviews 
           ,row_number() OVER ( 
        ORDER BY dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit)  DESC) AS [row_number] 
        FROM dbo.item A WITH (NOLOCK) 
        INNER JOIN dbo.itemstock I WITH (NOLOCK) 
            ON A.ProductID=I.ProductID 
        INNER JOIN dbo.product P WITH (NOLOCK) 
            ON P.ID=I.ProductID 
        LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
            ON P.SellerProductID=PS.ItemNumber 
        WHERE 
            A.CategoryID=@Categoryid 
            AND A.[Status]=0  
            AND A.ShowOrder >= 0  
     )   AS D 
WHERE 
    D.[row_number] > (20*(@Page-1))     
ORDER BY D.SellingQty DESC
End

if(@OrderByType='Qty' and @BrandId>0)
begin

	SELECT TOP 
    (20)  
	D.ID 
   ,D.Name 
   ,D.Spechead 
   ,D.SpecDetail 
   ,D.Photoname 
   ,D.PriceCash 
   ,D.ProductID 
   ,D.ProductFK 
   ,D.Rating 
   ,D.TotalReviews   
   ,D.SellingQty 
    FROM ( 
            SELECT 
                A.ID AS ID 
               ,A.Name AS Name 
               ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
               ,A.Spechead AS Spechead 
               ,A.SpecDetail AS SpecDetail 
               ,A.PhotoName AS PhotoName 
               ,A.PriceCash AS PriceCash 
               ,P.ID AS ProductID 
               ,P.FK AS ProductFK 
               ,PS.Rating AS Rating 
               ,PS.TotalreViews AS TotalReviews 
               ,row_number() OVER ( 
            ORDER BY dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit)  DESC) AS [row_number] 
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.ProductID=I.ProductID 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.ID=I.ProductID 
                AND P.ManufactureID=@BrandId 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.SellerProductID=PS.ItemNumber 
            WHERE 
                A.CategoryID=@Categoryid 
                AND A.[Status]=0  
                AND A.ShowOrder >= 0  
         )   AS D 
    WHERE 
        D.[row_number] > (20*(@Page-1))      
    ORDER BY D.SellingQty DESC
END

if(@OrderByType='Hit' and @BrandId=0)
begin
	SELECT TOP 
    (20)  D.ID 
   ,D.Name 
   ,D.Spechead 
   ,D.Specdetail 
   ,D.Photoname 
   ,D.PriceCash 
   ,D.ProductID 
   ,D.ProductFK 
   ,D.Rating 
   ,D.TotalReviews   
   ,D.SellingQty 
    FROM ( 
            SELECT 
                A.ID AS ID 
               ,A.Name AS Name 
               ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
               ,A.Spechead AS Spechead 
               ,A.SpecDetail AS SpecDetail 
               ,A.PhotoName AS PhotoName 
               ,A.PriceCash AS PriceCash 
               ,P.ID AS ProductID 
               ,P.FK AS ProductFK 
               ,PS.Rating AS Rating 
               ,PS.TotalReviews AS TotalReviews 
               ,row_number() OVER ( 
            ORDER BY PS.TotalReviews DESC) AS [row_number] 
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.ProductID=I.ProductID 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.ID=I.ProductID 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.SellerProductID=PS.ItemNumber 
            WHERE 
                A.CategoryID=@Categoryid 
                AND A.[Status]=0  
                AND A.ShowOrder >= 0  
         )   AS D 
    WHERE 
        D.[row_number] > (20*(@Page-1))         
    ORDER BY D.TotalReviews DESC

END

if(@OrderByType='Hit' and @BrandId>0)
begin
	SELECT TOP 
    (20)  D.ID 
   ,D.Name 
   ,D.Spechead 
   ,D.Specdetail 
   ,D.Photoname 
   ,D.PriceCash 
   ,D.ProductID 
   ,D.ProductFK 
   ,D.Rating 
   ,D.TotalreViews   
   ,D.SellingQty 
    FROM ( 
            SELECT 
                A.ID AS ID 
               ,A.Name AS Name 
               ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
               ,A.Spechead AS Spechead 
               ,A.SpecDetail AS SpecDetail 
               ,A.Photoname AS PhotoName 
               ,A.PriceCash AS PriceCash 
               ,P.ID AS ProductID 
               ,P.FK AS ProductFK 
               ,PS.Rating AS Rating 
               ,PS.TotalReviews AS TotalReviews 
               ,row_number() OVER ( 
            ORDER BY PS.TotalReviews DESC) AS [row_number] 
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.ProductID=I.ProductID 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.ID=I.ProductID 
                AND P.ManufactureID=@BrandId 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.SellerProductID=PS.ItemNumber 
            WHERE 
                A.Categoryid=@Categoryid 
                AND A.[Status]=0  
                AND A.ShowOrder >= 0  
         )   AS D 
    WHERE 
        D.[row_number] > (20*(@Page-1))          
    ORDER BY D.TotalReviews DESC

END

if(@OrderByType='Price' and @OrderBy='DESC' and @BrandId=0 )
begin
	SELECT TOP 
    (20)  D.ID 
   ,D.Name 
   ,D.Spechead 
   ,D.Specdetail 
   ,D.Photoname 
   ,D.PriceCash 
   ,D.ProductID 
   ,D.ProductFK 
   ,D.Rating 
   ,D.TotalreViews   
   ,D.SellingQty 
    FROM ( 
            SELECT 
                A.ID AS ID 
               ,A.Name AS Name 
               ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
               ,A.Spechead AS Spechead 
               ,A.SpecDetail AS SpecDetail 
               ,A.PhotoName AS PhotoName 
               ,A.PriceCash AS PriceCash 
               ,P.ID AS ProductID 
               ,P.FK AS ProductFK 
               ,PS.Rating AS Rating 
               ,PS.TotalReviews AS TotalReviews 
               ,row_number() OVER ( 
            ORDER BY A.PriceCash DESC) AS [row_number] 
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.ProductID=I.ProductID 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.ID=I.ProductID 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.SellerProductid=PS.ItemNumber 
            WHERE 
                A.Categoryid=@Categoryid 
                AND A.[Status]=0  
                AND A.ShowOrder >= 0  
         )   AS D 
    WHERE 
        D.[row_number] > (20*(@Page-1))          
    ORDER BY D.PriceCash DESC

END

if(@OrderByType='Price' and @OrderBy='DESC' and @BrandId>0)
begin
	SELECT TOP 
    (20)  D.ID 
   ,D.Name 
   ,D.Spechead 
   ,D.Specdetail 
   ,D.Photoname 
   ,D.PriceCash 
   ,D.ProductID 
   ,D.ProductFK 
   ,D.Rating 
   ,D.TotalreViews   
   ,D.SellingQty 
    FROM ( 
            SELECT 
                A.ID AS ID 
               ,A.Name AS Name 
               ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
               ,A.Spechead AS Spechead 
               ,A.SpecDetail AS SpecDetail 
               ,A.PhotoName AS PhotoName 
               ,A.Pricecash AS PriceCash 
               ,P.ID AS ProductID 
               ,P.FK AS ProductFK 
               ,PS.Rating AS Rating 
               ,PS.TotalReviews AS TotalReviews 
               ,row_number() OVER ( 
            ORDER BY A.PriceCash DESC) AS [row_number] 
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.ProductID=I.ProductID 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.ID=I.ProductID 
                AND P.ManufactureID=@BrandId 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.SellerProductID=PS.ItemNumber 
            WHERE 
                A.CategoryID=@Categoryid 
                AND A.[Status]=0  
                AND A.ShowOrder >= 0  
         )   AS D 
    WHERE 
        D.[row_number] > (20*(@Page-1))          
    ORDER BY D.PriceCash DESC

END



if(@OrderByType='Price' and @OrderBy='ASC' and @BrandId=0 )
begin
	SELECT TOP 
    (20)  D.ID 
   ,D.Name 
   ,D.Spechead 
   ,D.Specdetail 
   ,D.Photoname 
   ,D.PriceCash 
   ,D.ProductID 
   ,D.ProductFK 
   ,D.Rating 
   ,D.TotalreViews   
   ,D.SellingQty 
    FROM ( 
            SELECT 
                A.ID AS ID 
               ,A.Name AS Name 
               ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
               ,A.Spechead AS Spechead 
               ,A.Specdetail AS Specdetail 
               ,A.Photoname AS Photoname 
               ,A.PriceCash AS PriceCash 
               ,P.ID AS ProductID 
               ,P.FK AS ProductFK 
               ,PS.Rating AS Rating 
               ,PS.TotalReviews AS TotalReviews 
               ,row_number() OVER ( 
            ORDER BY A.PriceCash ASC) AS [row_number] 
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.ProductID=I.ProductID 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.ID=I.ProductID 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.SellerProductID=PS.ItemNumber 
            WHERE 
                A.CategoryID=@Categoryid 
                AND A.[Status]=0  
                AND A.ShowOrder >= 0  
         )   AS D 
    WHERE 
        D.[row_number] > (20*(@Page-1))          
    ORDER BY D.PriceCash ASC

END

if(@OrderByType='Price' and @OrderBy='ASC' and @BrandId>0 )
begin
	SELECT TOP 
    (20)  D.ID 
   ,D.Name 
   ,D.Spechead 
   ,D.Specdetail 
   ,D.Photoname 
   ,D.PriceCash 
   ,D.ProductID 
   ,D.ProductFK 
   ,D.Rating 
   ,D.TotalreViews   
   ,D.SellingQty 
    FROM ( 
            SELECT 
                A.ID AS ID 
               ,A.Name AS Name 
               ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
               ,A.Spechead AS Spechead 
               ,A.SpecDetail AS Specdetail 
               ,A.PhotoName AS Photoname 
               ,A.PriceCash AS PriceCash 
               ,P.ID AS ProductID 
               ,P.FK AS ProductFK 
               ,PS.Rating AS Rating 
               ,PS.TotalReviews AS TotalReviews 
               ,row_number() OVER ( 
            ORDER BY A.PriceCash ASC) AS [row_number] 
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.ProductID=I.ProductID 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.ID=I.ProductID
                AND P.ManufactureID=@BrandId 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.SellerProductID=PS.ItemNumber 
            WHERE 
                A.CategoryID=@Categoryid 
                AND A.[Status]=0  
                AND A.ShowOrder >= 0  
         )   AS D 
    WHERE 
        D.[row_number] > (20*(@Page-1))          
    ORDER BY D.PriceCash ASC

END


if(@OrderByType='Review' and @BrandId=0 )
begin
	SELECT TOP 
    (20)  D.ID 
   ,D.Name 
   ,D.Spechead 
   ,D.Specdetail 
   ,D.Photoname 
   ,D.PriceCash 
   ,D.ProductID 
   ,D.ProductFK 
   ,D.Rating 
   ,D.TotalreViews   
   ,D.SellingQty 
    FROM ( 
            SELECT 
                A.ID AS ID 
               ,A.Name AS Name 
               ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
               ,A.Spechead AS Spechead 
               ,A.SpecDetail AS SpecDetail 
               ,A.PhotoName AS PhotoName 
               ,A.PriceCash AS PriceCash  
               ,P.ID AS ProductID 
               ,P.FK AS ProductFK 
               ,PS.Rating AS Rating 
               ,PS.TotalReviews AS TotalReviews 
               ,row_number() OVER ( 
            ORDER BY PS.Rating DESC) AS [row_number] 
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.ProductID=I.ProductID 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.ID=I.ProductID 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.SellerProductID=PS.ItemNumber 
            WHERE 
                A.CategoryID=@Categoryid 
                AND A.[Status]=0  
                AND A.ShowOrder >= 0  
         )   AS D 
    WHERE 
        D.[row_number] > (20*(@Page-1))        
    ORDER BY D.Rating DESC

END

if(@OrderByType='Review' and @BrandId>0 )
begin
	SELECT TOP 
    (20)  D.ID 
   ,D.Name 
   ,D.Spechead 
   ,D.Specdetail 
   ,D.Photoname 
   ,D.PriceCash 
   ,D.ProductID 
   ,D.ProductFK 
   ,D.Rating 
   ,D.TotalreViews   
   ,D.SellingQty 
    FROM ( 
            SELECT 
                A.ID AS ID 
               ,A.Name AS Name 
               ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
               ,A.Spechead AS Spechead 
               ,A.SpecDetail AS SpecDetail 
               ,A.PhotoName AS PhotoName 
               ,A.PriceCash AS PriceCash  
               ,P.ID AS ProductID 
               ,P.FK AS ProductFK 
               ,PS.Rating AS Rating 
               ,PS.TotalReviews AS TotalReviews 
               ,row_number() OVER ( 
            ORDER BY PS.Rating DESC) AS [row_number] 
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.ProductID=I.ProductID 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.ID=I.ProductID 
                AND P.ManufactureID=@BrandId 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.SellerProductID=PS.ItemNumber 
            WHERE 
                A.CategoryID=@Categoryid 
                AND A.[Status]=0  
                AND A.ShowOrder >= 0  
         )   AS D 
    WHERE 
        D.[row_number] > (20*(@Page-1))         
    ORDER BY D.Rating DESC

END



END








GO
