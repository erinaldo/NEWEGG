--dbo.�ӫ~�f��
--vendor�إ߰ӫ~��, IPP�i��ӫ~�f��

use TWSQLDB_PRD

--�s�ذӫ~���Ʀb�o
select Discard4,* from dbo.ItemSketch
--where ID='34669'
where 0=0
and Status='0'

--�ӫ~�i��f�֮ɸ�Ʀb�o
select Discard4, Status, CreateDate, UpdateDate,* from dbo.itemtemp
where 0=0
--and Status='1'
--and ItemID='473107'
and UpdateDate >='2016-05-18'
--�ӫ~�f�֫�Status='1', �~�|�u���ܦ�����~



--����~
select Discard4,* from dbo.Item
where 0=0
--and ID='473107'
and Discard4='Y'




