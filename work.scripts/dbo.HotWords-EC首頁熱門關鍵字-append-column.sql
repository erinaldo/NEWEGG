use TWSQLDB
--use TWSQLDB_PRD
select * from dbo.HotWords
where 0=0
--and ShowAll=1
order by Showorder

----append schema by bruce------------------from BSATW-172 �ݭԥd, BSATW-178 �i���ݭԻy�Ҳ�

--ALTER TABLE  dbo.HotWords
----ALTER COLUMN Name nvarchar(50) --�W��
--ADD Name nvarchar(50)

----MemoCode �ۭq�N�X��r(new add 20160324)
--ALTER TABLE  dbo.HotWords  ADD MemoCode nvarchar(50)
--ALTER TABLE  dbo.HotWords ALTER COLUMN MemoCode nvarchar(50)
--ALTER TABLE  dbo.HotWords DROP COLUMN MemoCode

----CategoryId
----0 ������������r
----1 �n�J�ݭԻy
----2 �`��ݭԥd

----Images Uniform Resource Locator �Ϥ���m
----CategoryId�O2�O�٬� ���ʯ���(new add 20160324)
--ALTER TABLE  dbo.HotWords
----ALTER COLUMN ImagesURL nvarchar(100)
--ADD ImagesURL nvarchar(100) 

--ALTER TABLE  dbo.HotWords
--ALTER COLUMN [Description] nvarchar(50)

----ALTER TABLE  dbo.HotWords DROP COLUMN MemoCode











