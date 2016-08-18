USE [TWSQLDB]
GO
/****** Object:  StoredProcedure [dbo].[UP_EC_InsertNewProduct]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE PROCEDURE [dbo].[UP_EC_InsertNewProduct]
       @dboutput varchar(100) OUTPUT 
AS
DECLARE @dbcategoryid int
DECLARE @dbcategorylayer int
DECLARE @gonum int
SET @gonum=0




DECLARE @Layer1 TABLE 
( 
     ID int
    ,ParentID int
);

INSERT INTO @Layer1 
SELECT 
    ID
   ,ParentID 
FROM category WITH (NOLOCK) 
WHERE 
    Layer=0 
ORDER BY ParentID

DECLARE @Layer2 TABLE 
( 
     ID int
    ,ParentID int
);
INSERT INTO @Layer2 
SELECT 
    ID
   ,ParentID 
FROM category WITH (NOLOCK) 
WHERE 
    Layer=1  
ORDER BY ParentID
   
    
DECLARE @Layer3 TABLE 
( 
     ID int
    ,ParentID int
);
 INSERT INTO @Layer3 
 SELECT 
     ID
    ,ParentID 
 FROM category WITH (NOLOCK) 
 WHERE 
     Layer=2  
 ORDER BY ParentID
   
DECLARE @Layer4 TABLE 
( 
     ID int
    ,ParentID int
);
 INSERT INTO @Layer4 
 SELECT 
     ID
    ,ParentID 
 FROM category WITH (NOLOCK) 
 WHERE 
     Layer=3  
 ORDER BY ParentID
      
   




DELETE newproduct 
WHERE 
    CategoryID=0 
    AND ShowType=0 
    AND CategoryLayer=99
INSERT newproduct 
(
     CategoryID
    ,CategoryLayer
    ,ItemID
    ,SellerID
    ,ShowType
    ,NewproductOrder
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
ORDER BY CO.ID,I.CreateDate DESC,I.Updatedate DESC )) AS RowNum ,dateadd("d",-7,GETDATE()),dateadd("d",+7,GETDATE()),'newproduct_SP_batch',getdate(),0
from item I with (nolock) 
inner join category CO with (nolock) on I.CategoryID=CO.ID 
where I.[Status]=0 and I.ShowOrder>=0   and  CO.ID in (select D.ID from @Layer4 D where D.ParentID in (select C.ID from @Layer3 C where C.ParentID in (select B.ID from @Layer2 B where B.ParentID in (select A.ID from @Layer1 A)))
union
select C.ID from @Layer3 C where C.ParentID in (select B.ID from @Layer2 B where B.ParentID in (select A.ID from @Layer1 A))
union
select B.ID from @Layer2 B where B.ParentID in (select A.ID from @Layer1 A)
union
select A.ID from @Layer1 A )
order by CO.ID,I.CreateDate desc,I.Updatedate desc 





DECLARE mycursor CURSOR FOR select distinct ID,Layer from category with (nolock) 
OPEN mycursor
FETCH NEXT FROM mycursor INTO @dbcategoryid,@dbcategorylayer
WHILE (@@FETCH_STATUS = 0)
BEGIN

delete newproduct where CategoryID=@dbcategoryid and ShowType=0

if(@dbcategorylayer=0)
	begin
		--指定layer1
	
		insert newproduct (CategoryID,CategoryLayer,ItemID,SellerID,ShowType,NewProductOrder,DateStart,DateEnd,CreateUser,CreateDate,Updated) 	
select top 30 @dbcategoryid,@dbcategorylayer,I.ID,convert(int,ISNULL(I.SellerID,0)),0
,convert(int,ROW_NUMBER() OVER(ORDER BY CO.ID,I.CreateDate desc,I.UpdateDate desc )) as RowNum
,dateadd("d",-7,getdate()),dateadd("d",+7,getdate()),'newproduct_SP_batch',GETDATE(),0 
FROM item I WITH (NOLOCK) 
INNER JOIN category CO WITH (NOLOCK) 
    ON I.CategoryID=CO.ID 
WHERE 
    I.[status]=0 
    AND I.ShowOrder>=0    
    AND CO.ID IN (
            SELECT 
                D.ID 
            FROM @Layer4 D 
            WHERE 
                D.ParentID IN (
                        SELECT 
                            C.ID 
                        FROM @Layer3 C 
                        WHERE 
                            C.ParentID IN (
                                    SELECT 
                                        B.ID 
                                    FROM @Layer2 B 
                                    WHERE 
                                        B.ParentID=@dbcategoryid
                                )
                    ) 
            UNION

            SELECT 
                C.ID 
            FROM @Layer3 C 
            WHERE 
                C.ParentID IN (
                        SELECT 
                            B.ID 
                        FROM @Layer2 B 
                        WHERE 
                            B.ParentID=@dbcategoryid
                    ) 
            UNION

            SELECT 
                B.ID 
            FROM @Layer2 B 
            WHERE 
                B.ParentID =@dbcategoryid 
            UNION

            SELECT 
                A.ID 
            FROM @Layer1 A  
            WHERE 
                A.ID=@dbcategoryid
        ) 
ORDER BY CO.ID,I.CreateDate DESC,I.UpdateDate DESC 
	
	END
	
if(@dbcategorylayer=1)
	begin
				INSERT newproduct 
                (
                     CategoryID
                    ,CategoryLayer
                    ,ItemID
                    ,SellerID
                    ,ShowType
                    ,NewproductOrder
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
                ORDER BY CO.ID,I.CreateDate DESC,I.UpdateDate DESC )) AS RowNum ,dateadd("d",-7,GETDATE()),dateadd("d",+7,GETDATE()),'newproduct_SP_batch',getdate(),0
from item I with (nolock) 
inner join category CO with (nolock) on I.CategoryID=CO.ID 
where I.[Status]=0 and I.ShowOrder>=0  and CO.ID in (select D.ID from @Layer4 D where D.ParentID in (select C.ID from @Layer3 C where C.ParentID =@dbcategoryid )
union
select C.ID from @Layer3 C where C.ParentID =@dbcategoryid
union 
select B.ID from @Layer2 B where B.ID=@dbcategoryid)
order by CO.ID,I.CreateDate desc,I.UpdateDate desc 

	
	
	end
	
if(@dbcategorylayer=2)
	begin
		insert newproduct (CategoryID,CategoryLayer,ItemID,SellerID,ShowType,NewProductOrder,DateStart,DateEnd,CreateUser,CreateDate,Updated) 	
select top 30 @dbcategoryid,@dbcategorylayer,I.ID,convert(int,ISNULL(I.SellerID,0)),0
,convert(int,ROW_NUMBER() OVER(ORDER BY CO.ID,I.CreateDate desc,I.UpdateDate desc )) as RowNum
,dateadd("d",-7,getdate()),dateadd("d",+7,getdate()),'newproduct_SP_batch',GETDATE(),0 
                FROM item I WITH (NOLOCK) 
                INNER JOIN category CO WITH (NOLOCK) 
                    ON I.CategoryID=CO.ID 
                WHERE 
                    I.[Status]=0 
                    AND I.ShowOrder>=0    
                    AND CO.ID IN (
                            SELECT 
                                D.ID 
                            FROM @Layer4 D 
                            WHERE 
                                D.ParentID =@dbcategoryid 
                            UNION

                            SELECT 
                                C.ID 
                            FROM @Layer3 C 
                            WHERE 
                                C.ID =@dbcategoryid
                        ) 
                ORDER BY CO.ID,I.CreateDate DESC,I.UpdateDate DESC 
	
	
	END
	
if(@dbcategorylayer=3)
	begin
		INSERT newproduct 
        (
             CategoryID
            ,CategoryLayer
            ,ItemID
            ,SellerID
            ,ShowType
            ,NewproductOrder
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
        ORDER BY CO.ID,I.CreateDate DESC,I.UpdateDate DESC )) AS RowNum ,dateadd("d",-7,GETDATE()),dateadd("d",+7,GETDATE()),'newproduct_SP_batch',getdate(),0
from item I with (nolock) 
inner join category CO with (nolock) on I.CategoryID=CO.ID 
where I.[Status]=0 and I.ShowOrder>=0    and CO.ID in (select D.ID from @Layer4 D where D.ID =@dbcategoryid)
order by CO.ID,I.CreateDate desc,I.UpdateDate desc 

	
	
	end




	
	
	FETCH NEXT FROM mycursor INTO @dbcategoryid,@dbcategorylayer

END

CLOSE mycursor
DEALLOCATE mycursor
select  @dboutput=convert(varchar(100),getdate())







GO
