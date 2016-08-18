--dbo.會員與帳號
--use TWSQLDB
use TWSQLDB_PRD

select * from member m , account a
where 0=0
and a.ID = m.AccID
--and a.Email='cn27529@hotmail.com'
and a.Email='bh96@newegg.com'

--delete member where accid='36432'
--delete account where id='36432'




