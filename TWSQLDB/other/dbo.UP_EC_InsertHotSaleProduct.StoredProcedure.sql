USE [TWSQLDB]
GO
/****** Object:  StoredProcedure [dbo].[UP_EC_InsertHotSaleProduct]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[UP_EC_InsertHotSaleProduct]
       @dboutput varchar(100) OUTPUT 
AS
DECLARE @dbcategoryid int
DECLARE @dbcategorylayer int
DECLARE @gonum int
SET @gonum=0




DECLARE @category_layer1 TABLE 
( 
     ID int
    ,ParentID int
);

INSERT INTO @category_layer1 
SELECT 
    ID
   ,Parentid 
FROM category WITH (NOLOCK) 
WHERE 
    Layer=0 
ORDER BY ParentID

DECLARE @category_layer2 TABLE 
( 
     ID int
    ,ParentID int
);
INSERT INTO @category_layer2 
SELECT 
    ID
   ,ParentID 
FROM category WITH (NOLOCK) 
WHERE 
    Layer=1  
ORDER BY ParentID
   
    
DECLARE @category_layer3 TABLE 
( 
     ID int
    ,ParentID int
);
 INSERT INTO @category_layer3 
 SELECT 
     ID
    ,ParentID 
 FROM category WITH (NOLOCK) 
 WHERE 
     Layer=2  
 ORDER BY ParentID
   
DECLARE @category_layer4 TABLE 
( 
     ID int
    ,ParentID int
);
 INSERT INTO @category_layer4 
 SELECT 
     ID
    ,ParentID 
 FROM category WITH (NOLOCK) 
 WHERE 
     Layer=3  
 ORDER BY ParentID
      
   




DELETE HotSale 
WHERE 
    CategoryID=0 
    AND ShowType=0 
    AND CategoryLayer=99
INSERT HotSale 
(
     CategoryID
    ,CategoryLayer
    ,ItemID
    ,SellerID
    ,ShowType
    ,HotsaleOrder
    ,DateStart
    ,DateEnd
    ,CreateUser
    ,CreateDate
    ,Updated
) 	
SELECT TOP 30 0
    
   ,99
   ,I.ID
   ,CONVERT(int,ISNULL(I.SellerID,0))
   ,0 
   ,CONVERT(int,ROW_NUMBER() OVER(
ORDER BY CO.ID,I.QtyReg DESC,I.UpdateDate DESC )) AS RowNum ,dateadd("d",-7,GETDATE()),dateadd("d",+7,GETDATE()),'HotSale_SP_batch',getdate(),0
from item I with (nolock) 
inner join category CO with (nolock) on I.CategoryID=CO.ID 
where I.[Status]=0 and I.ShowOrder>=0 and CO.ID in (select D.ID from @category_layer4 D where D.ParentID in (select C.ID from @category_layer3 C where C.ParentID in (select B.ID from @category_layer2 B where B.ParentID in (select A.ID from @category_layer1 A)))
union
select C.ID from @category_layer3 C where C.ParentID in (select B.ID from @category_layer2 B where B.ParentID in (select A.ID from @category_layer1 A))
union
select B.ID from @category_layer2 B where B.ParentID in (select A.ID from @category_layer1 A)
union
select A.ID from @category_layer1 A )
order by ID,I.Qtyreg desc,I.UpdateDate desc 





DECLARE mycursor CURSOR FOR select distinct ID,Layer from category with (nolock) 
OPEN mycursor
FETCH NEXT FROM mycursor INTO @dbcategoryid,@dbcategorylayer
WHILE (@@FETCH_STATUS = 0)
BEGIN

delete HotSale where CategoryID=@dbcategoryid and ShowType=0

if(@dbcategorylayer=0)
	begin
		--指定layer1
	
		insert HotSale (CategoryID,CategoryLayer,ItemID,SellerID,ShowType,HotSaleOrder,DateStart,DateEnd,CreateUser,CreateDate,Updated) 	
select top 30 @dbcategoryid,@dbcategorylayer,I.ID,convert(int,ISNULL(I.SellerID,0)),0
,convert(int,ROW_NUMBER() OVER(ORDER BY CO.ID,I.Qtyreg desc,I.UpdateDate desc )) as RowNum
,dateadd("d",-7,getdate()),dateadd("d",+7,getdate()),'HotSale_SP_batch',GETDATE(),0 
FROM item I WITH (NOLOCK) 
INNER JOIN category CO WITH (NOLOCK) 
    ON I.CategoryID=CO.ID 
WHERE 
    I.[Status]=0 
    AND I.ShowOrder>=0  
    AND CO.ID IN (
            SELECT 
                D.ID 
            FROM @category_layer4 D 
            WHERE 
                D.ParentID IN (
                        SELECT 
                            C.ID 
                        FROM @category_layer3 C 
                        WHERE 
                            C.ParentID IN (
                                    SELECT 
                                        B.ID 
                                    FROM @category_layer2 B 
                                    WHERE 
                                        B.ParentID=@dbcategoryid
                                )
                    ) 
            UNION

            SELECT 
                C.ID 
            FROM @category_layer3 C 
            WHERE 
                C.ParentID IN (
                        SELECT 
                            B.ID 
                        FROM @category_layer2 B 
                        WHERE 
                            B.ParentID=@dbcategoryid
                    ) 
            UNION

            SELECT 
                B.ID 
            FROM @category_layer2 B 
            WHERE 
                B.ParentID =@dbcategoryid 
            UNION

            SELECT 
                A.ID 
            FROM @category_layer1 A  
            WHERE 
                A.ID=@dbcategoryid
        ) 
ORDER BY CO.ID,I.Qtyreg DESC,I.Updatedate DESC 
	
	END
	
if(@dbcategorylayer=1)
	begin
				INSERT HotSale 
                (
                     CategoryID
                    ,CategoryLayer
                    ,ItemID
                    ,SellerID
                    ,ShowType
                    ,HotsaleOrder
                    ,DateStart
                    ,DateEnd
                    ,CreateUser
                    ,CreateDate
                    ,Updated
                ) 	
                SELECT TOP 30 
                    @dbcategoryid
                   ,@dbcategorylayer
                   ,I2.ID
                   ,CONVERT(int,ISNULL(I2.SellerID,0))
                   ,0 
                   ,CONVERT(int,ROW_NUMBER() OVER(
                ORDER BY CO2.ID,I2.Qtyreg DESC,I2.UpdateDate DESC )) AS RowNum ,dateadd("d",-7,GETDATE()),dateadd("d",+7,GETDATE()),'HotSale_SP_batch',getdate(),0
from item I2 with (nolock) 
inner join category CO2 with (nolock) on I2.CategoryID=CO2.ID 
where I2.[status]=0 and I2.ShowOrder>=0  and CO2.ID in (select D.ID from @category_layer4 D where D.ParentID in (select C.ID from @category_layer3 C where C.ParentID =@dbcategoryid )
union
select C.ID from @category_layer3 C where C.ParentID =@dbcategoryid
union 
select B.ID from @category_layer2 B where B.ID=@dbcategoryid)
order by CO2.ID,I2.Qtyreg desc,I2.UpdateDate desc 

	
	
	end
	
if(@dbcategorylayer=2)
	begin
		insert HotSale (CategoryID,CategoryLayer,ItemID,SellerID,ShowType,HotsaleOrder,DateStart,DateEnd,CreateUser,CreateDate,Updated) 	
select top 30 @dbcategoryid,@dbcategorylayer,I.ID,convert(int,ISNULL(I.SellerID,0)),0
,convert(int,ROW_NUMBER() OVER(ORDER BY C.ID,I.Qtyreg desc,I.Updatedate desc )) as RowNum
,dateadd("d",-7,getdate()),dateadd("d",+7,getdate()),'HotSale_SP_batch',GETDATE(),0 
                FROM item I WITH (NOLOCK) 
                INNER JOIN category C WITH (NOLOCK) 
                    ON I.CategoryID=C.ID 
                WHERE 
                    I.[Status]=0 
                    AND I.ShowOrder>=0 
                    AND C.ID IN (
                            SELECT 
                                D.ID 
                            FROM @category_layer4 D 
                            WHERE 
                                D.ParentID =@dbcategoryid 
                            UNION

                            SELECT 
                                C.ID 
                            FROM @category_layer3 C 
                            WHERE 
                                C.ID =@dbcategoryid
                        ) 
                ORDER BY C.ID,I.Qtyreg DESC,I.UpdateDate DESC 
	
	
	END
	
if(@dbcategorylayer=3)
	begin
		INSERT HotSale 
        (
             CategoryID
            ,CategoryLayer
            ,ItemID
            ,SellerID
            ,ShowType
            ,HotsaleOrder
            ,DateStart
            ,DateEnd
            ,CreateUser
            ,CreateDate
            ,Updated
        ) 	
        SELECT TOP 30 
            @dbcategoryid
           ,@dbcategorylayer
           ,I.ID
           ,CONVERT(int,ISNULL(I.SellerID,0))
           ,0 
           ,CONVERT(int,ROW_NUMBER() OVER(
        ORDER BY CO.ID,I.Qtyreg DESC,I.UpdateDate DESC )) AS RowNum ,dateadd("d",-7,GETDATE()),dateadd("d",+7,GETDATE()),'HotSale_SP_batch',getdate(),0
from item I with (nolock) 
inner join category CO with (nolock) on I.CategoryID=CO.ID 
where I.[Status]=0 and I.ShowOrder>=0  and CO.ID in (select D.ID from @category_layer4 D where D.ID =@dbcategoryid)
order by CO.ID,I.Qtyreg desc,I.UpdateDate desc 

	
	
	end




	
	
	FETCH NEXT FROM mycursor INTO @dbcategoryid,@dbcategorylayer

END

CLOSE mycursor
DEALLOCATE mycursor
select  @dboutput=convert(varchar(100),getdate())



GO
