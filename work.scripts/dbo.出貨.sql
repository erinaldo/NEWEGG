--dbo.�X�f

use TWBACKENDDB_PRD

declare @cart_id varchar(20) = 'LBO160422000028';
declare @process_id varchar(20) = 'LBS160422000030';

--��s�D�檬�A
--DelvStatus
--�w�X�f = 1,
--�t�F = 2,
--�ݥX�f = 0, //2014/07/11 Ted Modified �̭n�D�N"�X�f��"�אּ���"�ݥX�f">>�H��e�x��ܤ@�P
--�w���� = 6,
--�ݶi�f = 7,
--�w�i�f = 8,
--��l���A = 999,

--update cart set DelvStatus='1'
--where ID=@cart_id

--��x�D��
select DelvStatus,* from cart
where ID=@cart_id

--�l��
select * from process
where CartID=@cart_id

--�^��渹
use TWSELLERPORTALDB
select * from Seller_DelvTrack
where ProcessID=@process_id


use TWSQLDB_PRD
select * from salesorder 
where Code=@cart_id

select * from salesorderitem
where SalesorderCode=@cart_id








