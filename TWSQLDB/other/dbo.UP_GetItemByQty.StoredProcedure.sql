USE [TWSQLDB]
GO
/****** Object:  StoredProcedure [dbo].[UP_GetItemByQty]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO







CREATE PROCEDURE [dbo].[UP_GetItemByQty] 
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
    (20)  D.item_id 
   ,D.item_name 
   ,D.item_spechead 
   ,D.item_specdetail 
   ,D.item_photoname 
   ,D.item_pricecash 
   ,D.product_id 
   ,D.product_fk 
   ,D.productfromws_rating 
   ,D.productfromws_totalreviews   
   ,D.item_sellingQty 
FROM ( 
        SELECT 
            A.item_id AS item_id 
           ,A.item_name AS item_name 
           ,dbo.fn_get_sellingQty(A.item_qty, A.item_qtyreg, I.itemstock_qty,I.itemstock_qtyreg, A.item_qtylimit) AS item_sellingQty 
           ,A.item_spechead AS item_spechead 
           ,A.item_specdetail AS item_specdetail 
           ,A.item_photoname AS item_photoname 
           ,A.item_pricecash AS item_pricecash 
           ,P.product_id AS product_id 
           ,P.product_fk AS product_fk 
           ,PS.productfromws_rating AS productfromws_rating 
           ,PS.productfromws_totalreviews AS productfromws_totalreviews 
           ,row_number() OVER ( 
        ORDER BY dbo.fn_get_sellingQty(item_qty, item_qtyreg, itemstock_qty,itemstock_qtyreg, item_qtylimit)  DESC) AS [row_number] 
        FROM dbo.item A WITH (NOLOCK) 
        INNER JOIN dbo.itemstock I WITH (NOLOCK) 
            ON A.item_productid=I.itemstock_productid 
        INNER JOIN dbo.product P WITH (NOLOCK) 
            ON P.product_id=I.itemstock_productid 
        LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
            ON P.product_sellerproductid=PS.productfromws_itemnumber 
        WHERE 
            A.item_categoryid=@Categoryid 
            AND A.item_status=0  
            AND A.item_showorder >= 0  
     )   AS D 
WHERE 
    D.[row_number] > (20*(@Page-1))     
ORDER BY D.item_sellingQty DESC
End

if(@OrderByType='Qty' and @BrandId>0)
begin

	SELECT TOP 
        (20)  D.item_id 
       ,D.item_name 
       ,D.item_spechead 
       ,D.item_specdetail 
       ,D.item_photoname 
       ,D.item_pricecash 
       ,D.product_id 
       ,D.product_fk 
       ,D.productfromws_rating 
       ,D.productfromws_totalreviews   
       ,D.item_sellingQty 
    FROM ( 
            SELECT 
                A.item_id AS item_id 
               ,A.item_name AS item_name 
               ,dbo.fn_get_sellingQty(A.item_qty, A.item_qtyreg, I.itemstock_qty,I.itemstock_qtyreg, A.item_qtylimit) AS item_sellingQty 
               ,A.item_spechead AS item_spechead 
               ,A.item_specdetail AS item_specdetail 
               ,A.item_photoname AS item_photoname 
               ,A.item_pricecash AS item_pricecash 
               ,P.product_id AS product_id 
               ,P.product_fk AS product_fk 
               ,PS.productfromws_rating AS productfromws_rating 
               ,PS.productfromws_totalreviews AS productfromws_totalreviews 
               ,row_number() OVER ( 
            ORDER BY dbo.fn_get_sellingQty(item_qty, item_qtyreg, itemstock_qty,itemstock_qtyreg, item_qtylimit)  DESC) AS [row_number] 
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.item_productid=I.itemstock_productid 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.product_id=I.itemstock_productid 
                AND product_manufactureid=@BrandId 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.product_sellerproductid=PS.productfromws_itemnumber 
            WHERE 
                A.item_categoryid=@Categoryid 
                AND A.item_status=0  
                AND A.item_showorder >= 0  
         )   AS D 
    WHERE 
        D.[row_number] > (20*(@Page-1))      
    ORDER BY D.item_sellingQty DESC
END

if(@OrderByType='Hit' and @BrandId=0)
begin
	SELECT TOP 
        (20)  D.item_id 
       ,D.item_name 
       ,D.item_spechead 
       ,D.item_specdetail 
       ,D.item_photoname 
       ,D.item_pricecash 
       ,D.product_id 
       ,D.product_fk 
       ,D.productfromws_rating 
       ,D.productfromws_totalreviews   
       ,D.item_sellingQty 
    FROM ( 
            SELECT 
                A.item_id AS item_id 
               ,A.item_name AS item_name 
               ,dbo.fn_get_sellingQty(A.item_qty, A.item_qtyreg, I.itemstock_qty,I.itemstock_qtyreg, A.item_qtylimit) AS item_sellingQty 
               ,A.item_spechead AS item_spechead 
               ,A.item_specdetail AS item_specdetail 
               ,A.item_photoname AS item_photoname 
               ,A.item_pricecash AS item_pricecash 
               ,P.product_id AS product_id 
               ,P.product_fk AS product_fk 
               ,PS.productfromws_rating AS productfromws_rating 
               ,PS.productfromws_totalreviews AS productfromws_totalreviews 
               ,row_number() OVER ( 
            ORDER BY PS.productfromws_totalreviews DESC) AS [row_number] 
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.item_productid=I.itemstock_productid 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.product_id=I.itemstock_productid 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.product_sellerproductid=PS.productfromws_itemnumber 
            WHERE 
                A.item_categoryid=@Categoryid 
                AND A.item_status=0  
                AND A.item_showorder >= 0  
         )   AS D 
    WHERE 
        D.[row_number] > (20*(@Page-1))         
    ORDER BY D.productfromws_totalreviews DESC

END

if(@OrderByType='Hit' and @BrandId>0)
begin
	SELECT TOP 
        (20)  D.item_id 
       ,D.item_name 
       ,D.item_spechead 
       ,D.item_specdetail 
       ,D.item_photoname 
       ,D.item_pricecash 
       ,D.product_id 
       ,D.product_fk 
       ,D.productfromws_rating 
       ,D.productfromws_totalreviews   
       ,D.item_sellingQty 
    FROM ( 
            SELECT 
                A.item_id AS item_id 
               ,A.item_name AS item_name 
               ,dbo.fn_get_sellingQty(A.item_qty, A.item_qtyreg, I.itemstock_qty,I.itemstock_qtyreg, A.item_qtylimit) AS item_sellingQty 
               ,A.item_spechead AS item_spechead 
               ,A.item_specdetail AS item_specdetail 
               ,A.item_photoname AS item_photoname 
               ,A.item_pricecash AS item_pricecash 
               ,P.product_id AS product_id 
               ,P.product_fk AS product_fk 
               ,PS.productfromws_rating AS productfromws_rating 
               ,PS.productfromws_totalreviews AS productfromws_totalreviews 
               ,row_number() OVER ( 
            ORDER BY PS.productfromws_totalreviews DESC) AS [row_number] 
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.item_productid=I.itemstock_productid 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.product_id=I.itemstock_productid 
                AND product_manufactureid=@BrandId 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.product_sellerproductid=PS.productfromws_itemnumber 
            WHERE 
                A.item_categoryid=@Categoryid 
                AND A.item_status=0  
                AND A.item_showorder >= 0  
         )   AS D 
    WHERE 
        D.[row_number] > (20*(@Page-1))          
    ORDER BY D.productfromws_totalreviews DESC

END

if(@OrderByType='Price' and @OrderBy='DESC' and @BrandId=0 )
begin
	SELECT TOP 
        (20)  D.item_id 
       ,D.item_name 
       ,D.item_spechead 
       ,D.item_specdetail 
       ,D.item_photoname 
       ,D.item_pricecash 
       ,D.product_id 
       ,D.product_fk 
       ,D.productfromws_rating 
       ,D.productfromws_totalreviews   
       ,D.item_sellingQty 
    FROM ( 
            SELECT 
                A.item_id AS item_id 
               ,A.item_name AS item_name 
               ,dbo.fn_get_sellingQty(A.item_qty, A.item_qtyreg, I.itemstock_qty,I.itemstock_qtyreg, A.item_qtylimit) AS item_sellingQty 
               ,A.item_spechead AS item_spechead 
               ,A.item_specdetail AS item_specdetail 
               ,A.item_photoname AS item_photoname 
               ,A.item_pricecash AS item_pricecash 
               ,P.product_id AS product_id 
               ,P.product_fk AS product_fk 
               ,PS.productfromws_rating AS productfromws_rating 
               ,PS.productfromws_totalreviews AS productfromws_totalreviews 
               ,row_number() OVER ( 
            ORDER BY A.item_pricecash DESC) AS [row_number] 
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.item_productid=I.itemstock_productid 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.product_id=I.itemstock_productid 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.product_sellerproductid=PS.productfromws_itemnumber 
            WHERE 
                A.item_categoryid=@Categoryid 
                AND A.item_status=0  
                AND A.item_showorder >= 0  
         )   AS D 
    WHERE 
        D.[row_number] > (20*(@Page-1))          
    ORDER BY D.item_pricecash DESC

END

if(@OrderByType='Price' and @OrderBy='DESC' and @BrandId>0)
begin
	SELECT TOP 
        (20)  D.item_id 
       ,D.item_name 
       ,D.item_spechead 
       ,D.item_specdetail 
       ,D.item_photoname 
       ,D.item_pricecash 
       ,D.product_id 
       ,D.product_fk 
       ,D.productfromws_rating 
       ,D.productfromws_totalreviews   
       ,D.item_sellingQty 
    FROM ( 
            SELECT 
                A.item_id AS item_id 
               ,A.item_name AS item_name 
               ,dbo.fn_get_sellingQty(A.item_qty, A.item_qtyreg, I.itemstock_qty,I.itemstock_qtyreg, A.item_qtylimit) AS item_sellingQty 
               ,A.item_spechead AS item_spechead 
               ,A.item_specdetail AS item_specdetail 
               ,A.item_photoname AS item_photoname 
               ,A.item_pricecash AS item_pricecash 
               ,P.product_id AS product_id 
               ,P.product_fk AS product_fk 
               ,PS.productfromws_rating AS productfromws_rating 
               ,PS.productfromws_totalreviews AS productfromws_totalreviews 
               ,row_number() OVER ( 
            ORDER BY A.item_pricecash DESC) AS [row_number] 
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.item_productid=I.itemstock_productid 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.product_id=I.itemstock_productid 
                AND product_manufactureid=@BrandId 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.product_sellerproductid=PS.productfromws_itemnumber 
            WHERE 
                A.item_categoryid=@Categoryid 
                AND A.item_status=0  
                AND A.item_showorder >= 0  
         )   AS D 
    WHERE 
        D.[row_number] > (20*(@Page-1))          
    ORDER BY D.item_pricecash DESC

END



if(@OrderByType='Price' and @OrderBy='ASC' and @BrandId=0 )
begin
	SELECT TOP 
        (20)  D.item_id 
       ,D.item_name 
       ,D.item_spechead 
       ,D.item_specdetail 
       ,D.item_photoname 
       ,D.item_pricecash 
       ,D.product_id 
       ,D.product_fk 
       ,D.productfromws_rating 
       ,D.productfromws_totalreviews   
       ,D.item_sellingQty 
    FROM ( 
            SELECT 
                A.item_id AS item_id 
               ,A.item_name AS item_name 
               ,dbo.fn_get_sellingQty(A.item_qty, A.item_qtyreg, I.itemstock_qty,I.itemstock_qtyreg, A.item_qtylimit) AS item_sellingQty 
               ,A.item_spechead AS item_spechead 
               ,A.item_specdetail AS item_specdetail 
               ,A.item_photoname AS item_photoname 
               ,A.item_pricecash AS item_pricecash 
               ,P.product_id AS product_id 
               ,P.product_fk AS product_fk 
               ,PS.productfromws_rating AS productfromws_rating 
               ,PS.productfromws_totalreviews AS productfromws_totalreviews 
               ,row_number() OVER ( 
            ORDER BY A.item_pricecash ASC) AS [row_number] 
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.item_productid=I.itemstock_productid 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.product_id=I.itemstock_productid 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.product_sellerproductid=PS.productfromws_itemnumber 
            WHERE 
                A.item_categoryid=@Categoryid 
                AND A.item_status=0  
                AND A.item_showorder >= 0  
         )   AS D 
    WHERE 
        D.[row_number] > (20*(@Page-1))          
    ORDER BY D.item_pricecash ASC

END

if(@OrderByType='Price' and @OrderBy='ASC' and @BrandId>0 )
begin
	SELECT TOP 
        (20)  D.item_id 
       ,D.item_name 
       ,D.item_spechead 
       ,D.item_specdetail 
       ,D.item_photoname 
       ,D.item_pricecash 
       ,D.product_id 
       ,D.product_fk 
       ,D.productfromws_rating 
       ,D.productfromws_totalreviews   
       ,D.item_sellingQty 
    FROM ( 
            SELECT 
                A.item_id AS item_id 
               ,A.item_name AS item_name 
               ,dbo.fn_get_sellingQty(A.item_qty, A.item_qtyreg, I.itemstock_qty,I.itemstock_qtyreg, A.item_qtylimit) AS item_sellingQty 
               ,A.item_spechead AS item_spechead 
               ,A.item_specdetail AS item_specdetail 
               ,A.item_photoname AS item_photoname 
               ,A.item_pricecash AS item_pricecash 
               ,P.product_id AS product_id 
               ,P.product_fk AS product_fk 
               ,PS.productfromws_rating AS productfromws_rating 
               ,PS.productfromws_totalreviews AS productfromws_totalreviews 
               ,row_number() OVER ( 
            ORDER BY A.item_pricecash ASC) AS [row_number] 
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.item_productid=I.itemstock_productid 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.product_id=I.itemstock_productid 
                AND product_manufactureid=@BrandId 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.product_sellerproductid=PS.productfromws_itemnumber 
            WHERE 
                A.item_categoryid=@Categoryid 
                AND A.item_status=0  
                AND A.item_showorder >= 0  
         )   AS D 
    WHERE 
        D.[row_number] > (20*(@Page-1))          
    ORDER BY D.item_pricecash ASC

END


if(@OrderByType='Review' and @BrandId=0 )
begin
	SELECT TOP 
        (20)  D.item_id 
       ,D.item_name 
       ,D.item_spechead 
       ,D.item_specdetail 
       ,D.item_photoname 
       ,D.item_pricecash 
       ,D.product_id 
       ,D.product_fk 
       ,D.productfromws_rating 
       ,D.productfromws_totalreviews   
       ,D.item_sellingQty 
    FROM ( 
            SELECT 
                A.item_id AS item_id 
               ,A.item_name AS item_name 
               ,dbo.fn_get_sellingQty(A.item_qty, A.item_qtyreg, I.itemstock_qty,I.itemstock_qtyreg, A.item_qtylimit) AS item_sellingQty 
               ,A.item_spechead AS item_spechead 
               ,A.item_specdetail AS item_specdetail 
               ,A.item_photoname AS item_photoname 
               ,A.item_pricecash AS item_pricecash  
               ,P.product_id AS product_id 
               ,P.product_fk AS product_fk 
               ,PS.productfromws_rating AS productfromws_rating 
               ,PS.productfromws_totalreviews AS productfromws_totalreviews 
               ,row_number() OVER ( 
            ORDER BY PS.productfromws_rating DESC) AS [row_number] 
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.item_productid=I.itemstock_productid 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.product_id=I.itemstock_productid 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.product_sellerproductid=PS.productfromws_itemnumber 
            WHERE 
                A.item_categoryid=@Categoryid 
                AND A.item_status=0  
                AND A.item_showorder >= 0  
         )   AS D 
    WHERE 
        D.[row_number] > (20*(@Page-1))        
    ORDER BY D.productfromws_rating DESC

END

if(@OrderByType='Review' and @BrandId>0 )
begin
	SELECT TOP 
        (20)  D.item_id 
       ,D.item_name 
       ,D.item_spechead 
       ,D.item_specdetail 
       ,D.item_photoname 
       ,D.item_pricecash 
       ,D.product_id 
       ,D.product_fk 
       ,D.productfromws_rating 
       ,D.productfromws_totalreviews   
       ,D.item_sellingQty 
    FROM ( 
            SELECT 
                A.item_id AS item_id 
               ,A.item_name AS item_name 
               ,dbo.fn_get_sellingQty(A.item_qty, A.item_qtyreg, I.itemstock_qty,I.itemstock_qtyreg, A.item_qtylimit) AS item_sellingQty 
               ,A.item_spechead AS item_spechead 
               ,A.item_specdetail AS item_specdetail 
               ,A.item_photoname AS item_photoname 
               ,A.item_pricecash AS item_pricecash  
               ,P.product_id AS product_id 
               ,P.product_fk AS product_fk 
               ,PS.productfromws_rating AS productfromws_rating 
               ,PS.productfromws_totalreviews AS productfromws_totalreviews 
               ,row_number() OVER ( 
            ORDER BY PS.productfromws_rating DESC) AS [row_number] 
            FROM dbo.item A WITH (NOLOCK) 
            INNER JOIN dbo.itemstock I WITH (NOLOCK) 
                ON A.item_productid=I.itemstock_productid 
            INNER JOIN dbo.product P WITH (NOLOCK) 
                ON P.product_id=I.itemstock_productid 
                AND product_manufactureid=@BrandId 
            LEFT JOIN dbo.productfromws PS WITH (NOLOCK) 
                ON P.product_sellerproductid=PS.productfromws_itemnumber 
            WHERE 
                A.item_categoryid=@Categoryid 
                AND A.item_status=0  
                AND A.item_showorder >= 0  
         )   AS D 
    WHERE 
        D.[row_number] > (20*(@Page-1))         
    ORDER BY D.productfromws_rating DESC

END



END





GO
