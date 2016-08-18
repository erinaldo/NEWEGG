--dbo.商品審核
--vendor建立商品後, IPP進行商品審核

use TWSQLDB_PRD

--新建商品後資料在這
select Discard4,* from dbo.ItemSketch
--where ID='34669'
where 0=0
and Status='0'

--商品進行審核時資料在這
select Discard4, Status, CreateDate, UpdateDate,* from dbo.itemtemp
where 0=0
--and Status='1'
--and ItemID='473107'
and UpdateDate >='2016-05-18'
--商品審核後Status='1', 才會真正變成賣場品



--賣場品
select Discard4,* from dbo.Item
where 0=0
--and ID='473107'
and Discard4='Y'




