USE [TWSQLDB]
GO
/****** Object:  StoredProcedure [dbo].[UP_EC_GetItemByQtyV3]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE PROCEDURE [dbo].[UP_EC_GetItemByQtyV3] 
      @Page int = 1 
     ,@Categoryid int = 1 
     ,@BrandId int = 0 
     ,@OrderByType char(10) 
     ,@OrderBy char(4) 
	 ,@CountryID int = 0
AS
SET NOCOUNT ON
BEGIN


/********/
DECLARE @ShowNumber int = 20


DECLARE @brandTb table
(
brandID int
)
DECLARE @categoryTb table
(
categoryID int
)
DECLARE @showZeroFalg int = -1,
		@showPriceFalg decimal(12,2) = 0.00


/********/

insert into @brandTb
select @BrandId


insert into @categoryTb
select @Categoryid

/********/



if(@OrderByType = 'Qty' and @BrandId = 0)
begin
	SELECT TOP 
    (@ShowNumber)  D.ID 
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
   ,D.CreateDate
FROM ( 
	SELECT *,row_number() OVER (ORDER BY D.SellingQty DESC) AS [row_number] /****ORDER CONDITION AT HERE****/
	FROM (    /**************************************/
        SELECT 
            A.ID AS ID
           ,A.Name AS Name
           ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
           ,A.Spechead AS Spechead 
           ,A.Specdetail AS SpecDetail 
           ,A.Photoname AS PhotoName 
           ,CAST(CASE WHEN IDP.DisplayPrice > 0 THEN IDP.DisplayPrice ELSE A.PriceCash END AS Decimal(12, 2)) AS PriceCash 
		   ,A.CreateDate AS CreateDate
           ,P.ID AS ProductID 
           ,P.fk AS ProductFK
           ,PS.Rating AS Rating 
           ,PS.TotalReViews AS TotalReviews 
           /*,row_number() OVER ( 
        ORDER BY dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit)  DESC) AS [row_number] */
        FROM dbo.item A WITH (NOLOCK) 
        INNER JOIN dbo.itemstock I WITH (NOLOCK) 
            ON A.ProductID=I.ProductID 
        INNER JOIN dbo.product P WITH (NOLOCK) 
            ON P.ID=I.ProductID 
        LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
            ON P.SellerProductID=PS.ItemNumber 
        LEFT OUTER JOIN dbo.itemdisplayprice AS IDP WITH (nolock) 
            ON A.ID = IDP.ItemID
        WHERE 
            /*A.CategoryID=@Categoryid */
			A.CategoryID IN (SELECT * FROM @categoryTb)
            AND A.[Status]=0  
            AND A.ShowOrder >= 0  
            AND (IDP.PriceType = 1 or IDP.PriceType is null)
            AND (IDP.MinNumber = 1 or IDP.MinNumber is null)
            AND (IDP.MaxNumber = 1 or IDP.MaxNumber is null)
     )   AS D 
	WHERE    /****WHERE CONDITION AT HERE****/
         D.SellingQty > @showZeroFalg 
         AND D.PriceCash > @showPriceFalg
	) AS D /**************************************/
WHERE 
    D.[row_number] > (@ShowNumber*(@Page-1)) 
ORDER BY D.SellingQty DESC
End

if(@OrderByType = 'Qty' and @BrandId > 0)
begin

	SELECT TOP 
    (@ShowNumber)  
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
   ,D.CreateDate
    FROM ( 
	SELECT *,row_number() OVER (ORDER BY D.SellingQty DESC) AS [row_number] /****ORDER CONDITION AT HERE****/
	FROM (    /**************************************/
            SELECT 
                A.ID AS ID 
               ,A.Name AS Name 
               ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
               ,A.Spechead AS Spechead 
               ,A.SpecDetail AS SpecDetail 
               ,A.PhotoName AS PhotoName 
               ,CAST(CASE WHEN IDP.DisplayPrice > 0 THEN IDP.DisplayPrice ELSE A.PriceCash END AS Decimal(12, 2)) AS PriceCash 
			   ,A.CreateDate AS CreateDate
               ,P.ID AS ProductID 
               ,P.FK AS ProductFK 
               ,PS.Rating AS Rating 
               ,PS.TotalreViews AS TotalReviews 
            /*   ,row_number() OVER ( 
            ORDER BY dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit)  DESC) AS [row_number] */
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.ProductID=I.ProductID 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.ID=I.ProductID 
                AND P.ManufactureID IN (SELECT * FROM @brandTb)
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.SellerProductID=PS.ItemNumber 
            LEFT OUTER JOIN dbo.itemdisplayprice AS IDP WITH (nolock) 
                ON A.ID = IDP.ItemID
            WHERE 
                A.CategoryID IN (SELECT * FROM @categoryTb)
                AND A.[Status]=0  
                AND A.ShowOrder >= 0  
                AND (IDP.PriceType = 1 or IDP.PriceType is null)
                AND (IDP.MinNumber = 1 or IDP.MinNumber is null)
                AND (IDP.MaxNumber = 1 or IDP.MaxNumber is null)
         )   AS D 
	WHERE    /****WHERE CONDITION AT HERE****/
         D.SellingQty > @showZeroFalg 
         AND D.PriceCash > @showPriceFalg
	) AS D /**************************************/
    WHERE 
        D.[row_number] > (@ShowNumber*(@Page-1))
    ORDER BY D.SellingQty DESC
END

if(@OrderByType='Hit' and @BrandId = 0)
begin
	SELECT TOP 
    (@ShowNumber)  D.ID 
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
   ,D.CreateDate
    FROM ( 
	SELECT *,row_number() OVER (ORDER BY D.TotalReviews DESC) AS [row_number] /****ORDER CONDITION AT HERE****/
	FROM (    /**************************************/
            SELECT 
                A.ID AS ID 
               ,A.Name AS Name 
               ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
               ,A.Spechead AS Spechead 
               ,A.SpecDetail AS SpecDetail 
               ,A.PhotoName AS PhotoName 
               ,CAST(CASE WHEN IDP.DisplayPrice > 0 THEN IDP.DisplayPrice ELSE A.PriceCash END AS Decimal(12, 2)) AS PriceCash 
			   ,A.CreateDate AS CreateDate
               ,P.ID AS ProductID 
               ,P.FK AS ProductFK 
               ,PS.Rating AS Rating 
               ,PS.TotalReviews AS TotalReviews 
            /*   ,row_number() OVER ( 
            ORDER BY PS.TotalReviews DESC) AS [row_number] */
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.ProductID=I.ProductID 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.ID=I.ProductID 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.SellerProductID=PS.ItemNumber 
            LEFT OUTER JOIN dbo.itemdisplayprice AS IDP WITH (nolock) 
                ON A.ID = IDP.ItemID
            WHERE 
                A.CategoryID IN (SELECT * FROM @categoryTb)
                AND A.[Status]=0  
                AND A.ShowOrder >= 0  
                AND (IDP.PriceType = 1 or IDP.PriceType is null)
                AND (IDP.MinNumber = 1 or IDP.MinNumber is null)
                AND (IDP.MaxNumber = 1 or IDP.MaxNumber is null)
         )   AS D 
	WHERE    /****WHERE CONDITION AT HERE****/
         D.SellingQty > @showZeroFalg 
         AND D.PriceCash > @showPriceFalg
	) AS D /**************************************/
    WHERE 
        D.[row_number] > (@ShowNumber*(@Page-1))
    ORDER BY D.TotalReviews DESC

END

if(@OrderByType='Hit' and @BrandId > 0)
begin
	SELECT TOP 
    (@ShowNumber)  D.ID 
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
   ,D.CreateDate
    FROM ( 
	SELECT *,row_number() OVER (ORDER BY D.TotalReviews DESC) AS [row_number] /****ORDER CONDITION AT HERE****/
	FROM (    /**************************************/
            SELECT 
                A.ID AS ID 
               ,A.Name AS Name 
               ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
               ,A.Spechead AS Spechead 
               ,A.SpecDetail AS SpecDetail 
               ,A.Photoname AS PhotoName 
               ,CAST(CASE WHEN IDP.DisplayPrice > 0 THEN IDP.DisplayPrice ELSE A.PriceCash END AS Decimal(12, 2)) AS PriceCash 
			   ,A.CreateDate AS CreateDate
               ,P.ID AS ProductID 
               ,P.FK AS ProductFK 
               ,PS.Rating AS Rating 
               ,PS.TotalReviews AS TotalReviews 
            /*   ,row_number() OVER ( 
            ORDER BY PS.TotalReviews DESC) AS [row_number] */
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.ProductID=I.ProductID 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.ID=I.ProductID 
                AND P.ManufactureID IN (SELECT * FROM @brandTb)
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.SellerProductID=PS.ItemNumber 
            LEFT OUTER JOIN dbo.itemdisplayprice AS IDP WITH (nolock) 
                ON A.ID = IDP.ItemID
            WHERE 
                A.Categoryid IN (SELECT * FROM @categoryTb)
                AND A.[Status]=0  
                AND A.ShowOrder >= 0  
                AND (IDP.PriceType = 1 or IDP.PriceType is null)
                AND (IDP.MinNumber = 1 or IDP.MinNumber is null)
                AND (IDP.MaxNumber = 1 or IDP.MaxNumber is null)
         )   AS D 
	WHERE    /****WHERE CONDITION AT HERE****/
         D.SellingQty > @showZeroFalg 
         AND D.PriceCash > @showPriceFalg
	) AS D /**************************************/
    WHERE 
        D.[row_number] > (@ShowNumber*(@Page-1))
    ORDER BY D.TotalReviews DESC

END

if(@OrderByType='Price' and @OrderBy='DESC' and @BrandId = 0)
begin
	SELECT TOP 
    (@ShowNumber)  D.ID 
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
   ,D.CreateDate
    FROM ( 
	SELECT *,row_number() OVER (ORDER BY D.PriceCash DESC) AS [row_number] /****ORDER CONDITION AT HERE****/
	FROM (    /**************************************/
            SELECT 
                A.ID AS ID 
               ,A.Name AS Name 
               ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
               ,A.Spechead AS Spechead 
               ,A.SpecDetail AS SpecDetail 
               ,A.PhotoName AS PhotoName 
               ,CAST(CASE WHEN IDP.DisplayPrice > 0 THEN IDP.DisplayPrice ELSE A.PriceCash END AS Decimal(12, 2)) AS PriceCash 
			   ,A.CreateDate AS CreateDate
               ,P.ID AS ProductID 
               ,P.FK AS ProductFK 
               ,PS.Rating AS Rating 
               ,PS.TotalReviews AS TotalReviews 
            /*   ,row_number() OVER ( 
            ORDER BY A.PriceCash DESC) AS [row_number] */
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.ProductID=I.ProductID 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.ID=I.ProductID 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.SellerProductid=PS.ItemNumber 
            LEFT OUTER JOIN dbo.itemdisplayprice AS IDP WITH (nolock) 
                ON A.ID = IDP.ItemID
            WHERE 
                A.Categoryid IN (SELECT * FROM @categoryTb) 
                AND A.[Status]=0  
                AND A.ShowOrder >= 0  
                AND (IDP.PriceType = 1 or IDP.PriceType is null)
                AND (IDP.MinNumber = 1 or IDP.MinNumber is null)
                AND (IDP.MaxNumber = 1 or IDP.MaxNumber is null)
         )   AS D 
	WHERE    /****WHERE CONDITION AT HERE****/
         D.SellingQty > @showZeroFalg 
         AND D.PriceCash > @showPriceFalg
	) AS D /**************************************/
    WHERE 
        D.[row_number] > (@ShowNumber*(@Page-1))
    ORDER BY D.PriceCash DESC

END

if(@OrderByType='Price' and @OrderBy='DESC' and @BrandId > 0)
begin
	SELECT TOP 
    (@ShowNumber)  D.ID 
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
   ,D.CreateDate
    FROM ( 
	SELECT *,row_number() OVER (ORDER BY D.PriceCash DESC) AS [row_number] /****ORDER CONDITION AT HERE****/
	FROM (    /**************************************/
            SELECT 
                A.ID AS ID 
               ,A.Name AS Name 
               ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
               ,A.Spechead AS Spechead 
               ,A.SpecDetail AS SpecDetail 
               ,A.PhotoName AS PhotoName 
               ,CAST(CASE WHEN IDP.DisplayPrice > 0 THEN IDP.DisplayPrice ELSE A.PriceCash END AS Decimal(12, 2)) AS PriceCash 
			   ,A.CreateDate AS CreateDate
               ,P.ID AS ProductID 
               ,P.FK AS ProductFK 
               ,PS.Rating AS Rating 
               ,PS.TotalReviews AS TotalReviews 
            /*   ,row_number() OVER ( 
            ORDER BY A.PriceCash DESC) AS [row_number] */
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.ProductID=I.ProductID 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.ID=I.ProductID 
                AND P.ManufactureID IN (SELECT * FROM @brandTb) 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.SellerProductID=PS.ItemNumber 
            LEFT OUTER JOIN dbo.itemdisplayprice AS IDP WITH (nolock) 
                ON A.ID = IDP.ItemID
            WHERE 
                A.CategoryID IN (SELECT * FROM @categoryTb) 
                AND A.[Status]=0  
                AND A.ShowOrder >= 0  
                AND (IDP.PriceType = 1 or IDP.PriceType is null)
                AND (IDP.MinNumber = 1 or IDP.MinNumber is null)
                AND (IDP.MaxNumber = 1 or IDP.MaxNumber is null)
         )   AS D 
	WHERE    /****WHERE CONDITION AT HERE****/
         D.SellingQty > @showZeroFalg 
         AND D.PriceCash > @showPriceFalg
	) AS D /**************************************/
    WHERE 
        D.[row_number] > (@ShowNumber*(@Page-1))
    ORDER BY D.PriceCash DESC

END



if(@OrderByType='Price' and @OrderBy='ASC' and @BrandId = 0)
begin
	SELECT TOP 
    (@ShowNumber)  D.ID 
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
   ,D.CreateDate
    FROM ( 
	SELECT *,row_number() OVER (ORDER BY D.PriceCash ASC) AS [row_number] /****ORDER CONDITION AT HERE****/
	FROM (    /**************************************/
            SELECT 
                A.ID AS ID 
               ,A.Name AS Name 
               ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
               ,A.Spechead AS Spechead 
               ,A.Specdetail AS Specdetail 
               ,A.Photoname AS Photoname 
               ,CAST(CASE WHEN IDP.DisplayPrice > 0 THEN IDP.DisplayPrice ELSE A.PriceCash END AS Decimal(12, 2)) AS PriceCash 
			   ,A.CreateDate AS CreateDate
               ,P.ID AS ProductID 
               ,P.FK AS ProductFK 
               ,PS.Rating AS Rating 
               ,PS.TotalReviews AS TotalReviews 
            /*   ,row_number() OVER ( 
            ORDER BY A.PriceCash ASC) AS [row_number] */
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.ProductID=I.ProductID 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.ID=I.ProductID 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.SellerProductID=PS.ItemNumber 
            LEFT OUTER JOIN dbo.itemdisplayprice AS IDP WITH (nolock) 
                ON A.ID = IDP.ItemID
            WHERE 
                A.CategoryID IN (SELECT * FROM @categoryTb) 
                AND A.[Status]=0  
                AND A.ShowOrder >= 0  
                AND (IDP.PriceType = 1 or IDP.PriceType is null)
                AND (IDP.MinNumber = 1 or IDP.MinNumber is null)
                AND (IDP.MaxNumber = 1 or IDP.MaxNumber is null)
         )   AS D 
	WHERE    /****WHERE CONDITION AT HERE****/
         D.SellingQty > @showZeroFalg 
         AND D.PriceCash > @showPriceFalg
	) AS D /**************************************/
    WHERE 
        D.[row_number] > (@ShowNumber*(@Page-1))
    ORDER BY D.PriceCash ASC

END

if(@OrderByType='Price' and @OrderBy='ASC' and @BrandId > 0)
begin
	SELECT TOP 
    (@ShowNumber)  D.ID 
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
   ,D.CreateDate
    FROM ( 
	SELECT *,row_number() OVER (ORDER BY D.PriceCash ASC) AS [row_number] /****ORDER CONDITION AT HERE****/
	FROM (    /**************************************/
            SELECT 
                A.ID AS ID 
               ,A.Name AS Name 
               ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
               ,A.Spechead AS Spechead 
               ,A.SpecDetail AS Specdetail 
               ,A.PhotoName AS Photoname 
               ,CAST(CASE WHEN IDP.DisplayPrice > 0 THEN IDP.DisplayPrice ELSE A.PriceCash END AS Decimal(12, 2)) AS PriceCash 
			   ,A.CreateDate AS CreateDate
               ,P.ID AS ProductID 
               ,P.FK AS ProductFK 
               ,PS.Rating AS Rating 
               ,PS.TotalReviews AS TotalReviews 
            /*   ,row_number() OVER ( 
            ORDER BY A.PriceCash ASC) AS [row_number] */
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.ProductID=I.ProductID 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.ID=I.ProductID
                AND P.ManufactureID IN (SELECT * FROM @brandTb) 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.SellerProductID=PS.ItemNumber 
            LEFT OUTER JOIN dbo.itemdisplayprice AS IDP WITH (nolock) 
                ON A.ID = IDP.ItemID
            WHERE 
                A.CategoryID IN (SELECT * FROM @categoryTb) 
                AND A.[Status]=0  
                AND A.ShowOrder >= 0  
                AND (IDP.PriceType = 1 or IDP.PriceType is null)
                AND (IDP.MinNumber = 1 or IDP.MinNumber is null)
                AND (IDP.MaxNumber = 1 or IDP.MaxNumber is null)
         )   AS D 
	WHERE    /****WHERE CONDITION AT HERE****/
         D.SellingQty > @showZeroFalg 
         AND D.PriceCash > @showPriceFalg
	) AS D /**************************************/
    WHERE 
        D.[row_number] > (@ShowNumber*(@Page-1))
    ORDER BY D.PriceCash ASC

END

if(@OrderByType='CreateDate' and @OrderBy='DESC' and @BrandId = 0)
begin
	SELECT TOP 
    (@ShowNumber)  D.ID 
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
   ,D.CreateDate
    FROM ( 
	SELECT *,row_number() OVER (ORDER BY D.CreateDate DESC) AS [row_number] /****ORDER CONDITION AT HERE****/
	FROM (    /**************************************/
            SELECT 
                A.ID AS ID 
               ,A.Name AS Name 
               ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
               ,A.Spechead AS Spechead 
               ,A.SpecDetail AS SpecDetail 
               ,A.PhotoName AS PhotoName 
               ,CAST(CASE WHEN IDP.DisplayPrice > 0 THEN IDP.DisplayPrice ELSE A.PriceCash END AS Decimal(12, 2)) AS PriceCash 
               ,A.CreateDate as CreateDate
               ,P.ID AS ProductID 
               ,P.FK AS ProductFK 
               ,PS.Rating AS Rating 
               ,PS.TotalReviews AS TotalReviews 
            /*   ,row_number() OVER ( 
            ORDER BY A.CreateDate DESC) AS [row_number] */
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.ProductID=I.ProductID 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.ID=I.ProductID 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.SellerProductid=PS.ItemNumber 
            LEFT OUTER JOIN dbo.itemdisplayprice AS IDP WITH (nolock) 
                ON A.ID = IDP.ItemID
            WHERE 
                A.Categoryid IN (SELECT * FROM @categoryTb) 
                AND A.[Status]=0  
                AND A.ShowOrder >= 0  
                AND (IDP.PriceType = 1 or IDP.PriceType is null)
                AND (IDP.MinNumber = 1 or IDP.MinNumber is null)
                AND (IDP.MaxNumber = 1 or IDP.MaxNumber is null)
         )   AS D 
	WHERE    /****WHERE CONDITION AT HERE****/
         D.SellingQty > @showZeroFalg 
         AND D.PriceCash > @showPriceFalg
	) AS D /**************************************/
    WHERE 
        D.[row_number] > (@ShowNumber*(@Page-1))
    ORDER BY D.CreateDate DESC

END

if(@OrderByType='CreateDate' and @OrderBy='DESC' and @BrandId > 0)
begin
	SELECT TOP 
    (@ShowNumber)  D.ID 
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
   ,D.CreateDate
    FROM ( 
	SELECT *,row_number() OVER (ORDER BY D.CreateDate DESC) AS [row_number] /****ORDER CONDITION AT HERE****/
	FROM (    /**************************************/
            SELECT 
                A.ID AS ID 
               ,A.Name AS Name 
               ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
               ,A.Spechead AS Spechead 
               ,A.SpecDetail AS SpecDetail 
               ,A.PhotoName AS PhotoName 
               ,CAST(CASE WHEN IDP.DisplayPrice > 0 THEN IDP.DisplayPrice ELSE A.PriceCash END AS Decimal(12, 2)) AS PriceCash 
			   ,A.CreateDate AS CreateDate
               ,P.ID AS ProductID 
               ,P.FK AS ProductFK 
               ,PS.Rating AS Rating 
               ,PS.TotalReviews AS TotalReviews 
            /*   ,row_number() OVER ( 
            ORDER BY A.CreateDate DESC) AS [row_number] */
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.ProductID=I.ProductID 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.ID=I.ProductID 
                AND P.ManufactureID IN (SELECT * FROM @brandTb) 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.SellerProductID=PS.ItemNumber 
            LEFT OUTER JOIN dbo.itemdisplayprice AS IDP WITH (nolock) 
                ON A.ID = IDP.ItemID
            WHERE 
                A.CategoryID IN (SELECT * FROM @categoryTb) 
                AND A.[Status]=0  
                AND A.ShowOrder >= 0 
                AND (IDP.PriceType = 1 or IDP.PriceType is null)
                AND (IDP.MinNumber = 1 or IDP.MinNumber is null)
                AND (IDP.MaxNumber = 1 or IDP.MaxNumber is null) 
         )   AS D 
	WHERE    /****WHERE CONDITION AT HERE****/
         D.SellingQty > @showZeroFalg 
         AND D.PriceCash > @showPriceFalg
	) AS D /**************************************/
    WHERE 
        D.[row_number] > (@ShowNumber*(@Page-1))
    ORDER BY D.CreateDate DESC

END



if(@OrderByType='CreateDate' and @OrderBy='ASC' and @BrandId = 0)
begin
	SELECT TOP 
    (@ShowNumber)  D.ID 
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
   ,D.CreateDate
    FROM ( 
	SELECT *,row_number() OVER (ORDER BY D.CreateDate ASC) AS [row_number] /****ORDER CONDITION AT HERE****/
	FROM (    /**************************************/
            SELECT 
                A.ID AS ID 
               ,A.Name AS Name 
               ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
               ,A.Spechead AS Spechead 
               ,A.Specdetail AS Specdetail 
               ,A.Photoname AS Photoname 
               ,CAST(CASE WHEN IDP.DisplayPrice > 0 THEN IDP.DisplayPrice ELSE A.PriceCash END AS Decimal(12, 2)) AS PriceCash 
			   ,A.CreateDate AS CreateDate
               ,P.ID AS ProductID 
               ,P.FK AS ProductFK 
               ,PS.Rating AS Rating 
               ,PS.TotalReviews AS TotalReviews 
            /*   ,row_number() OVER ( 
            ORDER BY A.CreateDate ASC) AS [row_number] */
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.ProductID=I.ProductID 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.ID=I.ProductID 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.SellerProductID=PS.ItemNumber 
            LEFT OUTER JOIN dbo.itemdisplayprice AS IDP WITH (nolock) 
                ON A.ID = IDP.ItemID
            WHERE 
                A.CategoryID IN (SELECT * FROM @categoryTb) 
                AND A.[Status]=0  
                AND A.ShowOrder >= 0  
                AND (IDP.PriceType = 1 or IDP.PriceType is null)
                AND (IDP.MinNumber = 1 or IDP.MinNumber is null)
                AND (IDP.MaxNumber = 1 or IDP.MaxNumber is null)
         )   AS D 
	WHERE    /****WHERE CONDITION AT HERE****/
         D.SellingQty > @showZeroFalg 
         AND D.PriceCash > @showPriceFalg
	) AS D /**************************************/
    WHERE 
        D.[row_number] > (@ShowNumber*(@Page-1))
    ORDER BY D.CreateDate ASC

END

if(@OrderByType='CreateDate' and @OrderBy='ASC' and @BrandId > 0)
begin
	SELECT TOP 
    (@ShowNumber)  D.ID 
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
   ,D.CreateDate
    FROM ( 
	SELECT *,row_number() OVER (ORDER BY D.CreateDate ASC) AS [row_number] /****ORDER CONDITION AT HERE****/
	FROM (    /**************************************/
            SELECT 
                A.ID AS ID 
               ,A.Name AS Name 
               ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
               ,A.Spechead AS Spechead 
               ,A.SpecDetail AS Specdetail 
               ,A.PhotoName AS Photoname 
               ,CAST(CASE WHEN IDP.DisplayPrice > 0 THEN IDP.DisplayPrice ELSE A.PriceCash END AS Decimal(12, 2)) AS PriceCash 
			   ,A.CreateDate AS CreateDate
               ,P.ID AS ProductID 
               ,P.FK AS ProductFK 
               ,PS.Rating AS Rating 
               ,PS.TotalReviews AS TotalReviews 
            /*   ,row_number() OVER ( 
            ORDER BY A.CreateDate ASC) AS [row_number] */
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.ProductID=I.ProductID 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.ID=I.ProductID
                AND P.ManufactureID IN (SELECT * FROM @brandTb) 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.SellerProductID=PS.ItemNumber 
            LEFT OUTER JOIN dbo.itemdisplayprice AS IDP WITH (nolock) 
                ON A.ID = IDP.ItemID
            WHERE 
                A.CategoryID IN (SELECT * FROM @categoryTb) 
                AND A.[Status]=0  
                AND A.ShowOrder >= 0  
                AND (IDP.PriceType = 1 or IDP.PriceType is null)
                AND (IDP.MinNumber = 1 or IDP.MinNumber is null)
                AND (IDP.MaxNumber = 1 or IDP.MaxNumber is null)
         )   AS D 
	WHERE    /****WHERE CONDITION AT HERE****/
         D.SellingQty > @showZeroFalg 
         AND D.PriceCash > @showPriceFalg
	) AS D /**************************************/
    WHERE 
        D.[row_number] > (@ShowNumber*(@Page-1))
    ORDER BY D.CreateDate ASC

END


if(@OrderByType='Review' and @BrandId = 0)
begin
	SELECT TOP 
    (@ShowNumber)  D.ID 
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
   ,D.CreateDate
    FROM ( 
	SELECT *,row_number() OVER (ORDER BY D.Rating DESC) AS [row_number] /****ORDER CONDITION AT HERE****/
	FROM (    /**************************************/
            SELECT 
                A.ID AS ID 
               ,A.Name AS Name 
               ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
               ,A.Spechead AS Spechead 
               ,A.SpecDetail AS SpecDetail 
               ,A.PhotoName AS PhotoName 
               ,CAST(CASE WHEN IDP.DisplayPrice > 0 THEN IDP.DisplayPrice ELSE A.PriceCash END AS Decimal(12, 2)) AS PriceCash 
			   ,A.CreateDate AS CreateDate
               ,P.ID AS ProductID 
               ,P.FK AS ProductFK 
               ,PS.Rating AS Rating 
               ,PS.TotalReviews AS TotalReviews 
            /*   ,row_number() OVER ( 
            ORDER BY PS.Rating DESC) AS [row_number] */
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.ProductID=I.ProductID 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.ID=I.ProductID 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.SellerProductID=PS.ItemNumber 
            LEFT OUTER JOIN dbo.itemdisplayprice AS IDP WITH (nolock) 
                ON A.ID = IDP.ItemID
            WHERE 
                A.CategoryID IN (SELECT * FROM @categoryTb) 
                AND A.[Status]=0  
                AND A.ShowOrder >= 0  
                AND (IDP.PriceType = 1 or IDP.PriceType is null)
                AND (IDP.MinNumber = 1 or IDP.MinNumber is null)
                AND (IDP.MaxNumber = 1 or IDP.MaxNumber is null)
         )   AS D 
	WHERE    /****WHERE CONDITION AT HERE****/
         D.SellingQty > @showZeroFalg 
         AND D.PriceCash > @showPriceFalg
	) AS D /**************************************/
    WHERE 
        D.[row_number] > (@ShowNumber*(@Page-1))
    ORDER BY D.Rating DESC

END

if(@OrderByType='Review' and @BrandId > 0)
begin
	SELECT TOP 
    (@ShowNumber)  D.ID 
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
   ,D.CreateDate
    FROM ( 
	SELECT *,row_number() OVER (ORDER BY D.Rating DESC) AS [row_number] /****ORDER CONDITION AT HERE****/
	FROM (    /**************************************/
            SELECT 
                A.ID AS ID 
               ,A.Name AS Name 
               ,dbo.fn_EC_GetSellingQty(A.Qty, A.Qtyreg, I.Qty,I.Qtyreg, A.Qtylimit) AS SellingQty 
               ,A.Spechead AS Spechead 
               ,A.SpecDetail AS SpecDetail 
               ,A.PhotoName AS PhotoName 
               ,CAST(CASE WHEN IDP.DisplayPrice > 0 THEN IDP.DisplayPrice ELSE A.PriceCash END AS Decimal(12, 2)) AS PriceCash 
			   ,A.CreateDate AS CreateDate
               ,P.ID AS ProductID 
               ,P.FK AS ProductFK 
               ,PS.Rating AS Rating 
               ,PS.TotalReviews AS TotalReviews 
            /*   ,row_number() OVER ( 
            ORDER BY PS.Rating DESC) AS [row_number] */
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.ProductID=I.ProductID 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.ID=I.ProductID 
                AND P.ManufactureID IN (SELECT * FROM @brandTb) 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.SellerProductID=PS.ItemNumber 
            LEFT OUTER JOIN dbo.itemdisplayprice AS IDP WITH (nolock) 
                ON A.ID = IDP.ItemID
            WHERE 
                A.CategoryID IN (SELECT * FROM @categoryTb) 
                AND A.[Status]=0  
                AND A.ShowOrder >= 0  
                AND (IDP.PriceType = 1 or IDP.PriceType is null)
                AND (IDP.MinNumber = 1 or IDP.MinNumber is null)
                AND (IDP.MaxNumber = 1 or IDP.MaxNumber is null)
         )   AS D 
	WHERE    /****WHERE CONDITION AT HERE****/
         D.SellingQty > @showZeroFalg 
         AND D.PriceCash > @showPriceFalg
	) AS D /**************************************/
    WHERE 
        D.[row_number] > (@ShowNumber*(@Page-1))
    ORDER BY D.Rating DESC

END

END












GO
