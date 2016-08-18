--dbo.廢四機

use TWSQLDB_PRD
--use TWSQLDB

declare @item_id varchar(50) ='125523'; --140,472077,125523
--declare @category_id varchar(50) ='403';
--declare @itemtemp_id varchar(50) ='526032';


--IPP.查詢資料輸入
select N'賣場品'
,*
 from dbo.salesorder
 where AccountID='36433'
 
 --select * from salesorderitem
 
 select * from account
 where Email='bh96@newegg.com'
 
 select * from member
 where AccID=36433
 
 use TWSQLDB_PRD
 select SellerID, * from item
 --update item set Discard4='y'
 where ID=125523
 
 
 
 
 use TWBACKENDDB_PRD
 select * from Discard4Item
 
 

