--dbo.���a
use TWSQLDB_PRD
select CreateDate, UpdateDate,Discard4,SellerID,* from item
--where ID=440402
where ID in ('489825','489834')


select ItemStatus , * from itemtemp
where ID in (
select ItemtempID from item
where ID in ('489826','489834')
)


--select * from itemtemp
select * from item
where Name like N'�L���K����123%'
