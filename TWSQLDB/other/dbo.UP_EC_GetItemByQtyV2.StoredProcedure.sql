USE [TWSQLDB]
GO
/****** Object:  StoredProcedure [dbo].[UP_EC_GetItemByQtyV2]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Bill Wu>
-- Create date: <2013/12/16>
-- Update date: <2014/04/24><Include ItemDisplayPrice>
-- Description:	<This stored procedure's main object is detecting >
-- =============================================
CREATE PROCEDURE [dbo].[UP_EC_GetItemByQtyV2]
	  @Page int = 1 
	 ,@ShowNumber int = 20
	 ,@ShowAll int = 0
	/*@ShowAll input string: 
		0 (Show numbers of items, it dependent on @ShowNumber.)
		1 (Show all items.)
	*/
	 ,@ShowZero int = 0 
	/*@ShowZero input string: 
		0 (Don't show the item, when it's SellingQty equal 0.)
		1 (Show all items, even item's SellingQty equal 0.)
	*/
     ,@BrandIds char(4000)
	/*@BrandIds input string: 
		236,285 
	*/
     ,@Categoryids char(4000)
	/*@Categoryids input string: 
		84,85,86 (Each category id MUST is the end level.)
	*/
     ,@OrderByType char(10) 
	/*@OrderByType input string: 
		Qty (Order by item's qty.), 
		Hit (Order by item's review number.),
		Price (Order by item's price.),
		Review (Order by item's rating.)
		CreateDate (Order by item's create date.)
	*/
     ,@OrderBy char(4) 
	/*@OrderBy input string: 
		ASC (Order by ASC, only work when @OrderByType is 'Price' or 'CreateDate'.), 
		DESC (Order by DESC, only work when @OrderByType is 'Price' or 'CreateDate'.),
	*/
	 ,@PriceCondition decimal(12,2) = 0.00
	/*@ShowZero input string: 
		0 (Show all items.)
		xxxx (Show item search by PriceCash.)
	*/

AS
SET NOCOUNT ON
BEGIN

/********/

DECLARE @brandTb table
(
brandID int
)
DECLARE @categoryTb table
(
categoryID int
)
DECLARE @showZeroFalg int = 0,
		@showPriceFalg decimal(12,2) = 0.00


/********/

while charindex(',',@BrandIds) > 0
begin
insert into @brandTb select substring(@BrandIds,1,(charindex(',',@BrandIds)-1))
SET @BrandIds = substring(@BrandIds,charindex(',',@BrandIds)+1,len(@BrandIds))
end
insert into @brandTb
select @BrandIds


while charindex(',',@Categoryids) > 0
begin
insert into @categoryTb select substring(@Categoryids,1,(charindex(',',@Categoryids)-1))
SET @Categoryids = substring(@Categoryids,charindex(',',@Categoryids)+1,len(@Categoryids))
end
insert into @categoryTb
select @Categoryids

/********/

if(@ShowZero = 1)
BEGIN
set @showZeroFalg = -1
END

if(@PriceCondition > 0)
BEGIN
set @showPriceFalg = @PriceCondition
END

/********/

if(@ShowAll = 0)
begin
if(@OrderByType='Qty' and @BrandIds='')
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

if(@OrderByType = 'Qty' and @BrandIds != '')
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

if(@OrderByType='Hit' and @BrandIds='')
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

if(@OrderByType='Hit' and @BrandIds!='')
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

if(@OrderByType='Price' and @OrderBy='DESC' and @BrandIds='' )
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

if(@OrderByType='Price' and @OrderBy='DESC' and @BrandIds!='')
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



if(@OrderByType='Price' and @OrderBy='ASC' and @BrandIds='' )
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

if(@OrderByType='Price' and @OrderBy='ASC' and @BrandIds!='' )
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

if(@OrderByType='CreateDate' and @OrderBy='DESC' and @BrandIds='' )
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

if(@OrderByType='CreateDate' and @OrderBy='DESC' and @BrandIds!='')
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



if(@OrderByType='CreateDate' and @OrderBy='ASC' and @BrandIds='' )
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

if(@OrderByType='CreateDate' and @OrderBy='ASC' and @BrandIds!='' )
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


if(@OrderByType='Review' and @BrandIds='' )
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

if(@OrderByType='Review' and @BrandIds!='' )
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
/********/
if(@ShowAll = 1)
begin
if(@OrderByType='Qty' and @BrandIds='')
begin

	SELECT   
	D.ID 
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
        /*   ,row_number() OVER ( 
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
	ORDER BY D.SellingQty DESC
End

if(@OrderByType='Qty' and @BrandIds!='')
begin

	SELECT 
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
    ORDER BY D.SellingQty DESC
END

if(@OrderByType='Hit' and @BrandIds='')
begin
	SELECT
	D.ID 
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
    ORDER BY D.TotalReviews DESC

END

if(@OrderByType='Hit' and @BrandIdS!='')
begin
	SELECT   
	D.ID 
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
    ORDER BY D.TotalReviews DESC

END

if(@OrderByType='Price' and @OrderBy='DESC' and @BrandIds='' )
begin
	SELECT 
	D.ID 
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
    ORDER BY D.PriceCash DESC

END

if(@OrderByType='Price' and @OrderBy='DESC' and @BrandIds!='')
begin
	SELECT 
	D.ID 
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
    ORDER BY D.PriceCash DESC

END



if(@OrderByType='Price' and @OrderBy='ASC' and @BrandIds='' )
begin
	SELECT 
	D.ID 
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
	ORDER BY D.PriceCash ASC

END

if(@OrderByType='Price' and @OrderBy='ASC' and @BrandIds!='' )
begin
	SELECT 
	D.ID 
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
    ORDER BY D.PriceCash ASC

END


if(@OrderByType='CreateDate' and @OrderBy='DESC' and @BrandIds='' )
begin
	SELECT 
	D.ID 
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
	SELECT *,row_number() OVER (ORDER BY D.CreateDate DESC) AS [row_number] /****ORDER CONDITION AT HERE AND BOTTOM****/
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
    ORDER BY D.CreateDate DESC

END

if(@OrderByType='CreateDate' and @OrderBy='DESC' and @BrandIds!='')
begin
	SELECT
	D.ID 
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
	SELECT *,row_number() OVER (ORDER BY D.CreateDate DESC) AS [row_number] /****ORDER CONDITION AT HERE AND BOTTOM****/
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
    ORDER BY D.CreateDate DESC

END



if(@OrderByType='CreateDate' and @OrderBy='ASC' and @BrandIds='' )
begin
	SELECT
	D.ID 
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
	SELECT *,row_number() OVER (ORDER BY D.CreateDate ASC) AS [row_number] /****ORDER CONDITION AT HERE AND BOTTOM****/
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
    ORDER BY D.CreateDate ASC

END

if(@OrderByType='CreateDate' and @OrderBy='ASC' and @BrandIds!='' )
begin
	SELECT
	D.ID 
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
	SELECT *,row_number() OVER (ORDER BY D.CreateDate ASC) AS [row_number] /****ORDER CONDITION AT HERE AND BOTTOM****/
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
    ORDER BY D.CreateDate ASC

END


if(@OrderByType='Review' and @BrandIds='' )
begin
	SELECT 
	D.ID 
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
	SELECT *,row_number() OVER (ORDER BY D.Rating DESC) AS [row_number] /****ORDER CONDITION AT HERE AND BOTTOM****/
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
    ORDER BY D.Rating DESC

END

if(@OrderByType='Review' and @BrandIds!='' )
begin
	SELECT 
	D.ID 
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
	SELECT *,row_number() OVER (ORDER BY D.Rating DESC) AS [row_number] /****ORDER CONDITION AT HERE AND BOTTOM****/
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
    ORDER BY D.Rating DESC

END
END
/********/
END




GO
