use TWSQLDB_PRD

declare @email varchar(20) = 'bh96@newegg.com';

--dbo.EC購物車
select N'購物車',* from salesordergroup with (nolock) 
where ID in (
	select SalesOrderGroupID from salesorder with (nolock) 
	where salesorder.Email=@email
)

--dbo.EC購物車明細
select  N'購物車明細',* from salesorder with (nolock) 
where salesorder.Email=@email
--order by CreateDate desc 


--dbo.EC購物車明細內容
select N'購物車明細內容' ,* from salesorderitem with (nolock) 
where SalesorderCode in (
	select Code from salesorder with (nolock) 
	where salesorder.Email=@email
)
--order by CreateDate desc

--select N'賣場品的供應商'
select N'賣場品的供應商' , ItemID, i.SellerID, s.Name from salesorderitem soi with (nolock) , item i , seller s
where 0=0
and soi.ItemID = i.ID
and s.ID = i.SellerID
and soi.SalesorderCode in (
	select Code from salesorder with (nolock) 
	where salesorder.Email=@email
)

