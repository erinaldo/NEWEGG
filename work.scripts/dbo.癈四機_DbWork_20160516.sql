--dbo.¼o¥|¾÷
use TWSQLDB_PRD
--use TWSQLDB

declare @item_id varchar(50) ='473107'; --140,472077,125523,473107
--declare @category_id varchar(50) ='403';
--declare @itemtemp_id varchar(50) ='526032';

--LBO151026000009
declare @car_id varchar(20) = '29590';

 
 use TWBACKENDDB_PRD
 select * from Discard4Item
 --update Discard4Item set Discard4Flag='N'
 where 0=0
 --and SalesorderCode='LBO160422000028'
 --and ID='1023'
 
 use TWSQLDB_PRD
 select * from item
 --update item set Discard4='Y'
 where ID=@item_id
 
 select * from Discard4 
 
 
 

