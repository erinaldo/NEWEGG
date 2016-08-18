use TWSQLDB_PRD


select * from CartTemp
where SerialNumber='FVIBVGwtyk8UqlxQj7d/2Y75/hWVBFgFl4Op1abdYGpLKS1pmEYpYHkywsq6R5eG5WzIi+54sEzy25XmvPnMCq8bfM3Rw0SbQo2VgVrB+cI='

select * from CartCouponTemp order by CartTempID
select * from CartTemp
select * from salesordergroup where ID=30019
select * from salesordergroupTemp

select * from salesorder
where SalesOrderGroupID in ('30020','30019','30018')

--where 0=0
----and Code='LBO160517000009'
--and AccountID=36433


use TWBACKENDDB_PRD
select * from process
where CartID='LBO160517000012'

use TWSQLDB_PRD
select * from Item
where ID=473107









