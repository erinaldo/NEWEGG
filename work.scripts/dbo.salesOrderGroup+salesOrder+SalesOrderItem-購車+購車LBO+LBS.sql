--dbo.�ʪ���+�ʪ������Ȥ���+�ʪ������ʶR���~

use TWSQLDB_PRD

declare @car_id varchar(20) = '25092';

--dbo.�ʪ���
select N'�ʪ���', * from dbo.salesordergroup
where 0=0
and ID=@car_id

--dbo.�ʪ���
select N'�ʪ������Ȥ���',* from dbo.salesorder
where 0=0
and SalesOrderGroupID=@car_id
--and Code='LBO160422000028'
--and Name='��ģ�i'
--and AccountID='36433'

select N'�ʪ������ʶR���~',* from dbo.salesorderitem
where 0=0
--and ItemID='125523'
and salesorderitem.SalesorderCode in (
	select Code from dbo.salesorder
	where 0=0
	and SalesOrderGroupID=@car_id
)





