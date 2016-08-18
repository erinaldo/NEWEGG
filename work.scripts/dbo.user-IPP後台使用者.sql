--dbo.後台使用者
--use TWBACKENDDB
use TWBACKENDDB_PRD

select * from [user]
where 0=0
--and Name='bh96'
and Email like 'bruce.y.huang%'
--and a.Email='bruce.y.huang@newegg.com'




--select * from dbo.userfunc
