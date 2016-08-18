--出貨.還原已押過的回押單號 Tracking Number 的 訂單可以再押 Tracking Number

use TWBACKENDDB_PRD

select * from cart
--UPDATE dbo.cart
--SET DelivNO = null,
--Forwarder = 0, 
--DelvStatus = 0, 
--[Status] = 0 , 
--UpdateNote = N'12/4/2014 3:15:49 PM訂單成立,System</br>12/4/2014 3:20:02 PM系統轉單,System</br>'
where ID = 'LBO160422000028'
