--dbo.購物車+購物車的客戶資料+購物車的購買物品

use TWSQLDB_PRD

declare @car_id varchar(20) = '25092';

--dbo.購物車
select N'購物車', * from dbo.salesordergroup
where 0=0
and ID=@car_id

--dbo.購物車
select N'購物車的客戶資料',* from dbo.salesorder
where 0=0
and SalesOrderGroupID=@car_id
--and Code='LBO160422000028'
--and Name='黃耀進'
--and AccountID='36433'

select N'購物車的購買物品',* from dbo.salesorderitem
where 0=0
--and ItemID='125523'
and salesorderitem.SalesorderCode in (
	select Code from dbo.salesorder
	where 0=0
	and SalesOrderGroupID=@car_id
)





