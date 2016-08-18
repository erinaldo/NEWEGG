use TWSQLDB_PRD

declare @email varchar(20) = 'bh96@newegg.com';

--dbo.EC�ʪ���
select N'�ʪ���',* from salesordergroup with (nolock) 
where ID in (
	select SalesOrderGroupID from salesorder with (nolock) 
	where salesorder.Email=@email
)

--dbo.EC�ʪ�������
select  N'�ʪ�������',* from salesorder with (nolock) 
where salesorder.Email=@email
--order by CreateDate desc 


--dbo.EC�ʪ������Ӥ��e
select N'�ʪ������Ӥ��e' ,* from salesorderitem with (nolock) 
where SalesorderCode in (
	select Code from salesorder with (nolock) 
	where salesorder.Email=@email
)
--order by CreateDate desc

--select N'����~��������'
select N'����~��������' , ItemID, i.SellerID, s.Name from salesorderitem soi with (nolock) , item i , seller s
where 0=0
and soi.ItemID = i.ID
and s.ID = i.SellerID
and soi.SalesorderCode in (
	select Code from salesorder with (nolock) 
	where salesorder.Email=@email
)

