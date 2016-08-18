--dbo.出貨

use TWBACKENDDB_PRD

declare @cart_id varchar(20) = 'LBO160422000028';
declare @process_id varchar(20) = 'LBS160422000030';

--更新主單狀態
--DelvStatus
--已出貨 = 1,
--配達 = 2,
--待出貨 = 0, //2014/07/11 Ted Modified 依要求將"出貨中"改為顯示"待出貨">>以跟前台顯示一致
--已成立 = 6,
--待進貨 = 7,
--已進貨 = 8,
--初始狀態 = 999,

--update cart set DelvStatus='1'
--where ID=@cart_id

--後台主單
select DelvStatus,* from cart
where ID=@cart_id

--子單
select * from process
where CartID=@cart_id

--回押單號
use TWSELLERPORTALDB
select * from Seller_DelvTrack
where ProcessID=@process_id


use TWSQLDB_PRD
select * from salesorder 
where Code=@cart_id

select * from salesorderitem
where SalesorderCode=@cart_id








