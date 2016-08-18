--dbo.cart+process

use TWBACKENDDB_PRD

declare @salesorder_code varchar(20) = 'LBO160517000012';
declare @user_id varchar(20) = '36433';


select  * from cart
where 0=0
and ID=@salesorder_code
and UserID=@user_id


select * from process
where 0=0
and CartID =@salesorder_code


