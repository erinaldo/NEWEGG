use TWSQLDB_PRD

select * from salesordergroup where ID=30019
select * from salesordergroupTemp

--購車商品
select * from salesorder
where SalesOrderGroupID >= '30023'
--where 0=0
----and Code='LBO160517000009'
--and AccountID=36433

select * from salesorderitem
where SalesorderCode >='LBO160518000003'


--在後台時是cart
use TWBACKENDDB_PRD
select * from process
where CartID='LBO160517000012'

--由商品找賣家
use TWSQLDB_PRD
select Discard4, SellerID,* from item
--update item set Discard4='Y'
where 0=0
--and ID=440402
and ( Discard4='Y' or Discard4='y' )

--癈四機回收四聯單
use TWBACKENDDB_PRD
select * from Discard4Item
--delete Discard4Item
where ID>=1033

use TWSQLDB_PRD
select top 1 * from itemstock
order by CreateDate desc

use TWSQLDB_PRD
select top 1 * from itemtemp
order by CreateDate desc


use TWSQLDB_PRD
select top 1 * from item
order by CreateDate desc

select ImageUrl,ID, Description from GreetingWords



















